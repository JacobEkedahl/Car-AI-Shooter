using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarAI : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public GameObject target_handler;

        public GameObject terrain_manager_game_object;
        TerrainManager terrain_manager;
        EnemyPlanner enemy_planner;
        CarManager generator;

        public List<GameObject> enemies;
        public List<GameObject> lineOfSight_enemies = new List<GameObject>();

        private List<Vector3> nodesToGoal;
        private int currIndex = -1; //which node to target
        private AStar astar;

        public bool hasEnemies { get; set; } = false;
        public void setEnemies(List<GameObject> objects) {
            enemies = objects;
            hasEnemies = true;
        }

        private void Start()
        {
            //Cluster manager, car ask for manager for which targets to find
            generator = target_handler.GetComponent<CarManager>();
            nodesToGoal = new List<Vector3>();

            // get the car controller
            m_Car = GetComponent<CarController>();
            terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
            enemy_planner = new EnemyPlanner();

            //retrieve the list of nodes from my position to next pos
            GridDiscretization grid = new GridDiscretization(terrain_manager.myInfo, 1, 1, 0);
            astar = new AStar(grid, false); //astar loads this grid into a internal voronoigrid, the targets are not turrets
        }
        
        private void remove_close_box()
        {
            if (Vector3.Distance(transform.position, enemies[0].transform.position) <= 3.0f)
            {
                Debug.Log("reached target");
                generator.reachedTarget = true;
                respawn();
            }
        }

        private void respawn() {
            enemies.Clear();
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.position = generator.getStartPos();
            transform.eulerAngles = new Vector3(10, generator.getRotation(), 0);
            current_target = null; //need to set to null to generate new path to next node
        }

        List<Spot> path = new List<Spot>();
        private void FixedUpdate()
        {
            if (generator.canFetch() && !generator.hasFetched)
            {
                enemies = generator.getTargets();
            } else if (generator.hasFetched) {
                runAstar();
                List<float> car_input = get_car_input();
                if (car_input == null)
                {
                    return;
                }

                float steering = car_input[0];
                float acceleration = car_input[1];

                if (current_target == null)
                {
                    replan();
                }

                if (go_back)
                {
                    go_back_routine(-1.0f);
                }
                else if (go_forward)
                {
                    go_back_routine(1.0f);
                }
                else
                {
                    m_Car.Move(steering, acceleration, acceleration, 0f);
                }
            }
        }


        private List<float> get_car_input()
        {
            if (currIndex == -1 || nodesToGoal.Count == 0)
                return null;

            Vector3 target_node = nodesToGoal[currIndex];
            Vector3 direction = (target_node - transform.position).normalized;

            bool is_to_the_right = Vector3.Dot(direction, transform.right) > 0f;
            bool is_to_the_front = Vector3.Dot(direction, transform.forward) > 0f;

            float steering = (Vector3.Angle(direction, transform.forward));
            if (steering >= 25f) {
                steering = 1.0f;
            } else {
                steering /= 25.0f;
            }
            
            float acceleration = 0;

            if (is_to_the_right && is_to_the_front)
            {
                //steering = 1f;
                acceleration = 1f;
            }
            else if (is_to_the_right && !is_to_the_front)
            {
                steering *= -1f;
                acceleration = -1f;
            }
            else if (!is_to_the_right && is_to_the_front)
            {
                steering *= -1f;
                acceleration = 1f;
            }
            else if (!is_to_the_right && !is_to_the_front)
            {
                //steering = 1f;
                acceleration = -1f;
            }

            if (m_Car.CurrentSpeed > 17) acceleration = 0;

            List<float> car_input = new List<float>();
            car_input.Add(steering);
            car_input.Add(acceleration);

            return car_input;
        }

        //Methods used for pathplanning ---------------------------------------------------------

        private bool can_run = true;
        private void runAstar()
        {
            remove_close_box();
            if (!can_update && can_run && enemies.Contains(current_target))
            {
                nodesToGoal = astar.getPath(transform.position, true); //goal has already been loaded in updatePath
                Debug.Log("goal: " + astar.goal.pos + " start: " + astar.start.pos);
                if (nodesToGoal == null) {
                } else
                {
                    can_run = false;
                }
            }
            else if (can_update)
            {
                updatePath();
            }
            else
            {
                findFurthestTarget(); //normal running
            }
        }

        private bool can_update = true;
        private GameObject current_target;

        private GameObject get_next_target() {
            for (int i = 0; i < enemies.Count; i++) {
                if (!(enemies[i] == null)){
                    return enemies[i];
                }
            }
            return null;
        }

        private void updatePath()
        {
            load_lineOfSight();
            current_target = get_next_target();
            if (current_target == null) {
                current_target = this.gameObject;
            }
        
            current_target = get_next_target();

            astar.initAstar(transform.position, current_target.transform.position);
            currIndex = 0;

         //   Debug.DrawLine(current_target.transform.position, transform.position, Color.blue, 100);
            can_update = false;
        }

        //Methods used for dealing with collisions --------------------------------
        private bool is_coll_back()
        {
            Vector3 offset_forward = transform.forward * 3;
            Vector3 offset_side = transform.right * 1;
            Vector3 left_pos = transform.position + offset_forward + offset_side;
            Vector3 right_pos = transform.position + offset_forward - offset_side;

            int layerMask = LayerMask.GetMask("CubeWalls");

            if ((Physics.Linecast(transform.position, left_pos, layerMask)) ||
                (Physics.Linecast(transform.position, right_pos, layerMask)))
            {
                return true;
            }
            return false;
        }

        private bool go_back = false;
        private void go_back_routine(float dir)
        {
            if (timer > 0)
            {
                timer--;
                m_Car.Move(0.0f, dir, dir, 0.0f);
            }
            else
            {
                timer = 100;
                go_back = false;
                go_forward = false;
            }
        }
        
        private int timer = 100;
        private void OnDrawGizmos()
        {
            if(Application.isPlaying)
            {
                if (nodesToGoal.Count > 0)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, nodesToGoal[currIndex]);
                }

                Gizmos.color = Color.red;
                for (int i = 0; i < enemies.Count - 1; i++ ){
                    if (enemies[i] == null) {
                        continue;
                    }

                    Gizmos.color = Color.red;
                    if (enemies[i + 1] == null){
                        continue;
                    }
                    Vector3 from = enemies[i].transform.position;
                    from.y += 1;
                    Vector3 to = enemies[i + 1].transform.position;
                    to.y += 1;
                    Gizmos.DrawLine(from, to);
                }
            }
        }

        private void OnCollisionExit(Collision other)
        {
            coll_timer = 100;
        }

        private bool go_forward = false;
        private int coll_timer = 100;
        private void OnCollisionStay(Collision other)
        {
            if (coll_timer == 100)
            {   
                if (other.gameObject.tag == "Player")
                {
                    int choice = Random.Range(0, 2);
                    if (choice == 0)
                    {
                        go_back = true;
                    } else
                    {
                        go_forward = true;
                    }
                } else if (is_coll_back())
                {
                    go_back = true;
                }
            }

            coll_timer--;
            if (coll_timer <= 0)
                coll_timer = 100;
        }

        //Methods used for following path and demanding new path to be generated --------------
        private void replan()
        {
            enemies = enemy_planner.remove_destroyed(enemies);
            currIndex = 0;
            can_run = true;
            can_update = true;
            astar.openSet.Clear();
        }

        private void findFurthestTarget()
        {
            if (nodesToGoal.Count == 0)
            {
                return;
            }

            //sets target to the furthest visible node on path
            if (!car_can_see())
            {
                //car has no vision to path
            }
        }

        private bool car_can_see()
        {
            Vector3 offset = transform.right;
            Vector3 offset_backward = -transform.forward * 2;
            Vector3 offset_forward = transform.forward * 2;


            offset *= 2;
            Vector3 right_pos_forward = transform.position + offset_forward + offset;
            Vector3 left_pos_forward = transform.position + offset_forward + - offset;
            Vector3 right_pos_backward = transform.position + offset_backward + offset;
            Vector3 left_pos_backward = transform.position + offset_backward + -offset;

            for (int i = nodesToGoal.Count - 1; i >= 0; i--)
            {
                if (can_see(left_pos_forward, nodesToGoal[i]) && can_see(right_pos_forward, nodesToGoal[i]) &&
                    can_see(left_pos_backward, nodesToGoal[i]) && can_see(right_pos_backward, nodesToGoal[i]))
                {
                    currIndex = i;
                    return true;
                }
            }

            return false;
        }

        private bool can_see(Vector3 from, Vector3 other_pos)
        {
            int layer_mask = LayerMask.GetMask("CubeWalls");
            if (!Physics.Linecast(from, other_pos, layer_mask))
            {
                Debug.DrawLine(from, other_pos, Color.green, 0.1f);
                return true;
            }
            return false;
        }

        private void load_lineOfSight()
        {
            lineOfSight_enemies.Clear();

            foreach (GameObject enemy in enemies)
            {
                if (enemy == null)
                {
                    continue;
                }

                if (can_see(transform.position, enemy.transform.position))
                {
                    lineOfSight_enemies.Add(enemy);
                }
            }
        }
    }
}
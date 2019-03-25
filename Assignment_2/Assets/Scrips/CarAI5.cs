using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car {
    [RequireComponent(typeof(CarController))]
    public class CarAI5 : MonoBehaviour {
        private CarController m_Car; // the car controller we want to use

        public GameObject terrain_manager_game_object;
        TerrainManager terrain_manager;
        EnemyPlanner enemy_planner;

        public GameObject cluster_manager_object;
        TargetHandler target_handler;

        public List<GameObject> enemies;
        public List<GameObject> lineOfSight_enemies = new List<GameObject>();

        private List<Vector3> nodesToGoal = new List<Vector3>();
        private int currIndex = -1; //which node to target
        private AStar astar;
        private VRPsolver vrp;
        private int loadTime = 100;

        private void Start() {
            //Cluster manager, car ask for manager for which targets to find
            target_handler = cluster_manager_object.GetComponent<TargetHandler>();
            //Debug.Log("value from targethandler: " + target_handler.no_clusters + ", enemies: " + target_handler.no_enemies);


            // get the car controller
            m_Car = GetComponent<CarController>();
            terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
            enemy_planner = new EnemyPlanner();

            //retrieve the list of nodes from my position to next pos
            GridDiscretization grid = new GridDiscretization(terrain_manager.myInfo, 1, 1, 4);
            astar = new AStar(grid, true); //astar loads this grid into a internal voronoigrid, the targets are turrets
        }

        private void remove_close_box() {
            foreach (GameObject box in enemies) {
                if (box == null)
                    continue;
                if (Vector3.Distance(transform.position, box.transform.position) <= 3.0f) {
                    Destroy(box);
                }
            }
        }

        private bool has_fetched = false;
        private void fetch_clusters() {
            while (!target_handler.has_clustered) {
                //wait
            }

            //fetch clusters if not has fetched
            if (!has_fetched) {
                enemies = target_handler.getCluster();
                has_fetched = true;
            }
        }

        private void FixedUpdate() {
            //Time.timeScale = 10.0f;
            if (loadTime > 0) { //waiting for car to settle after landing from sky
                loadTime--;
                go_back_routine(1.0f);
            } else {
                fetch_clusters();

                if (has_fetched) {

                    runAstar();


                    List<float> car_input = get_car_input();
                    if (car_input == null) {
                        return;
                    }

                    float steering = car_input[0];
                    float acceleration = car_input[1];
                    float breaking = 0f;

                    if (current_target == null) {
                        replan();
                    }

                    if (go_back) {
                        go_back_routine(-1.0f);
                    } else if (go_forward) {
                        go_back_routine(1.0f);
                    } else {
                        if(peek(this.gameObject, 10)){
                            Debug.Log("Positive peek!");
                            if(m_Car.CurrentSpeed > 6){
                                acceleration = 0f;
                                breaking = 1f;
                            }
                        }
                        m_Car.Move(steering, acceleration, acceleration, breaking);
                    }
                }
            }

        }

        private bool peek(GameObject leader_car, float peek_distance){
            Vector3 look_ahead = leader_car.transform.position + leader_car.transform.forward * peek_distance;

            for(int i = 0; i < enemies.Count; i++){
                int layerMask = LayerMask.GetMask("CubeWalls");
                if (enemies[i] == null) continue;
                if (!Physics.Linecast(look_ahead, enemies[i].transform.position, layerMask)) {
                    Debug.DrawLine(look_ahead, enemies[i].transform.position, Color.cyan);
                    return true;
                }
            }
            return false;
        }


        private List<float> get_car_input() {
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

            if (is_to_the_right && is_to_the_front) {
                //steering = 1f;
                acceleration = 1f;
            } else if (is_to_the_right && !is_to_the_front) {
                steering *= -1f;
                acceleration = -1f;
            } else if (!is_to_the_right && is_to_the_front) {
                steering *= -1f;
                acceleration = 1f;
            } else if (!is_to_the_right && !is_to_the_front) {
                //steering = 1f;
                acceleration = -1f;
            }

            List<float> car_input = new List<float>();
            car_input.Add(steering);
            car_input.Add(acceleration);

            return car_input;
        }

        //Methods used for pathplanning ---------------------------------------------------------

        private bool can_run = true;
        private void runAstar() {
            remove_close_box();
            if (!can_update && can_run && enemies.Contains(current_target)) {
                nodesToGoal = astar.getPath(); //goal has already been loaded in updatePath
                if (nodesToGoal.Count > 0) {
                    can_run = false;
                }
            } else if (can_update) {
                updatePath();
            } else {
                findFurthestTarget(); //normal running
            }
        }

        private bool can_update = true;
        private GameObject current_target;

        private GameObject get_next_target() {
            List<GameObject> possibleTargets = new List<GameObject>();
            GameObject next_target;

            for (int i = 0; i < enemies.Count; i++) {
                if (!(enemies[i] == null)) {
                    possibleTargets.Add(enemies[i]);
                }
            }

            float minDistance = float.MaxValue;
            if (possibleTargets.Count == 2) {
                astar.initAstar(this.transform.position, possibleTargets[0].transform.position);
                List<Vector3> path = astar.getPath();
                path = astar.reconstructPath(path);
                float dist_target_one = astar.dist_astar(path);

                astar.initAstar(this.transform.position, possibleTargets[1].transform.position);
                List<Vector3> path2 = astar.getPath();
                path2 = astar.reconstructPath(path2);
                float dist_target_two = astar.dist_astar(path2);

                if (dist_target_one < dist_target_two) {
                    return possibleTargets[0];
                } else {
                    return possibleTargets[1];
                }

            } else if (possibleTargets.Count != 0) {
                return possibleTargets[0];
            }

            return null;
        }

        private void updatePath() {
            load_lineOfSight();
            current_target = get_next_target();
            if (current_target == null) {
                current_target = this.gameObject;
            }

            astar.initAstar(transform.position, current_target.transform.position);
            currIndex = 0;

            // Debug.DrawLine(current_target.transform.position, transform.position, Color.blue, 100);
            can_update = false;
        }

        //Methods used for dealing with collisions --------------------------------
        private bool is_coll_back() {
            Vector3 offset_forward = transform.forward * 3;
            Vector3 offset_side = transform.right * 1;
            Vector3 left_pos = transform.position + offset_forward + offset_side;
            Vector3 right_pos = transform.position + offset_forward - offset_side;

            int layerMask = LayerMask.GetMask("CubeWalls");

            if ((Physics.Linecast(transform.position, left_pos, layerMask)) ||
                (Physics.Linecast(transform.position, right_pos, layerMask))) {
                return true;
            }
            return false;
        }

        private bool go_back = false;
        private void go_back_routine(float dir) {
            if (timer > 0) {
                timer--;
                m_Car.Move(0.0f, dir, dir, 0.0f);
            } else {
                timer = 100;
                go_back = false;
                go_forward = false;
            }
        }

        private int timer = 100;

        /* 
        private void OnDrawGizmos() {
            if (Application.isPlaying) {
                if (nodesToGoal.Count > 0) {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, nodesToGoal[currIndex]);
                }

                Gizmos.color = Color.red;
                for (int i = 0; i < enemies.Count - 1; i++) {
                    if (enemies[i] == null) {
                        continue;
                    }
                    // Gizmos.color = Color.blue;
                    // Gizmos.DrawLine(transform.position, enemies[i].transform.position);

                    Gizmos.color = Color.red;
                    if (enemies[i + 1] == null) {
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
        */

        private void OnCollisionExit(Collision other) {
            coll_timer = 100;
        }

        private bool go_forward = false;
        private int coll_timer = 100;
        private void OnCollisionStay(Collision other) {
            if (coll_timer == 100) {
                if (other.gameObject.tag == "Player") {
                    int choice = Random.Range(0, 2);
                    if (choice == 0) {
                        go_back = true;
                    } else {
                        go_forward = true;
                    }
                } else if (is_coll_back()) {
                    go_back = true;
                }
            }

            coll_timer--;
            if (coll_timer <= 0)
                coll_timer = 100;
        }

        //Methods used for following path and demanding new path to be generated --------------
        private void replan() {
            enemies = enemy_planner.remove_destroyed(enemies);
            currIndex = 0;
            can_run = true;
            can_update = true;
            astar.openSet.Clear();
        }

        private void findFurthestTarget() {
            if (nodesToGoal.Count == 0) {
                return;
            }

            //sets target to the furthest visible node on path
            if (!car_can_see()) {
                //car has no vision to path
            }
        }

        private bool car_can_see() {
            Vector3 offset = transform.right;
            Vector3 offset_backward = -transform.forward * 2;
            Vector3 offset_forward = transform.forward * 2;


            offset *= 2;
            Vector3 right_pos_forward = transform.position + offset_forward + offset;
            Vector3 left_pos_forward = transform.position + offset_forward + -offset;
            Vector3 right_pos_backward = transform.position + offset_backward + offset;
            Vector3 left_pos_backward = transform.position + offset_backward + -offset;


            for (int i = nodesToGoal.Count - 1; i >= 0; i--) {
                Vector3 left_other = nodesToGoal[i] + offset;
                Vector3 right_other = nodesToGoal[i] - offset;

                if (can_see(left_pos_forward, left_other) && can_see(right_pos_forward, left_other) &&
                    can_see(left_pos_backward, left_other) && can_see(right_pos_backward, left_other) &&
                    can_see(left_pos_forward, right_other) && can_see(right_pos_forward, right_other) &&
                    can_see(left_pos_backward, right_other) && can_see(right_pos_backward, right_other)) {
                    currIndex = i;
                    return true;
                }
            }

            return false;
        }

        private bool can_see(Vector3 from, Vector3 other_pos) {
            Vector3 offset_right = transform.right;
            Vector3 offset_top = transform.forward;

            int layer_mask = LayerMask.GetMask("CubeWalls");
            if (!Physics.Linecast(from, other_pos, layer_mask)) {
                Debug.DrawLine(from, other_pos, Color.green, 0.1f);
                return true;
            }
            return false;
        }

        private void load_lineOfSight() {
            lineOfSight_enemies.Clear();

            foreach (GameObject enemy in enemies) {
                if (enemy == null) {
                    continue;
                }

                if (can_see(transform.position, enemy.transform.position)) {
                    lineOfSight_enemies.Add(enemy);
                }
            }
        }
    }
}

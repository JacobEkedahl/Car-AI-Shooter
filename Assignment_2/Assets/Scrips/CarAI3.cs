using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarAI3 : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        public GameObject terrain_manager_game_object;
        TerrainManager terrain_manager;
        EnemyPlanner enemy_planner;

        public GameObject cluster_manager_object;
        TargetHandler target_handler;

        public GameObject[] friends;
        public List<GameObject> enemies;
        public List<GameObject> lineOfSight_enemies = new List<GameObject>();

        private List<Vector3> nodesToGoal = new List<Vector3>();
        private int currIndex = 0; //which node to target
        private AStar astar;
        private int loadTime = 100;

        private void Start()
        {
            //Cluster manager, car ask for manager for which targets to find
            target_handler = cluster_manager_object.GetComponent<TargetHandler>();
            //Debug.Log("value from targethandler: " + target_handler.no_clusters + ", enemies: " + target_handler.no_enemies);
            

            // get the car controller
            m_Car = GetComponent<CarController>();
            terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
            enemy_planner = new EnemyPlanner();

            // note that both arrays will have holes when objects are destroyed
            // but for initial planning they should work
            friends = GameObject.FindGameObjectsWithTag("Player");
            //enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));

            //retrieve the list of nodes from my position to next pos
            GridDiscretization grid = new GridDiscretization(terrain_manager.myInfo);
            astar = new AStar(grid, true); //astar loads this grid into a internal voronoigrid
        }

        private bool has_fetched = false;
        private void fetch_clusters()
        {
            while (!target_handler.has_clustered)
            {
                //wait
            }

            //fetch clusters if not has fetched
            if (!has_fetched)
            {
                enemies = target_handler.getCluster();
                has_fetched = true;
            }
        }

        List<Spot> path = new List<Spot>();
        private void FixedUpdate()
        {
            if (loadTime > 0)
            { //waiting for car to settle after landing from sky
                loadTime--;
                go_back_routine(1.0f);
            }
            else
            {
                fetch_clusters();
                runAstar();
            }

            List<float> car_input = get_car_input();
            float steering = car_input[0];
            float acceleration = car_input[1];

            if (current_target == null)
            {
                replan();
            }

            if (go_back)
            {
                go_back_routine(-1.0f);
            } else if (go_forward)
            {
                go_back_routine(1.0f);
            }
            else
            {
                m_Car.Move(steering, acceleration, acceleration, 0f);
            }
        }


        private List<float> get_car_input()
        {
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

            if (m_Car.CurrentSpeed > 20) acceleration = 0;

            List<float> car_input = new List<float>();
            car_input.Add(steering);
            car_input.Add(acceleration);

            return car_input;
        }

        //Methods used for pathplanning ---------------------------------------------------------

        private bool can_run = true;
        private void runAstar()
        {
            if (!can_update && can_run && enemies.Contains(current_target))
            {
                nodesToGoal = astar.getPath(transform.position, true); //goal has already been loaded in updatePath
                if (nodesToGoal.Count > 0)
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
        private void updatePath()
        {
            load_lineOfSight();

            if (lineOfSight_enemies.Count > 0)
            {
                current_target = enemy_planner.get_closest_object(lineOfSight_enemies, transform.position);
            }
            else
            {
                current_target = enemy_planner.get_closest_object(enemies, transform.position);
            }

            astar.initAstar(transform.position, current_target.transform.position);
            currIndex = 0;

            Debug.DrawLine(current_target.transform.position, transform.position, Color.blue, 100);
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
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, nodesToGoal[currIndex]);
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
                    Debug.Log("Collision with another player!");
                    int choice = Random.Range(0, 2);
                    if (choice == 0)
                    {
                        go_back = true;
                    } else
                    {
                        go_forward = true;
                        Debug.Log("not zero!");
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
            Debug.Log("reset!");
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

/* Methods used when visualizing the pathfinding algorithm and car movement */

/*
    private void runAstar () {

        if (astar.openSet.Count > 0 && can_run && !can_update && enemies.Contains (current_target)) {
            Debug.Log ("calculating path in fixedupdate.., start: " + astar.start.wall + ", goal: " + astar.goal.wall + ", goalpos: " + astar.goal.pos + " target is null: " + (current_target == null));
            var winner = 0;
            for (int index = 0; index < astar.openSet.Count; index++) {
                if (astar.openSet[index].f < astar.openSet[winner].f) {
                    winner = index;
                }
            }

            var current = astar.openSet[winner];

            var goal = astar.goal;
            int dist_to_goal = 3;
            if (astar.grid.grid_distance[goal.i, goal.j] == -1) {
                dist_to_goal = 5;
            }
            //find the path
            if (Vector3.Distance (current.pos, astar.goal.pos) < dist_to_goal) {
                nodesToGoal.Clear ();
                var temp = current;
                nodesToGoal.Add (temp.pos);

                while (temp.previous != null) {
                    nodesToGoal.Add (temp.previous.pos);
                    temp = temp.previous;
                }

                int size_of_path = nodesToGoal.Count;
                int modVal = 5;

                if (size_of_path > 20) {
                    modVal = 10;
                } else if (size_of_path < 6) {
                    modVal = 2;
                }

                for (int node_i = size_of_path - 1; node_i >= 0; node_i--) {
                    if (node_i % modVal != 0) {
                        Debug.Log ("removed");
                        nodesToGoal.RemoveAt (node_i);
                    }
                }

                nodesToGoal.Reverse ();

                can_run = false;
                DrawPath (nodesToGoal);

                // findFurthestTarget ();
                return;
            }

            astar.openSet.Remove (current);
            astar.closedSet.Add (current);

            var neighbors = current.neighbors;

            for (int k = 0; k < neighbors.Count; k++) {
                var neighbor = neighbors[k];
                if (!astar.closedSet.Contains (neighbor) && !neighbor.wall) {
                    var tempG = current.g + 1 * astar.getSplit ();

                    if (astar.openSet.Contains (neighbor)) {
                        if (tempG < neighbor.g) {
                            neighbor.g = tempG;
                        }
                    } else {
                        neighbor.g = tempG;
                        astar.openSet.Add (neighbor);
                    }

                    neighbor.h = astar.heuristic (current.previous, current, neighbor, astar.goal);
                    neighbor.f = neighbor.g + neighbor.h;
                    neighbor.previous = current;
                }
            }
            animateAstar ();
        } else {
            if (can_update) {
                Debug.Log ("is updating");
                updatePath ();
            } else {
                findFurthestTarget ();
            }

        }
    }

    private void animateAstar () {
        foreach (Spot spot in astar.closedSet) {
            spot.Draw (Color.red);
        }

        foreach (Spot spot in astar.openSet) {
            spot.Draw (Color.green);
        }
    }

    private void DrawPath (List<Vector3> path) {
        for (int i = 0; i < path.Count - 1; i++) {
            Debug.DrawLine (path[i], path[i + 1], Color.red, 100);
        }
    }
    */

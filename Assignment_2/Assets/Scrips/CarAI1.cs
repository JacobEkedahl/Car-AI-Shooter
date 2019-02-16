using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car {
    [RequireComponent (typeof (CarController))]
    public class CarAI1 : MonoBehaviour {
        private CarController m_Car; // the car controller we want to use

        public GameObject terrain_manager_game_object;
        TerrainManager terrain_manager;
        EnemyPlanner enemy_planner;

        public GameObject[] friends;
        public List<GameObject> enemies;

        PathHandler pathHandler;
        private List<Vector3> nodesToGoal = new List<Vector3> ();
        private int currIndex = 0;
        private AStar astar;

        private void Start () {
            // get the car controller
            m_Car = GetComponent<CarController> ();
            terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager> ();
            enemy_planner = new EnemyPlanner ();

            // note that both arrays will have holes when objects are destroyed
            // but for initial planning they should work
            friends = GameObject.FindGameObjectsWithTag ("Player");
            enemies = new List<GameObject> (GameObject.FindGameObjectsWithTag ("Enemy"));

            // Plan your path here
            // pathHandler = new PathHandler (terrain_manager);

            //retrieve the list of nodes from my position to next pos
            GridDiscretization grid = new GridDiscretization (terrain_manager.myInfo);
            astar = new AStar (grid);
            updatePath ();
            // astar.initAstar();
        }

        private void animateAstar () {
            foreach (Spot spot in astar.closedSet) {
                spot.Draw (Color.red);
            }

            foreach (Spot spot in astar.openSet) {
                spot.Draw (Color.green);
            }

            DrawPath ();
        }

        private void DrawPath () {
            foreach (Spot spot in path) {
                spot.Draw (Color.yellow);
            }
        }

        List<Spot> path = new List<Spot> ();

        private void FixedUpdate () {
            if (astar.openSet.Count > 0) {
                var winner = 0;
                for (int index = 0; index < astar.openSet.Count; index++) {
                    if (astar.openSet[index].f < astar.openSet[winner].f) {
                        winner = index;
                    }
                }

                var current = astar.openSet[winner];

                //find the path
                if (current == astar.goal) {
                    var temp = current;
                    path.Add (temp);

                    while (temp.previous != null) {
                        path.Add (temp.previous);
                        temp = temp.previous;
                    }
                    Debug.Log ("done!");
                }

                astar.openSet.Remove (current);
                astar.closedSet.Add (current);

                var neighbors = current.neighbors;
                Debug.Log (neighbors.Count);

                for (int k = 0; k < neighbors.Count; k++) {
                    var neighbor = neighbors[k];
                    if (!astar.closedSet.Contains (neighbor) && !neighbor.wall) {
                        var tempG = current.g + 1;

                        if (astar.openSet.Contains (neighbor)) {
                            if (tempG < neighbor.g) {
                                neighbor.g = tempG;
                            }
                        } else {
                            neighbor.g = tempG;
                            astar.openSet.Add (neighbor);
                        }

                        neighbor.h = astar.heuristic (neighbor, astar.goal);
                        neighbor.f = neighbor.g + neighbor.h;
                        neighbor.previous = current;
                    }
                }
                animateAstar ();

            }

            Debug.Log ("executing path..");
            // Execute your path here
            // ...

            Debug.Log ("current index: " + currIndex);
            Debug.Log ("nodesToGoal: " + nodesToGoal.Count);
            Vector3 target_node = nodesToGoal[currIndex];
            Vector3 direction = (target_node - transform.position).normalized;

            bool is_to_the_right = Vector3.Dot (direction, transform.right) > 0f;
            bool is_to_the_front = Vector3.Dot (direction, transform.forward) > 0f;

            float steering = 0f;
            float acceleration = 0;

            if (is_to_the_right && is_to_the_front) {
                steering = 1f;
                acceleration = 1f;
            } else if (is_to_the_right && !is_to_the_front) {
                steering = -1f;
                acceleration = -1f;
            } else if (!is_to_the_right && is_to_the_front) {
                steering = -1f;
                acceleration = 1f;
            } else if (!is_to_the_right && !is_to_the_front) {
                steering = 1f;
                acceleration = -1f;
            }

            if (Vector3.Distance (transform.position, target_node) < 3) {
                //  setNextTarget ();
            }

            steering *= dir_car;
            steering *= dir_car;

            // this is how you access information about the terrain
            int i = terrain_manager.myInfo.get_i_index (transform.position.x);
            int j = terrain_manager.myInfo.get_j_index (transform.position.z);
            float grid_center_x = terrain_manager.myInfo.get_x_pos (i);
            float grid_center_z = terrain_manager.myInfo.get_z_pos (j);

            Debug.DrawLine (transform.position, new Vector3 (grid_center_x, 0f, grid_center_z));

            // this is how you control the car
            //Debug.Log("Steering:" + steering + " Acceleration:" + acceleration);
            m_Car.Move (steering, acceleration, acceleration, 0f);
            //m_Car.Move(0f, -1f, 1f, 0f);
        }

        private int dir_car = 1;
        private bool can_change_dir = true;
        private void OnCollisionEnter (Collision other) {
            if (can_change_dir == true) {
                dir_car *= -1;
                can_change_dir = false;
            }
            Debug.Log ("Crashed!, " + dir_car);
            //if car is going forward, navigate by reversing else vice versa
        }

        private void OnCollisionExit (Collision other) {
            can_change_dir = true;
        }

        private void setNextTarget () {
            if (currIndex < nodesToGoal.Count - 1)
                currIndex++;

            if (currIndex == nodesToGoal.Count - 1) {
                //  updatePath ();
            }
        }

        private void updatePath () {
            enemies = enemy_planner.remove_destroyed (enemies);
            GameObject target_enemy = enemy_planner.get_closest_object (enemies, transform.position);
            astar.initAstar (transform.position, target_enemy.transform.position);
            // nodesToGoal = pathHandler.getPath (transform.position, target_enemy.transform.position);
            // currIndex = 0;
            //  Debug.Log("enemy pos: " + target_enemy.transform.position + ", my pos: " + transform.position);
        }
    }
}
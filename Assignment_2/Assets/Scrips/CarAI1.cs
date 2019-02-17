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
        public List<GameObject> lineOfSight_enemies = new List<GameObject> ();

        PathHandler pathHandler;
        private List<Vector3> nodesToGoal = new List<Vector3> ();
        private int currIndex = 0;
        private AStar astar;
        private int loadTime = 100;

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
            // updatePath ();
        }

        private void animateAstar () {
            foreach (Spot spot in astar.closedSet) {
                spot.Draw (Color.red);
            }

            foreach (Spot spot in astar.openSet) {
                spot.Draw (Color.green);
            }

            //  DrawPath (nodesToGoal);
        }

        private void DrawPath (List<Vector3> path) {
            for (int i = 0; i < path.Count - 1; i++) {
                Debug.DrawLine (path[i], path[i + 1], Color.red, 100);
            }
        }

        //        List<Spot> path = new List<Spot> ();

        private void FixedUpdate () {
            if (loadTime > 0) {
                loadTime--;
            } else {

                runAstar ();
                //Debug.Log ("executing path..");
                // Execute your path here
                // ...

                //Debug.Log ("current index: " + currIndex);
                //Debug.Log ("nodesToGoal: " + nodesToGoal.Count);
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

                if (current_target == null) {
                    replan ();
                }

                if (go_back) {
                    go_back_routine ();
                } else {
                    m_Car.Move (steering, acceleration, acceleration, 0f);
                }
                Debug.Log ("coll timer: " + coll_timer);
            }
        }

        private bool go_back = false;
        private void go_back_routine () {
            if (timer > 0) {
                timer--;
                m_Car.Move (0.0f, -1.0f, -1.0f, 0.0f);
            } else {
                timer = 100;
                go_back = false;
            }
        }

        private int timer = 100;
        private void OnDrawGizmos () {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine (transform.position, nodesToGoal[currIndex]);
        }

        private void OnCollisionExit (Collision other) {
            coll_timer = 100;
        }

        private int coll_timer = 100;
        private void OnCollisionStay (Collision other) {
            if (coll_timer == 100) {
                if (is_coll_back ()) {
                    go_back = true;
                }
            }

            coll_timer--;
            if (coll_timer <= 0)
                coll_timer = 100;
        }

        private bool is_coll_back () {
            Vector3 offset_forward = transform.forward * 3;
            Vector3 offset_side = transform.right * 2;
            Vector3 left_pos = transform.position + offset_forward + offset_side;
            Vector3 right_pos = transform.position + offset_forward - offset_side;

            int layerMask = LayerMask.GetMask ("CubeWalls");

            if ((Physics.Linecast (transform.position, left_pos, layerMask)) || 
                (Physics.Linecast (transform.position, right_pos, layerMask)))
                {
                return true;
            }
            return false;
        }

        private void replan () {
            Debug.Log ("reset!");
            enemies = enemy_planner.remove_destroyed (enemies);
            currIndex = 0;
            can_run = true;
            can_update = true;
            astar.openSet.Clear ();
        }

        private void setNextTarget () {
            if (currIndex < nodesToGoal.Count - 1)
                currIndex++;
        }

        private void findFurthestTarget () {
            if (nodesToGoal.Count == 0) {
                Debug.Log ("empty path");
                return;
            }

            if (!car_can_see ()) {
                //car has no vision to path
                // replan ();
                //find the closest position we can see, and that the closest node to use can see and add that to our list of nodes
            }
        }

        private bool car_can_see () {
            Vector3 offset = transform.right;
            offset *= 2;
            Vector3 right_pos = transform.position + offset;
            Vector3 left_pos = transform.position - offset;

            for (int i = nodesToGoal.Count - 1; i >= 0; i--) {
                if (can_see (left_pos, nodesToGoal[i]) && can_see (right_pos, nodesToGoal[i])) {
                    currIndex = i;
                    Debug.Log ("new index: " + currIndex);
                    return true;
                }
            }

            return false;
        }

        private void load_lineOfSight () {
            lineOfSight_enemies.Clear ();

            foreach (GameObject enemy in enemies) {
                if (enemy == null) {
                    continue;
                }

                if (can_see (transform.position, enemy.transform.position)) {
                    lineOfSight_enemies.Add (enemy);
                }
            }
        }

        private bool can_see (Vector3 from, Vector3 other_pos) {
            int layer_mask = LayerMask.GetMask ("CubeWalls");
            if (!Physics.Linecast (from, other_pos, layer_mask)) {
                Debug.DrawLine (from, other_pos, Color.green, 5);
                return true;
            }
            return false;
        }

        private bool can_update = true;
        private GameObject current_target;
        private void updatePath () {
            // Debug.Log ("updating path.. " + astar.openSet.Count);
            //enemies = enemy_planner.remove_destroyed (enemies);
            load_lineOfSight ();

            if (lineOfSight_enemies.Count > 0) {
                current_target = enemy_planner.get_closest_object (lineOfSight_enemies, transform.position);
            } else {
                current_target = enemy_planner.get_closest_object (enemies, transform.position);
            }
            Debug.Log ("enemy: " + current_target.transform.position);
            //  nodesToGoal = astar.getPath (transform.position, target_enemy.transform.position);
            if (nodesToGoal == null) {
                Debug.Log ("returned res is null");
                //    updatePath();
            }
            astar.initAstar (transform.position, current_target.transform.position);
            // Debug.Log ("updated astar.. " + astar.openSet.Count);
            currIndex = 0;

            Debug.DrawLine (current_target.transform.position, transform.position, Color.blue, 100);
            //  Debug.Log("enemy pos: " + target_enemy.transform.position + ", my pos: " + transform.position);
            can_update = false;
        }

        private bool can_run = true;
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
                Debug.Log (neighbors.Count);

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
    }
}
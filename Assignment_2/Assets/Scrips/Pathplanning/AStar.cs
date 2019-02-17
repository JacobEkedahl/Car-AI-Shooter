using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar {
    //public GridDiscretization grid {get; set;}
    public VoronoiGraph grid { get; set; }
    public Spot[, ] grid_spots { get; set; }
    public List<Spot> openSet { get; set; }
    public List<Spot> closedSet { get; set; }

    public AStar (GridDiscretization grid) {
        openSet = new List<Spot> ();
        closedSet = new List<Spot> ();

        this.grid = new VoronoiGraph (grid);
        //this.grid.draw();
        this.grid_spots = new Spot[this.grid.x_N, this.grid.z_N];

        for (int i = 0; i < this.grid.x_N; i++) {
            for (int j = 0; j < this.grid.z_N; j++) {
                bool wall = this.grid.grid_distance[i, j] == -1;
                grid_spots[i, j] = new Spot (grid.get_x_pos (i), grid.get_z_pos (j), i, j, wall, this.grid.grid_distance[i, j]);
            }
        }

        for (int i = 0; i < this.grid.x_N; i++) {
            for (int j = 0; j < this.grid.z_N; j++) {
                grid_spots[i, j].addNeighbors (grid_spots);
            }
        }
    }

    private int[] findNonObstacle (int obj_i, int obj_j) {
        int[] result = new int[2];
        int size = 1;
        bool has_not_find_start = true;
        while (has_not_find_start) {
            for (int i = -size; i <= size; i++) {
                for (int j = -size; j <= size; j++) {
                    if (grid.grid_distance[obj_i + i, obj_j + j] != -1) {
                        result[0] = obj_i + i;
                        result[1] = obj_j + j;
                        Debug.Log ("i: " + result[0] + ", j: " + result[1] + grid.grid_distance[result[0], result[1]]);
                        has_not_find_start = false;
                        return result;
                    }
                }
            }
            size += 1;
        }

        return result;
    }

    public Spot start { get; set; }
    public Spot goal { get; set; }
    public void initAstar (Vector3 start_pos, Vector3 goal_pos) {
        clear_grid ();
        openSet = new List<Spot> ();
        closedSet = new List<Spot> ();

        int start_i = grid.get_i_index (start_pos.x);
        int start_j = grid.get_j_index (start_pos.z);

        int goal_i = grid.get_i_index (goal_pos.x);
        int goal_j = grid.get_j_index (goal_pos.z);

        Debug.Log ("i: " + goal_i + ", j: " + goal_j);

        int[] index_start = findNonObstacle (start_i, start_j);
        int[] index_goal = findNonObstacle (goal_i, goal_j);

        start = grid_spots[index_start[0], index_start[1]];
        goal = grid_spots[index_goal[0], index_goal[1]];

        openSet.Add (start);
    }

    private void clear_grid () {
        for (int i = 0; i < grid_spots.GetLength (0); i++) {
            for (int j = 0; j < grid_spots.GetLength (1); j++) {
                grid_spots[i, j].clear ();
            }
        }
    }

    public List<Vector3> getPath (Vector3 start_pos) {
        List<Vector3> path = new List<Vector3> ();
        
        while (openSet.Count > 0) {
            var winner = 0;
            for (int index = 0; index < openSet.Count; index++) {
                if (openSet[index].f < openSet[winner].f) {
                    winner = index;
                }
            }

            var current = openSet[winner];

            //start orig
;
            int dist_to_goal = 3;
            if (grid.grid_distance[goal.i, goal.j] == -1) {
                dist_to_goal = 5;
            }

            //find the path
            if (Vector3.Distance (current.pos, goal.pos) < dist_to_goal) {
                var temp = current;
                path.Add (temp.pos);

                while (temp.previous != null) {
                    path.Add (temp.previous.pos);
                    temp = temp.previous;
                }

                int size_of_path = path.Count;
                int modVal = 5;

                if (size_of_path > 20) {
                    modVal = 10;
                } else if (size_of_path < 6) {
                    modVal = 2;
                }

                for (int node_i = size_of_path - 1; node_i >= 0; node_i--) {
                    if (node_i % modVal != 0) {
                        path.RemoveAt (node_i);
                    }
                }

                path.Reverse ();
                return path;
            }

            //find the path
            if (current == goal) {
                var temp = current;
                path.Add (temp.pos);

                while (temp.previous != null) {
                    path.Add (temp.previous.pos);
                    temp = temp.previous;
                }
                return path;
            }

            openSet.Remove (current);
            closedSet.Add (current);

            var neighbors = current.neighbors;

            for (int k = 0; k < neighbors.Count; k++) {
                var neighbor = neighbors[k];
                if (!closedSet.Contains (neighbor) && !neighbor.wall) {
                    var tempG = current.g + 1 * getSplit ();

                    if (openSet.Contains (neighbor)) {
                        if (tempG < neighbor.g) {
                            neighbor.g = tempG;
                        }
                    } else {
                        neighbor.g = tempG;
                        openSet.Add (neighbor);
                    }

                    neighbor.h = heuristic (current.previous, current, neighbor, goal);
                    neighbor.f = neighbor.g + neighbor.h;
                    neighbor.previous = current;
                }
            }
        }

        return null;
    }

    private float dist (Spot a, Spot b) {
        float iDist = Mathf.Abs (a.i - b.i);
        float jDist = Mathf.Abs (a.j - b.j);
        return Mathf.Sqrt ((iDist * iDist) + (jDist * jDist));
    }

    public float getSplit () {
        return grid.splits * 0.1f;
    }

    public float heuristic (Spot prev, Spot from, Spot neighbor, Spot end) {
        float dist_n_goal = dist (neighbor, end);
        int changedDir = 1;

        /*        if (prev != null) {
                    float dist_prev_from = dist(prev, from);
                    float dist_from_n = dist(from, neighbor);

                    if (dist_prev_from != dist_from_n) {
                        changedDir = 2;
                    }
                }
         */
        //  Debug.Log ("value neighbor: " + neighbor.value);
        return dist_n_goal * (neighbor.value);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar {
    public VoronoiGraph grid { get; set; }
    public SpotRealTime[,] grid_spots { get; set; }
    public List<SpotRealTime> openSet { get; set; }
    public List<SpotRealTime> closedSet { get; set; }

    private Boolean turretTarget;
    public Vector3 start_pos;

    public AStar(GridDiscretization grid, Boolean turretTarget) {
        //Debug.Log("grid: " + grid.toString());
        this.turretTarget = turretTarget;
        openSet = new List<SpotRealTime>();
        closedSet = new List<SpotRealTime>();

        this.grid = new VoronoiGraph(grid);
        //this.grid.draw();
        this.grid_spots = new SpotRealTime[this.grid.x_N, this.grid.z_N];

        for (int i = 0; i < this.grid.x_N; i++) {
            for (int j = 0; j < this.grid.z_N; j++) {
                bool wall = this.grid.grid_distance[i, j] == -1;
                grid_spots[i, j] = new SpotRealTime(grid.get_x_pos(i), grid.get_z_pos(j), i, j, wall, this.grid.grid_distance[i, j]);
            }
        }

        for (int i = 0; i < this.grid.x_N; i++) {
            for (int j = 0; j < this.grid.z_N; j++) {
                grid_spots[i, j].addNeighbors(grid_spots);
            }
        }
    }

    private int[] findNonObstacle(int obj_i, int obj_j) {
        int[] result = new int[2];
        int size = 1;
        bool has_not_find_start = true;
        while (has_not_find_start) {
            for (int i = -size; i <= size; i++) {
                for (int j = -size; j <= size; j++) {
                    if (grid.grid_distance[obj_i + i, obj_j + j] != -1) {
                        result[0] = obj_i + i;
                        result[1] = obj_j + j;
                        // Debug.Log ("i: " + result[0] + ", j: " + result[1] + grid.grid_distance[result[0], result[1]]);
                        has_not_find_start = false;
                        return result;
                    }
                }
            }
            size += 1;
        }

        return result;
    }

    public SpotRealTime start { get; set; }
    public SpotRealTime goal { get; set; }
    public void initAstar(Vector3 start_pos, Vector3 goal_pos) {
        this.start_pos = start_pos;
        clear_grid();
        openSet = new List<SpotRealTime>();
        closedSet = new List<SpotRealTime>();

        int start_i = grid.get_i_index(start_pos.x);
        int start_j = grid.get_j_index(start_pos.z);

        int goal_i = grid.get_i_index(goal_pos.x);
        int goal_j = grid.get_j_index(goal_pos.z);

        //  Debug.Log ("i: " + goal_i + ", j: " + goal_j);

        if (turretTarget) {
            int[] index_start = findNonObstacle(start_i, start_j);
            int[] index_goal = findNonObstacle(goal_i, goal_j);

            start = grid_spots[index_start[0], index_start[1]];
            goal = grid_spots[index_goal[0], index_goal[1]];
        } else {
            start = grid_spots[start_i, start_j];
            goal = grid_spots[goal_i, goal_j];
        }
        openSet.Add(start);
    }

    private void clear_grid() {
        for (int i = 0; i < grid_spots.GetLength(0); i++) {
            for (int j = 0; j < grid_spots.GetLength(1); j++) {
                grid_spots[i, j].clear();
            }
        }
    }

    public float getDistance(Vector3 start_node, Vector3 goal_node) {
        initAstar(start_node, goal_node);
        List<Vector3> path = getPath();
        path = reconstructPath(path);
        return dist_astar(path);
    }

    public float dist_astar(List<Vector3> path) {
        float result = 0.0f;

        for (int i = 0; i < path.Count - 1; i++) {
            result += Vector3.Distance(path[i], path[i + 1]);
         //   Debug.DrawLine(path[i], path[i + 1], Color.white, 50);
        }

        return result;
    }

    public List<Vector3> reconstructPath(List<Vector3> path) {
        List<Vector3> result = new List<Vector3>();
        //Debug.Log("path: " + path.Count);
        Vector3 start = path[0];
        Vector3 furthest = findFurthest(start, start, path);
        //Debug.Log("is furthest start: " + (start == furthest));
        result = navigateToTarget(start, furthest, path);
        return result;
    }

    private Vector3 findFurthest(Vector3 from, Vector3 currentFurthest, List<Vector3> path) {
        int indexFurthest = path.IndexOf(currentFurthest);

        for (int i = path.Count - 1; i >= indexFurthest; i--) {
            if (canSee(from, path[i])) {
                return path[i];
            }
        }

        return currentFurthest;
    }

    private List<Vector3> navigateToTarget(Vector3 from, Vector3 to, List<Vector3> path) {
        //lerp from, to. If one see a target further then add the current vector to result and repeat.
        List<Vector3> result = new List<Vector3>();
        float distance = Vector3.Distance(from, to);
        result.Add(from);

        float fracJourney = 0.0f;

        while (fracJourney <= 1) {
            Vector3 newPos = Vector3.Lerp(from, to, fracJourney);
            Vector3 furthest = findFurthest(newPos, to, path);

            if (furthest != to) {
                from = newPos;
                to = furthest;
                distance = Vector3.Distance(from, to);
                result.Add(from);
             //   Debug.DrawLine(from, to, Color.blue, 50);
                fracJourney = 0.0f;
            }
            fracJourney += (2 / distance);
        }

        result.Add(path[path.Count - 1]);
        return result;
    }

    private bool canSee(Vector3 from, Vector3 to) {
       // Debug.DrawLine(from, to, Color.blue);
        float dist = Vector3.Distance(from, to);
        float currPos = 0;
        float fracJourney = 0;

        while (fracJourney < 1) {
            // Set our position as a fraction of the distance between the markers.
            Vector3 newPos = Vector3.Lerp(from, to, fracJourney);
            if (!isInGrid(newPos)) {
                return false;
            }
            currPos += 10;
            fracJourney = currPos / dist;
        }

        return true;
    }

    private bool isInGrid(Vector3 pos) {
        float x = pos.x;
        float z = pos.z;
        int i = grid.get_i_index(x);
        int j = grid.get_i_index(z);
        //  Debug.Log("x, z:" + x + ":" + z);

        //wall
        if (grid.grid_distance[i, j] == -1) {
            return false;
        }

        return true;
    }

    public float getAngleStart(Vector3 end, Vector3 dir) {
        return getAngle(start.pos, end, dir);
    }

    public float getAngleEnd(Vector3 start, Vector3 dir) {
        return getAngle(start, goal.pos, dir);
    }

    private float getAngle(Vector3 from, Vector3 to, Vector3 dir) {
        Vector3 targetDir = to - from;
       // Debug.DrawLine(from, to, Color.red, 80);
        return Vector3.SignedAngle(targetDir, dir, Vector3.down);
    }

    public List<Vector3> getPath() {
        List<Vector3> path = new List<Vector3>();

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
            if (Vector3.Distance(current.pos, goal.pos) < dist_to_goal) {
                var temp = current;
                path.Add(temp.pos);

                while (temp.previous != null) {
                    path.Add(temp.previous.pos);
                    temp = temp.previous;
                }

                int size_of_path = path.Count;
                int modVal = 5;

                if (size_of_path > 20) {
                    modVal = 5;
                } else if (size_of_path < 6) {
                    modVal = 2;
                }

                for (int node_i = size_of_path - 1; node_i >= 0; node_i--) {
                    if (node_i % modVal != 0) {
                        path.RemoveAt(node_i);
                    }
                }

                path.Reverse();
                return path;
            }

            //find the path
            if (current == goal) {
                var temp = current;
                path.Add(temp.pos);

                while (temp.previous != null) {
                    path.Add(temp.previous.pos);
                    temp = temp.previous;
                }
                return path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            var neighbors = current.neighbors;

            for (int k = 0; k < neighbors.Count; k++) {
                var neighbor = neighbors[k];
                if (!closedSet.Contains(neighbor) && !neighbor.wall) {
                    var tempG = current.g + 1 * getSplit();

                    if (openSet.Contains(neighbor)) {
                        if (tempG < neighbor.g) {
                            neighbor.g = tempG;
                        }
                    } else {
                        neighbor.g = tempG;
                        openSet.Add(neighbor);
                    }

                    neighbor.h = heuristic(current.previous, current, neighbor, goal);

                    neighbor.f = neighbor.g + neighbor.h;
                    neighbor.previous = current;
                }
            }
        }

        return null;
    }

    private float dist(SpotRealTime a, SpotRealTime b) {
        float iDist = Mathf.Abs(a.i - b.i);
        float jDist = Mathf.Abs(a.j - b.j);
        return Mathf.Sqrt((iDist * iDist) + (jDist * jDist));
    }

    public float getSplit() {
        return grid.splits * 0.1f;
    }

    public float heuristic_nonWall(SpotRealTime neighbor, SpotRealTime end) {
        return dist(neighbor, end);
    }

    public float heuristic(SpotRealTime prev, SpotRealTime from, SpotRealTime neighbor, SpotRealTime end) {
        float dist_n_goal = dist(neighbor, end);
        return dist_n_goal * (neighbor.value);
    }

    public override bool Equals(object obj) {
        var star = obj as AStar;
        return star != null &&
               EqualityComparer<VoronoiGraph>.Default.Equals(grid, star.grid) &&
               EqualityComparer<SpotRealTime[,]>.Default.Equals(grid_spots, star.grid_spots) &&
               EqualityComparer<List<SpotRealTime>>.Default.Equals(openSet, star.openSet) &&
               EqualityComparer<List<SpotRealTime>>.Default.Equals(closedSet, star.closedSet) &&
               turretTarget == star.turretTarget &&
               start_pos.Equals(star.start_pos) &&
               EqualityComparer<SpotRealTime>.Default.Equals(start, star.start) &&
               EqualityComparer<SpotRealTime>.Default.Equals(goal, star.goal);
    }

    public override int GetHashCode() {
        var hashCode = -460371681;
        hashCode = hashCode * -1521134295 + EqualityComparer<VoronoiGraph>.Default.GetHashCode(grid);
        hashCode = hashCode * -1521134295 + EqualityComparer<SpotRealTime[,]>.Default.GetHashCode(grid_spots);
        hashCode = hashCode * -1521134295 + EqualityComparer<List<SpotRealTime>>.Default.GetHashCode(openSet);
        hashCode = hashCode * -1521134295 + EqualityComparer<List<SpotRealTime>>.Default.GetHashCode(closedSet);
        hashCode = hashCode * -1521134295 + turretTarget.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(start_pos);
        hashCode = hashCode * -1521134295 + EqualityComparer<SpotRealTime>.Default.GetHashCode(start);
        hashCode = hashCode * -1521134295 + EqualityComparer<SpotRealTime>.Default.GetHashCode(goal);
        return hashCode;
    }

    public override string ToString() {
        return base.ToString();
    }
}
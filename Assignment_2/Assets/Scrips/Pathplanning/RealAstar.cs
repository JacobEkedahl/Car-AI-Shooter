using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealAstar
{ 
    public SpotRealTime[,] grid_spots { get; set; }
    public List<SpotRealTime> openSet { get; set; }
    public List<SpotRealTime> closedSet { get; set; }
    private GridDiscretization grid;

    public RealAstar(GridDiscretization grid)
    {
        openSet = new List<SpotRealTime>();
        closedSet = new List<SpotRealTime>();
        this.grid = grid;
        this.grid_spots = new SpotRealTime[this.grid.x_N, this.grid.z_N];

        for (int i = 0; i < this.grid.x_N; i++)
        {
            for (int j = 0; j < this.grid.z_N; j++)
            {
                bool wall = this.grid.discretized_traversibility[i, j] == 1;
                grid_spots[i, j] = new SpotRealTime(grid.get_x_pos(i), grid.get_z_pos(j), i, j, wall);
            }
        }

        for (int i = 0; i < this.grid.x_N; i++)
        {
            for (int j = 0; j < this.grid.z_N; j++)
            {
                grid_spots[i, j].addNeighbors(grid_spots);
            }
        }
    }

    private void clear_grid()
    {
        for (int i = 0; i < grid_spots.GetLength(0); i++)
        {
            for (int j = 0; j < grid_spots.GetLength(1); j++)
            {
                grid_spots[i, j].clear();
            }
        }
    }

    public SpotRealTime start { get; set; }
    public SpotRealTime goal { get; set; }
    public void initAstar(Vector3 start_pos, Vector3 goal_pos)
    {
        clear_grid();
        openSet = new List<SpotRealTime>();
        closedSet = new List<SpotRealTime>();
        int start_i = grid.get_i_index(start_pos.x);
        int start_j = grid.get_j_index(start_pos.z);

        int goal_i = grid.get_i_index(goal_pos.x);
        int goal_j = grid.get_j_index(goal_pos.z);

        start = grid_spots[start_i, start_j];
        goal = grid_spots[goal_i, goal_j];

        openSet.Add(start);
    }

    public float heuristic(SpotRealTime neighbor, SpotRealTime end)
    {
        float iDist = Mathf.Abs(neighbor.i - end.i);
        float jDist = Mathf.Abs(neighbor.j - end.j);
        var d = Mathf.Sqrt((iDist * iDist) + (jDist * jDist));
        
        return d * neighbor.value;
    }

    public float dist_astar(List<Vector3> path)
    {
        float result = 0.0f;
        List<Vector3> path_to_goal = getPath();

        for (int i = 0; i < path_to_goal.Count - 1; i++)
        {
            result += Vector3.Distance(path_to_goal[i], path_to_goal[i + 1]);
            Debug.DrawLine(path_to_goal[i], path_to_goal[i + 1], Color.white, 50);
        }

        return result;
    }

    public float getAngle(List<Vector3> path, Vector3 dir) {
        int index = 0;
        for (int i = 0; i < path.Count; i++) {
            if (Vector3.Distance(path[i], start.pos) > 0) {
                index = i;
                break;
            }
        }
        
        Vector3 targetDir = path[index] - start.pos;
        Debug.DrawLine(start.pos, path[index], Color.red, 80);
        return Vector3.SignedAngle(targetDir, dir, Vector3.down);
    }

    //comments are copied from wikipedia
    public List<Vector3> getPath()
    {
        List<Vector3> path = new List<Vector3>();

        while (openSet.Count > 0)
        {
            var winner = 0;
            for (int index = 0; index < openSet.Count; index++)
            {
                if (openSet[index].f < openSet[winner].f)
                {
                    winner = index;
                }
            }

            var current = openSet[winner];

            //start orig
            ;
            int dist_to_goal = 8;
            if (grid.discretized_traversibility[goal.i, goal.j] == 1) //is wall
            {
                dist_to_goal = 5;
            }

            //find the path
            if (Vector3.Distance(current.pos, goal.pos) < dist_to_goal)
            {
                //we have reached the goal
                //Debug.Log("reached goal!");
                var temp = current;
                path.Add(temp.pos);

                while (temp.previous != null)
                {
                    path.Add(temp.previous.pos);
                    temp = temp.previous;
                }

                int size_of_path = path.Count;

                int modVal = 1;



                if (size_of_path > 20)
                {
                    modVal = 4;
                } 
                else if (size_of_path > 10)
                {
                    modVal = 2;
                } else if (size_of_path > 6) {
                    modVal = 2;
                }

                for (int node_i = size_of_path - 1; node_i >= 0; node_i--)
                {
                    if (node_i % modVal != 0)
                    {
                        path.RemoveAt(node_i);
                    }
                }

                path.Reverse();
                return path;
            }

            //find the path
            if (Vector3.Distance(current.pos, goal.pos) < dist_to_goal)
            {
                var temp = current;
                path.Add(temp.pos);

                while (temp.previous != null)
                {
                    path.Add(temp.previous.pos);
                    temp = temp.previous;
                }
                return path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            var neighbors = current.neighbors;

            for (int k = 0; k < neighbors.Count; k++)
            {
                var neighbor = neighbors[k];
                if (!closedSet.Contains(neighbor) && !neighbor.wall)
                {
                    var tempG = current.g + 1;

                    if (openSet.Contains(neighbor))
                    {
                        if (tempG < neighbor.g)
                        {
                            neighbor.g = tempG;
                        }
                    }
                    else
                    {
                        neighbor.g = tempG;
                        openSet.Add(neighbor);
                    }
                    neighbor.h = heuristic(neighbor, goal);
                    neighbor.f = neighbor.g + neighbor.h;
                    neighbor.previous = current;
                }
            }
        }

        return null;
    }
}

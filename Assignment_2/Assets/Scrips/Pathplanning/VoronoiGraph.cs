using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoronoiGraph {
    public float[, ] grid_distance { get; private set; }

    private float x_high;
    private float x_low;
    public int x_N;
    public float x_step { get; private set; }

    private float z_high;
    private float z_low;
    public int z_N;
    public float z_step { get; private set; }
    public int max_number { get; }

    public Vector3 start_pos { get; set; }
    public Vector3 goal_pos { get; set; }

    public int start_i { get; set; }
    public int start_j { get; set; }
    public int goal_i { get; set; }
    public int goal_j { get; set; }
    public GridDiscretization grid;


    public VoronoiGraph (GridDiscretization grid) {
        this.grid = grid;
        this.grid_distance = grid.discretized_traversibility;
        x_N = grid.x_N;
        z_N = grid.z_N;
        this.max_number = fill_grid();
    }

    public void setStart(Vector3 start) {
        start_pos = start;
        start_i = grid.get_i_index(start.x);
        start_j = grid.get_j_index(start.z);
    }

    public void setGoal(Vector3 goal) {
        goal_pos = goal;
        goal_i = grid.get_i_index(goal.x);
        goal_j = grid.get_j_index(goal.z);
    }

    public List<Vector2> fill_voronoi_attempt2() {
        List<Vector2> boundary = new List<Vector2>();
        int currentIteration = 1;
        bool isDone = false;

        while (!isDone) {
            isDone = true;
            // check grid below
            for (int i = 0; i < x_N - 1; i++) {
                for (int j = 0; j < z_N; j++) {
                    if (grid_distance[i + 1, j] == currentIteration) {
                        if (grid_distance[i, j] == currentIteration + 1) {
                            boundary.Add(new Vector2(i, j));
                        } else if (grid_distance[i, j] == 0) {
                            isDone = false;
                            grid_distance[i, j] = currentIteration + 1;
                        }
                    }
                }
            }

            // check grid above
            for (int i = x_N - 1; i > 0; i--) {
                for (int j = 0; j < z_N; j++) {
                    if (grid_distance[i - 1, j] == currentIteration) {
                        if (grid_distance[i, j] == currentIteration + 1) {
                            boundary.Add(new Vector2(i, j));
                        } else if (grid_distance[i, j] == 0) {
                            isDone = false;
                            grid_distance[i, j] = currentIteration + 1;
                        }
                    }
                }
            }

            // check grid right
            for (int i = 0; i < this.x_N; i++) {
                for (int j = 0; j < z_N - 1; j++) {
                    if (grid_distance[i, j + 1] == currentIteration) {
                        if (grid_distance[i, j] == currentIteration + 1) {
                            boundary.Add(new Vector2(i, j));
                        } else if (grid_distance[i, j] == 0) {
                            isDone = false;
                            grid_distance[i, j] = currentIteration + 1;
                        }
                    }
                }
            }

            // check grid left
            for (int i = 0; i < x_N; i++) {
                for (int j = z_N - 1; j < 0; j--) {
                    if (grid_distance[i, j - 1] == currentIteration) {
                        if (grid_distance[i, j] == currentIteration + 1) {
                            boundary.Add(new Vector2(i, j));
                        } else if (grid_distance[i, j] == 0) {
                            isDone = false;
                            grid_distance[i, j] = currentIteration + 1;
                        }
                    }
                }
            }
            currentIteration++;
        }
        return boundary;
    }

    public List<Vector2> fill_voronoi_attempt1() {
        List<Vector2> boundary = new List<Vector2>();
        int currentIteration = 1;
        bool isDone = false;
        while (!isDone) {
            bool[,] marked = new bool[x_N, z_N];
            isDone = true;
            for (int i = 0; i < x_N; i++) {
                for (int j = 0; j < z_N; j++) {
                    if (grid_distance[i, j] == currentIteration) {
                        List<Point> neighbours = GetNeighbours(i, j);
                        foreach(Point p in neighbours) {
                            if (grid_distance[p.i, p.j] == 0) {
                                isDone = false;
                                grid_distance[p.i, p.j] = currentIteration + 1;
                                marked[p.i, p.j] = true;
                            } else if (marked[p.i, p.j]) {
                                isDone = false;
                                Vector2 idx = new Vector2(p.i, p.j);
                                boundary.Add(idx);
                            }
                        }
                    }
                }
            }
            currentIteration++;
        }
        return boundary;
    }

    public void draw_boundary() {
        List<Vector2> boundary = fill_voronoi_attempt2();
        //List<Vector2> boundary = fill_voronoid();
        for(int i = 0; i < boundary.Count - 1; i++) {
            Debug.Log(boundary[i]);
            Vector3 pos = new Vector3(grid.get_x_pos((int)boundary[i].x), 0, grid.get_z_pos((int)boundary[i].y));
            Debug.DrawLine( pos, new Vector3(pos.x - 2, pos.y, pos.z + 2), Color.yellow, 1000);
        }
    }

    public int fill_grid () {
        bool isDone = false;
        int currentIteration = 1;

        while (!isDone) {
            isDone = true;
            //loop through grid and if my neighbour is 0 set its value to my value +1
            for (int i = 0; i < x_N; i++) {
                for (int j = 0; j < z_N; j++) {
                    if (grid_distance[i, j] == 0) {
                        isDone = false;
                        List<Point> neighbours = GetNeighbours(i, j);
                        foreach(Point p in neighbours) {
                            if (grid_distance[p.i, p.j] == currentIteration) {
                                grid_distance[i, j] = currentIteration + 1;
                            }
                        }
                    }
 /*                   List<Point> neighbours = GetNeighbours (i, j);
                    float myValue = grid_distance[i, j];
                    if (myValue != currentIteration) { //skip this one
                        foreach (Point p in neighbours) {
                            if (grid_distance[p.i, p.j] == 0) {
                                isDone = false;
                                grid_distance[p.i,p.j] = currentIteration + 1;
                            }
                        }
                    }
                    */
                }
            }
            currentIteration++;
        }
        return currentIteration;
    }

    public List<Point> GetNeighbours (int i, int j) {
        List<Point> neighbours = new List<Point> ();
        if (i > 0) {
            neighbours.Add (new Point (i - 1, j)); //west
            /*
            if (j > 0)
                neighbours.Add (new Point (i - 1, j - 1)); //northwest
            if (j < z_N - 1)
                neighbours.Add (new Point (i - 1, j + 1)); //southwest
                */
        }

        if (i < z_N - 1) {
            neighbours.Add (new Point (i + 1, j)); //east
            /*
            if (j > 0)
                neighbours.Add (new Point (i + 1, j - 1)); //northeast
            if (j < z_N - 1)
                neighbours.Add (new Point (i + 1, j + 1)); //southeast
                */
        }

        if (j > 0) {
            neighbours.Add (new Point (i, j - 1)); //north
        } else if (j < z_N - 1) {
            neighbours.Add (new Point (i, j + 1)); //south
        }

        return neighbours;
    }

    public void draw() {
        for (int i = 0; i < x_N; i++) {
            for (int j = 0; j < z_N; j++) {
                float val = grid_distance[i, j];
                Debug.Log("val: " + val);
                Color col = Color.Lerp(Color.red, Color.blue, val/this.max_number);
                Vector3 pos = new Vector3(grid.get_x_pos(i), 10.0f, grid.get_z_pos(j));
                Debug.DrawLine(pos, new Vector3(pos.x +2, pos.y, pos.z), col, 10000);
                Debug.DrawLine(pos, new Vector3(pos.x+1, pos.y, pos.z - 1), col, 10000);
            }
        }
    }

    public String toString () { return ""; }
}


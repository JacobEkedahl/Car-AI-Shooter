using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoronoiGraph {
    public float[, ] grid_distance { get; private set; }

    private float x_high { get; set; }
    private float x_low { get; set; }
    public int x_N { get; set; }
    public float x_step { get; set; }

    private float z_high { get; set; }
    private float z_low { get; set; }
    public int z_N { get; set; }
    public float z_step { get; set; }
    public int max_number { get; }

    public Vector3 start_pos { get; set; }
    public Vector3 goal_pos { get; set; }

    public int start_i { get; set; }
    public int start_j { get; set; }
    public int goal_i { get; set; }
    public int goal_j { get; set; }
    public GridDiscretization grid;
    public int splits{get; set;}

    public VoronoiGraph (GridDiscretization grid) {
        this.grid = grid;
        this.grid_distance = grid.discretized_traversibility;
        this.x_low = grid.x_low;
        this.z_low = grid.z_low;
        x_N = grid.x_N;
        z_N = grid.z_N;
        this.max_number = fill_grid ();


        //invert grid distance
        int divisions = 2;
        splits = max_number / divisions;
        for (int i = 0; i < x_N; i++) {
            for (int j = 0; j < z_N; j++) {
                if (grid.discretized_traversibility[i, j] == 1) {
                    grid_distance[i, j] = -1;
                } else {
                    grid_distance[i, j] = (((max_number - grid_distance[i, j]) % splits) * 0.1f) + 1;
                }
            }
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
                        List<Point> neighbours = GetNeighbours (i, j);
                        foreach (Point p in neighbours) {
                            if (grid_distance[p.i, p.j] == currentIteration) {
                                grid_distance[i, j] = currentIteration + 1;
                            }
                        }
                    }
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
        }
        if (i < z_N - 1) {
            neighbours.Add (new Point (i + 1, j)); //east
        }

        if (j > 0) {
            neighbours.Add (new Point (i, j - 1)); //north
        }

        if (j < z_N - 1) {
            neighbours.Add (new Point (i, j + 1)); //south
        }

        return neighbours;
    }

    public void draw () {
        for (int i = 0; i < x_N; i++) {
            for (int j = 0; j < z_N; j++) {
                float val = grid_distance[i, j];
                Color col = Color.Lerp (Color.red, Color.blue, val / this.max_number);
                Vector3 pos = new Vector3 (grid.get_x_pos (i), 10.0f, grid.get_z_pos (j));
                Debug.DrawLine (pos, new Vector3 (pos.x + 2, pos.y, pos.z), col, 10000);
                Debug.DrawLine (pos, new Vector3 (pos.x + 1, pos.y, pos.z - 1), col, 10000);
            }
        }
    }

    public float get_x_pos (int i) {
        float xPos = x_low + (i * x_step);
        return xPos;
    }

    public float get_z_pos (int j) {
        float zPos = z_low + (j * z_step);
        return zPos;
    }

    public int get_i_index (float x) {
        return (int) (x - x_low);
    }

    public int get_j_index (float z) {
        return (int) (z - z_low);
    }

    public int get_index_as_1d_array (int i, int j) {
        return i * this.x_N + j;
    }

    public String toString () { return ""; }
}
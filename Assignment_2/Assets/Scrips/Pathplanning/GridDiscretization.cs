using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GridDiscretization {
    public float[, ] discretized_traversibility { get; private set; }

    public float x_high;
    public float x_low;
    public int x_N;
    public float x_step { get; private set; }

    public float z_high;
    public float z_low;
    public int z_N;
    public float z_step { get; private set; }

    //TODO: Add function parameter - step size
    public GridDiscretization (TerrainInfo info, int width, int height, int obstacleExpansion) {
        x_high = info.x_high;
        x_low = info.x_low;
        //x_N = (int) (Mathf.Ceil(x_high - x_low) / width);
        //x_N = info.x_N;
        x_N = (int) Mathf.Ceil (x_high - x_low) / width;
        x_step = width; //(x_high - x_low) / x_N;

        z_high = info.z_high;
        z_low = info.z_low;
        //x_N = (int)(Mathf.Ceil(x_high - x_low) / height);
        //z_N = info.z_N;
        z_N = (int) Mathf.Ceil (z_high - z_low) / height;
        z_step = height; //(z_high - z_low) / z_N;

        //Debug.Log ("x_high: " + x_high + ", x_low: " + x_low + ", x_N: " + x_N +
            //", x_step: " + x_step + ", z_high: " + z_high + ", z_step: " + z_step);

        if (x_N <= info.x_N && z_N <= info.z_N) {
            // Copy the traversibility info if the original grid size <= 1
            copy_traversibility (info);

        } else {
            // Discretize traversibility info if the original grid size > 1
            construct_discretized_traversibility (info);
        }

        for (int i = 0; i < obstacleExpansion; i++) {
            expand_obstacles ();
        }
    }

    private void expand_obstacles () {
        // If the grid below current grid is an obstacle, set this grid to an obstacle
        for (int i = 0; i < this.x_N - 1; i++) {
            for (int j = 0; j < this.z_N; j++) {
                if (this.discretized_traversibility[i + 1, j] == 1) {
                    this.discretized_traversibility[i, j] = 1;
                }
            }
        }

        // If the grid above current grid is an obstacle, set this grid to an obstacle
        for (int i = this.x_N - 1; i > 0; i--) {
            for (int j = 0; j < this.z_N; j++) {
                if (this.discretized_traversibility[i - 1, j] == 1) {
                    this.discretized_traversibility[i, j] = 1;
                }
            }
        }

        // If the grid to the right of current grid is an obstacle, set this grid to an obstacle
        for (int i = 0; i < this.x_N; i++) {
            for (int j = 0; j < this.z_N - 1; j++) {
                if (this.discretized_traversibility[i, j + 1] == 1) {
                    this.discretized_traversibility[i, j] = 1;
                }
            }
        }

        // If the grid to the left of current grid is an obstacle, set this grid to an obstacle
        for (int i = 0; i < this.x_N; i++) {
            for (int j = this.z_N - 1; j > 0; j--) {
                if (this.discretized_traversibility[i, j - 1] == 1) {
                    this.discretized_traversibility[i, j] = 1;
                }
            }
        }
    }

    private void copy_traversibility (TerrainInfo info) {
        this.x_N = info.x_N;
        this.x_step = (x_high - x_low) / this.x_N;

        this.z_N = info.z_N;
        this.z_step = (z_high - z_low) / this.z_N;
        this.discretized_traversibility = info.traversability;
    }

    private void construct_discretized_traversibility (TerrainInfo info) {
        discretized_traversibility = new float[x_N, z_N];
        for (int i = 0; i < x_N; i++) {
            for (int j = 0; j < z_N; j++) {
                float x_center = get_x_pos (i);
                float z_center = get_z_pos (j);

                if (info.traversability[info.get_i_index (x_center), info.get_j_index (z_center)] == 1) {
                    discretized_traversibility[i, j] = 1;
                } else {
                    discretized_traversibility[i, j] = 0;
                }
            }
        }
    }

    public float get_x_pos (int i) {
        float xPos = x_low + (i * x_step);
        //    Debug.Log("from grid: " + i+  ", xPos" + xPos);
        return xPos;
        //   return x_low + x_step / 2 + x_step * i;
    }

    public float get_z_pos (int j) {
        float zPos = z_low + (j * z_step);
        return zPos;
        //  return z_low + z_step / 2 + z_step * j;
    }

    public int get_i_index (float x) {
        //return (int)(x - x_low) / x_step;
        
        int index = (int) Mathf.Floor ((x - x_low) / x_step);
        if (index < 0) {
            index = 0;
        } else if (index > x_N - 1) {
            index = x_N - 1;
        }
        return index;
        
    }

    public int get_j_index (float z) {
        //return (int) (z - z_low);
        
        int index = (int) Mathf.Floor ((z - z_low) / z_step);
        if (index < 0) {
            index = 0;
        } else if (index > z_N - 1) {
            index = z_N - 1;
        }
        return index;
        
    }

    public int get_index_as_1d_array (int i, int j) {
        return i * this.x_N + j;
    }

    public string toString() {
        string res = "";

        for (int i = 0; i < this.x_N; i++)
        {
            for (int j = this.z_N - 1; j > 0; j--)
            {
                res += this.discretized_traversibility[i, j];
            }

            res += "\n";
        }

        return res;
    }
}
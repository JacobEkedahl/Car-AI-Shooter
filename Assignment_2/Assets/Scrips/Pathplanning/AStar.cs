using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar {
    //public GridDiscretization grid {get; set;}
    public VoronoiGraph grid {get; set;}
    public Spot[, ] grid_spots {get; set;}
    public List<Spot> openSet {get; set;}
    public List<Spot> closedSet {get; set;}

    public AStar (GridDiscretization grid) {
        openSet = new List<Spot>();
        closedSet = new List<Spot>();
        this.grid = new VoronoiGraph(grid);
        //this.grid.draw();
        this.grid_spots = new Spot[this.grid.x_N, this.grid.z_N];

        for (int i = 0; i < this.grid.x_N; i++) {
            for (int j = 0; j < this.grid.z_N; j++) {
                bool wall = this.grid.grid_distance[i, j] == -1;
                grid_spots[i, j] = new Spot (grid.get_x_pos (i), grid.get_z_pos (j), i, j, wall, this.grid.grid_distance[i,j]);
            }
        }

        for (int i = 0; i < this.grid.x_N; i++) {
            for (int j = 0; j < this.grid.z_N; j++) {
                grid_spots[i,j].addNeighbors(grid_spots);
            }
        }
    }

    public Spot start {get; set;}
    public Spot goal {get; set;}
    public void initAstar (Vector3 start_pos, Vector3 goal_pos) {
        int start_i = grid.get_i_index (start_pos.x);
        int start_j = grid.get_j_index (start_pos.z);

        int goal_i = grid.get_i_index (goal_pos.x);
        int goal_j = grid.get_j_index (goal_pos.z);

        Debug.Log("i: " + goal_i +", j: " + goal_j);

        start = grid_spots[start_i, start_j];
        goal = grid_spots[goal_i, goal_j];

        openSet.Add (start);
    }

    public float heuristic(Spot neighbor, Spot end) {
        float iDist = Mathf.Abs(neighbor.i - end.i);
        float jDist = Mathf.Abs(neighbor.j - end.j);
        var d = Mathf.Sqrt((iDist * iDist) + (jDist * jDist));
        
        Debug.Log("value neighbor: " + neighbor.value);
        return d * neighbor.value;
    }

    //comments are copied from wikipedia
    public List<Vector3> getPath (Vector3 start_pos, Vector3 goal_pos) {

        //  while (openSet.Count > 0) {
        //we can keep going

        //  }

        //no solution
        return null;
    }
}
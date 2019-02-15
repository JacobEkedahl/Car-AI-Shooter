using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class PathHandler {
    TerrainManager terrain_manager;
    GridDiscretization grid;
    VoronoiGraph vg;
    VoronoiAStar planner;

    public PathHandler (TerrainManager terrain_manager) {
        this.terrain_manager = terrain_manager;
        this.grid = new GridDiscretization (terrain_manager.myInfo);
    }

    public List<Vector3> getPath (Vector3 from, Vector3 to) {
        vg = new VoronoiGraph (grid);
        vg.setStart(from);
        vg.setGoal(to);
        planner = new VoronoiAStar (vg);
        List<Vector3> nodes;
        List<Vector3> goal_nodes = new List<Vector3> ();
        nodes = planner.a_star ();

        Vector3 prev = Vector3.zero; 
        //removes first node where we are and reverses the list
        for (int i = nodes.Count-2; i >= 0; i--) {
            Vector2 n = nodes[i];
            Vector3 n_vec = new Vector3 (grid.get_x_pos ((int) n.x), 0, grid.get_z_pos ((int) n.y));
            goal_nodes.Add (n_vec);

            if (prev != Vector3.zero)
                Debug.DrawLine(prev, n_vec, Color.red, 1000);

            prev = n_vec;
        }

        return goal_nodes;
    }
}
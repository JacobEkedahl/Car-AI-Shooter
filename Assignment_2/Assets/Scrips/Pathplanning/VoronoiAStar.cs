using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiAStar {
    private float[,] voronoi_grid;
    private Vector2 goal_idx;
    private Vector2 start_idx;
    private int max_number;

    public VoronoiAStar(VoronoiGraph vg) {
        this.max_number = vg.max_number;
        vg.fill_grid();
        this.voronoi_grid = vg.grid_distance;
        this.start_idx = new Vector2(vg.start_i, vg.start_j);
        this.goal_idx = new Vector2(vg.goal_i, vg.goal_j);
    }

    public List<Vector3> a_star() {
        return a_star(this.start_idx);
    }

    public List<Vector3> a_star(Vector2 start_idx) {
        Dictionary<Vector2, float> open_set = new Dictionary<Vector2, float>();
        HashSet<Vector2> closed_set = new HashSet<Vector2>();
        Dictionary<Vector2, Vector2> parent = new Dictionary<Vector2, Vector2>();
        Dictionary<Vector2, float> current_costs = new Dictionary<Vector2, float>();

        open_set.Add(start_idx, 0);
        current_costs.Add(start_idx, 0);

        while (open_set.Count > 0) {
            Vector2 curr = get_node_with_smallest_cost(open_set, start_idx);

            if (curr == this.goal_idx) {
                Debug.Log("I am here!");
                break;
            }
            open_set.Remove(curr);
            closed_set.Add(curr);

            List<Vector2> neighbours = get_neighbours(curr, this.voronoi_grid, closed_set);
            foreach (Vector2 next in neighbours) {
                float new_cost = current_costs[curr] + 2*(this.max_number - this.voronoi_grid[(int) next.x, (int) next.y]);
                if (!current_costs.ContainsKey(next) || new_cost <= current_costs[next]) {
                    current_costs[next] = new_cost;
                    float priority = new_cost + get_heuristic(next, goal_idx)/10;
                    open_set[next] = priority;
                    parent[next] = curr;
                }
            }
        }

        // Path reconstruction: 
        List<Vector3> path = new List<Vector3>();
        Vector2 child = this.goal_idx;

        path.Add(child);

        Debug.Log("Goal idx = " + child);
        while (child != start_idx) {
            child = parent[child];
            path.Add(child);
        }
        return path;
    }


    private Vector2 get_node_with_smallest_cost(Dictionary<Vector2, float> open_set, Vector2 start_idx) {
        Vector2 curr_idx = start_idx;
        float curr_min_cost = float.MaxValue;

        foreach (KeyValuePair<Vector2, float> entry in open_set) {
            if (entry.Value < curr_min_cost) {
                curr_min_cost = entry.Value;
                curr_idx = entry.Key;
            }
        }
        return curr_idx;
    }

    private List<Vector2> get_neighbours(Vector2 curr, float[,] traversability, HashSet<Vector2> closed_set) {
        List<Vector2> neighbours = new List<Vector2>();
        if (curr.x > 1 && traversability[(int)curr.x - 1, (int)curr.y] != 1) {
            Vector2 n = new Vector2(curr.x - 1, curr.y);
            if (!closed_set.Contains(n)) {
                neighbours.Add(n);
            }
        }
        if (curr.x < traversability.GetLength(0) - 1 && traversability[(int)curr.x + 1, (int)curr.y] != 1) {
            Vector2 n = new Vector2(curr.x + 1, curr.y);
            if (!closed_set.Contains(n)) {
                neighbours.Add(n);
            }
        }
        if (curr.y > 1 && traversability[(int)curr.x, (int)curr.y - 1] != 1) {
            Vector2 n = new Vector2(curr.x, curr.y - 1);
            if (!closed_set.Contains(n)) {
                neighbours.Add(n);
            }
        }
        if (curr.y < traversability.Length - 1 && traversability[(int)curr.x, (int)curr.y + 1] != 1) {
            Vector2 n = new Vector2(curr.x, curr.y + 1);
            if (!closed_set.Contains(n)) {
                neighbours.Add(n);
            }
        }

        return neighbours;
    }

    private float get_heuristic(Vector2 curr, Vector2 next) {
        return Vector2.Distance(curr, next);
    }
}

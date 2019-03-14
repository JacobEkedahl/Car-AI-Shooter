using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPsolver {
    public List<GameObject> tour;
    TerrainManager terrain_manager;
    AStar aStar;
    public VRPsolver(List<GameObject> nodes, TerrainManager terrain_manager){
        this.tour = nodes;
        this.terrain_manager = terrain_manager;

        GridDiscretization grid = new GridDiscretization(terrain_manager.myInfo);
        //this.aStar = new AStar(grid); //astar loads this grid into a internal voronoigrid
    }

    public void construct_NN_tour(GameObject start_node){
        List<GameObject> new_tour = new List<GameObject>();
        GameObject curr_node = start_node;

        while(tour.Count > 0) {
            float closest_dist = long.MaxValue;
            int closest_ind = -1;
            for(int i = 0; i < tour.Count; i++) {
                Vector3 curr_pos = curr_node.transform.position;
                Vector3 next_pos = tour[i].transform.position;

                float this_dist = Vector3.Distance(curr_pos, next_pos);
                //aStar.initAstar(curr_node.transform.position, tour[i].transform.position);
                //float this_dist = aStar.dist_astar();
                RaycastHit hit;
                bool hits_wall = Physics.Raycast(curr_pos, next_pos - curr_pos, out hit, this_dist);
                if (hits_wall) this_dist *= 2;
                if(this_dist < closest_dist){
                    closest_dist = this_dist;
                    closest_ind = i;
                }
            }
            new_tour.Add(tour[closest_ind]);
            curr_node = tour[closest_ind];
            tour.RemoveAt(closest_ind);
        }

        tour = new_tour;
    }

    private List<GameObject> opt_swap(List<GameObject> tour, int first, int second){
        List<GameObject> new_tour = new List<GameObject>();

        for(int i = 0; i < first; i++) {
            new_tour.Add(tour[i]);
        }
        for(int i = second; i >= first; i--) {
            new_tour.Add(tour[i]);
        }
        for(int i = second + 1; i < tour.Count; i++) {
            new_tour.Add(tour[i]);
        }

        return new_tour;
    }
    
    private float tour_distance(List<GameObject> tour){
        float distance = 0;
        for (int i = 0; i < tour.Count - 1; i++){
            distance += Vector3.Distance(tour[i].transform.position, tour[i + 1].transform.position);
        }
        return distance;
    }

    public void two_opt(){
        //bool improvement = false;
        float curr_dist = tour_distance(tour);
        Debug.Log("initial distance: " + curr_dist);
        int count = 0;
        while (count < 1000){
            int i = UnityEngine.Random.Range(1, tour.Count);
            int j = UnityEngine.Random.Range(1, tour.Count);
            if (i == j) {
                continue;
            }
            List<GameObject> new_tour = opt_swap(tour, i, j);
            float new_dist = tour_distance(new_tour);
            if(new_dist < curr_dist){
                tour = new List<GameObject>(new_tour);
                curr_dist = new_dist;
            }
            count++;
        }
        Debug.Log("2opt distance: " + curr_dist);
        /*
        do{
            improvement = false;
            for(int i = 1; i < tour.Count && !improvement && count < 10000; i++){
                for(int j = i + 2; j < tour.Count && !improvement && count < 10000; j++){
                    List<GameObject> new_tour = opt_swap(tour, i, j);
                    float new_dist = tour_distance(new_tour);
                    if(new_dist < curr_dist){
                        tour = new List<GameObject>(new_tour);
                        curr_dist = new_dist;
                        improvement = true;
                    }
                    count += 1;
                }
            }
        } while (improvement && count < 10000);
        */
    }
}
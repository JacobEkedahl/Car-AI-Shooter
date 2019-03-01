using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPsolver {
    public List<GameObject> tour;
    public VRPsolver(List<GameObject> nodes){
        this.tour = nodes;
    }

    public void construct_NN_tour(GameObject start_node){
        List<GameObject> new_tour = new List<GameObject>();
        GameObject curr_node = start_node;

        while(tour.Count > 0) {
            float closest_dist = long.MaxValue;
            int closest_ind = -1;
            for(int i = 0; i < tour.Count; i++) {
                float this_dist = Vector3.Distance(tour[i].transform.position, curr_node.transform.position);
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
        bool improvement = false;
        float curr_dist = tour_distance(tour);
        do{
            improvement = false;
            for(int i = 1; i < tour.Count && !improvement; i++){
                for(int j = i + 2; j < tour.Count && !improvement; j++){
                    List<GameObject> new_tour = opt_swap(tour, i, j);
                    float new_dist = tour_distance(new_tour);
                    if(new_dist < curr_dist){
                        tour = new List<GameObject>(new_tour);
                        curr_dist = new_dist;
                        improvement = true;
                    }
                }
            }
        } while (improvement);
    }
}
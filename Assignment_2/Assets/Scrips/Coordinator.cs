using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coordinator {

    private Dictionary<String, int> nameToIndex = new Dictionary<string, int>();

    List<float> left_offsets;
    List<float> backward_offsets;
    public Coordinator(){
        left_offsets = new List<float>();
        backward_offsets = new List<float>();
        left_offsets.Add(20);
        left_offsets.Add(10);
        left_offsets.Add(-10);
        left_offsets.Add(-20);

        backward_offsets.Add(20);
        backward_offsets.Add(10);
        backward_offsets.Add(10);
        backward_offsets.Add(20);
    }

    public void setFirstCarName(GameObject car){
        nameToIndex[car.name] = 1;
    }
    public void setSecondCarName(GameObject car){
        nameToIndex[car.name] = 2;
    }
    public void setThirdCarName(GameObject car){
        nameToIndex[car.name] = 3;
    }
    public void setFourthCarName(GameObject car){
        nameToIndex[car.name] = 4;
    }

    public Vector3 get_target_position(GameObject leader, GameObject car, int index){
        Vector3 leader_position = leader.transform.position;
        Vector3 leader_direction = leader.transform.forward;
        Vector3 right = new Vector3(leader_direction.z, leader_direction.y, -leader_direction.x);
        Vector3 left = -right;

        Vector3 target_position = leader_position - backward_offsets[index] * leader_direction;
        target_position = target_position + left * left_offsets[index];
        return target_position;
    }
}
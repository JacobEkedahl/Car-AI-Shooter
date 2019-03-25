using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coordinator {

    private Dictionary<String, int> nameToIndex = new Dictionary<string, int>();

    List<float> left_offsets;
    List<float> backward_offsets;

    float width = 40;
    float length = 20;
    public Coordinator(){
        left_offsets = new List<float>();
        backward_offsets = new List<float>();
        left_offsets.Add(width);
        left_offsets.Add(width/2);
        left_offsets.Add(-width/2);
        left_offsets.Add(-width);

        backward_offsets.Add(length);
        backward_offsets.Add(length/2);
        backward_offsets.Add(length/2);
        backward_offsets.Add(length);
    }

    public Vector3 get_target_position(GameObject leader, GameObject car, int index){

        Vector3 leader_position = leader.transform.position;
        Vector3 leader_direction = leader.transform.forward;
        Vector3 right = new Vector3(leader_direction.z, leader_direction.y, -leader_direction.x).normalized;
        Vector3 left = -right;

        RaycastHit hit;
        int layerMask = 1 << LayerMask.NameToLayer("CubeWalls");

        Vector3 target_position = leader_position - backward_offsets[index] * leader_direction;
        target_position = target_position + left * left_offsets[index];

        bool is_hit = Physics.Raycast(target_position, leader_direction, out hit, length, layerMask);

        if(is_hit) target_position = leader_position - backward_offsets[index] * leader_direction;

        return target_position;
    }
}
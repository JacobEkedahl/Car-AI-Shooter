using System;
using System.Collections.Generic;
using UnityEngine;

public class Coordinator {

    private Dictionary<String, int> nameToIndex = new Dictionary<string, int>();

    List<float> left_offsets;
    List<float> backward_offsets;

    float width;
    float length;
    public Coordinator(float width, float length){
        this.width = width;
        this.length = length;
        left_offsets = new List<float>();
        backward_offsets = new List<float>();
        left_offsets.Add(width);
        left_offsets.Add(width/2);
        left_offsets.Add(-width/2);
        left_offsets.Add(-width);

        backward_offsets.Add(length);
        backward_offsets.Add(length/2);
        backward_offsets.Add(length/2 + 10);
        backward_offsets.Add(length + 10);
    }

    public Vector3 get_target_position(GameObject leader, GameObject car, int index){
        Vector3 leader_position = leader.transform.position;
        Vector3 leader_direction = leader.transform.forward;
        Vector3 right = new Vector3(leader_direction.z, leader_direction.y, -leader_direction.x).normalized;
        Vector3 left = -right;

        RaycastHit hit;
        RaycastHit hit_diag;
        int layerMask = LayerMask.GetMask("CubeWalls");

        Vector3 target_position = leader_position - backward_offsets[index] * leader_direction;
        Vector3 target_position_offset = target_position;

        target_position = target_position + left * left_offsets[index] + new Vector3(0, 1, 0);
        target_position_offset = target_position + left * left_offsets[index] * 1.2f;

        bool is_hit = Physics.Raycast(car.transform.position, target_position - car.transform.position, out hit_diag, Math.Abs(backward_offsets[index]), layerMask);
        bool is_hit_diag = Physics.Raycast(leader.transform.position, target_position - leader.transform.position, out hit_diag, (target_position - leader.transform.position).magnitude, layerMask);

        Color color = Color.yellow;
        if (is_hit_diag) color = Color.red;
        Debug.DrawRay(leader.transform.position, (target_position - leader.transform.position).normalized * (target_position - leader.transform.position).magnitude, color);

        color = Color.yellow;
        if (is_hit) color = Color.red;
        Debug.DrawRay(car.transform.position, (target_position - car.transform.position).normalized * Math.Abs(backward_offsets[index]), color);

        if(is_hit || is_hit_diag) target_position = leader_position - backward_offsets[index] * leader_direction;

        return target_position;
    }
}

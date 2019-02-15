using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyPlanner {
    public EnemyPlanner(){
    }

    public void remove_destroyed(ref List<GameObject> objects){
        foreach(GameObject obj in objects){
            if(obj == null){
                objects.Remove(obj);
            }
        }
    }

    public GameObject get_closest_object(List<GameObject> objects, Vector3 ref_point){
        float smallest_distance = float.MaxValue;
        GameObject closest_object = objects[0];
        foreach(GameObject obj in objects) {
            float distance = Vector3.Distance(obj.transform.position, ref_point);
            if(distance < smallest_distance){
                smallest_distance = distance;
                closest_object = obj;
            }
        }
        return closest_object;
    }
}
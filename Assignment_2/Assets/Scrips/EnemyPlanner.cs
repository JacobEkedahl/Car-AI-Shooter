using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyPlanner {
    public EnemyPlanner(){
    }

    public List<GameObject> remove_destroyed(List<GameObject> enemies){
        foreach(GameObject obj in enemies){
            if(obj == null){
                enemies.Remove(obj);
                Debug.Log("Removed enemy!");
            }
        }

        return enemies;
    }

    public GameObject get_closest_object(List<GameObject> enemies, Vector3 ref_point){
        float smallest_distance = float.MinValue;
        GameObject closest_object = enemies[0];
        foreach(GameObject obj in enemies) {
            float distance = Vector3.Distance(obj.transform.position, ref_point);
            if(distance > smallest_distance){
                smallest_distance = distance;
                closest_object = obj;
            }
        }
        return closest_object;
    }
}
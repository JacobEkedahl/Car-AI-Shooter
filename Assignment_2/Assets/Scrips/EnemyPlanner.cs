using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlanner {
    public EnemyPlanner () { }

    public List<GameObject> remove_destroyed (List<GameObject> enemies) {
        foreach (GameObject obj in enemies) {
            if (obj == null) {
                enemies.Remove (obj);
                Debug.Log ("Removed enemy!");
            }
        }

        return enemies;
    }

    public GameObject get_closest_object (List<GameObject> enemies, Vector3 ref_point) {
     /*   Random.seed =  (int)System.DateTime.Now.Ticks;
        float rand_float = Random.Range(0, enemies.Count);
        Debug.Log("rand float: " + rand_float);
        return enemies[(int) rand_float];
    */
        
        float smallest_distance = float.MaxValue;
        GameObject closest_object = null;
        int value = 0;
        foreach (GameObject obj in enemies) {
            if (obj == null) {
                continue;
            }
            float distance = Vector3.Distance (obj.transform.position, ref_point);
            if (distance < smallest_distance) {
                smallest_distance = distance;
                closest_object = obj;
            }
        }

        Debug.Log ("closest object: " + closest_object.transform.position);
        return closest_object;
    }
}
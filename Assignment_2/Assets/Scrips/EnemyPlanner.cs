using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlanner {
    public EnemyPlanner () { }

    public List<GameObject> remove_destroyed (List<GameObject> enemies) {
        for (int i = enemies.Count -1; i >= 0; i--)
        {
            if (enemies[i] == null)
            {
                enemies.Remove(enemies[i]);
            }
        }
        return enemies;
    }

    public GameObject get_closest_object (List<GameObject> enemies, Vector3 ref_point) {
        float smallest_distance = float.MaxValue;
        GameObject closest_object = null;

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

        return closest_object;
    }
}
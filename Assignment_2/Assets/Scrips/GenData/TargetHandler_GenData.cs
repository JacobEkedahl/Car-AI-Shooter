using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler_GenData
{
    public List<GameObject> enemies;

    public float angle { get; set; } = 0.0f;
    HashSet<int> taken = new HashSet<int>();
    public int myStart { get; set; } = 0;
    public int targetIndex { get; set; } = 0;
    private int angleInc = 90;

    public TargetHandler_GenData(TerrainInfo info) { 
        addEnemies(info); //adding cubes to map
    }
    
    //should be called after every new starting position
    private void generateTargets()
    {
        angle = 0;
        targetIndex = 0;
        taken.Clear();
        while (taken.Count < 20) {
            int randomIndex = Random.Range(0, enemies.Count - 1);

            if (!taken.Contains(myStart)) {
                taken.Add(randomIndex);
            }
        }
    }

    //angle cant be increased anymore, resetting and caller should increment startpos
    public bool incrementAngle() {
        if (angle + angleInc > 360.0) {
            angle = 0;
            return false;
        }

        angle += angleInc;
        return true;
    }



    //if returns false stop recording, should also generate targets after called
    public bool incrementStartPos() {
        if (myStart +1 == enemies.Count) {
            return false;
        }
        myStart++;
        generateTargets();
        return true;
    }

    //if it returns false increment startPos should be called
    public bool incrementTarget() {
        if (targetIndex + 1 == taken.Count) {
            return false;
        }

        targetIndex++;
        return true;
    }
    

    public GameObject getStartPos() {
        return enemies[myStart];
    }

    public GameObject getTarget()
    {
        Debug.Log("found obj: " + enemies[targetIndex] + " targetIndex: " + targetIndex);
        return enemies[targetIndex];
    }
    
    public void addEnemies(TerrainInfo info)
    {
        enemies = new List<GameObject>();
        int width = info.x_N;
        int height = info.z_N;
        float widthSquare = (info.x_high - info.x_low) / width;
        float heightSquare = (info.z_high - info.z_low) / height;
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (info.traversability[i, j] != 1)
                {
                    float x = info.x_low + (i * widthSquare) + (widthSquare / 2);
                    float z = info.x_low + (j * heightSquare) + (heightSquare / 2);
                    // Debug.Log("creating cube at: " + x + ":" + z);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.layer = 10; //Waypoint
                    cube.transform.position = new Vector3(x, -0.40f, z);
                    enemies.Add(cube);
                }
            }
        }

        Debug.Log("enemies size: " + enemies.Count);
    }
}

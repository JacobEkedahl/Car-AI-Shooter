
using System.Collections.Generic;
using UnityEngine;

public class NodeGenerator
{
    private TerrainInfo info;
    public NodeGenerator(TerrainInfo info)
    {
        this.info = info;
    }

    public List<GameObject> getObjects()
    {
        List<GameObject> enemies = new List<GameObject>();

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
                    cube.transform.position = new Vector3(x, -0.0f, z);
                    enemies.Add(cube);
                }
            }
        }

        return enemies;
    }

    public List<GameObject> generateRandomObjects(int number)
    {
        List<GameObject> targets = getObjects();
        List<GameObject> result = new List<GameObject>();

        for (int i = 0; i < number; i++)
        {
            int randomIndex = Random.Range(0, targets.Count - 1);
            while (result.Contains(targets[randomIndex]))
            {
                randomIndex = Random.Range(0, targets.Count - 1);
            }

            result.Add(targets[randomIndex]);
        }

        for (int i = targets.Count-1; i >= 0; i--)
        {
            if (!result.Contains(targets[i]))
            {
                targets[i] = null;
            }
        }

        return result;
    }
}

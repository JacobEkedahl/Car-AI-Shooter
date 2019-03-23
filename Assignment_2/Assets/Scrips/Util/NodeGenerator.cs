
using System.Collections.Generic;
using UnityEngine;

public class NodeGenerator
{
    private TerrainInfo info;
    public NodeGenerator(TerrainInfo info)
    {
        this.info = info;
    }

    public List<GameObject> prob1()
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

    /*Generate nodes at all the gridspaces as prob 1.
     *Find nodes which only has one neighbour, on these path add the nodes which covers the least amount of nodes while covering the most area
     *Use GeneticTsp for these nodes
     *Recreate the path between the nodes and along the path remove nodes which can be seen from our total set.
     *If any nodes are remaining let the third car take these. Otherwise let the third car navigate from the middle position on the path
     */
    public List<GameObject> prob2()
    {

        List<GameObject> enemies = new List<GameObject>();
        
        return enemies;
    }

    public List<GameObject> prob3()
    {
        List<GameObject> enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        return enemies;
    }

    public List<GameObject> generateRandomObjects(int number)
    {
        List<GameObject> targets = prob1();
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

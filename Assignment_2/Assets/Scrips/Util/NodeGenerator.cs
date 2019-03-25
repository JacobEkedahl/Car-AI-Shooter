
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeGenerator {
    private TerrainInfo info;
    private static NodeGenerator instance = null;

    public static NodeGenerator getInstance() {
        if (instance == null) {
            instance = new NodeGenerator();
        }

        return instance;
    }

    private NodeGenerator() {
    }

    public List<GameObject> prob1(TerrainInfo info) {
        this.info = info;
        List<GameObject> enemies = new List<GameObject>();

        enemies = new List<GameObject>();
        int width = info.x_N;
        int height = info.z_N;
        float widthSquare = (info.x_high - info.x_low) / width;
        float heightSquare = (info.z_high - info.z_low) / height;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (info.traversability[i, j] != 1) {
                    enemies.Add(createCube(i, j, widthSquare, heightSquare));
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
     
    List<AdjNode> nodes;
    public List<GameObject> prob2(TerrainInfo info) {
        this.info = info;
        List<GameObject> enemies = new List<GameObject>();

        enemies = new List<GameObject>();
        int width = info.x_N;
        int height = info.z_N;
        float widthSquare = (info.x_high - info.x_low) / width;
        float heightSquare = (info.z_high - info.z_low) / height;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (info.traversability[i, j] != 1) {
                    enemies.Add(createCube(i, j, widthSquare, heightSquare));
                }
            }
        }

        return enemies;
    }

    public List<GameObject> getPriority(List<GameObject> myNodes, Vector3 carPos) {
        Cluster cluster = new Cluster(3, info, myNodes);
        List<GameObject> chosenCluster = myNodes;
        cluster.run();
        float minDist = float.MaxValue;
        for(int i = 0; i < cluster.cluster_means.Count; i++) {
            float dist = Vector3.Distance(cluster.cluster_means[i], carPos);
            if (dist < minDist) {
                minDist = dist;
                chosenCluster = cluster.clusters[i];
            }
        }

        return chosenCluster;
    }
    
    public GameObject createCube(int i, int j, float widthSquare, float heightSquare) {

        float x = info.x_low + (i * widthSquare) + (widthSquare / 2);
        float z = info.x_low + (j * heightSquare) + (heightSquare / 2);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.layer = 10; //Waypoint
        cube.transform.position = new Vector3(x, -0.0f, z);

        return cube;
    }
    
    public List<GameObject> prob3() {
        List<GameObject> enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        return enemies;
    }

    public List<GameObject> generateRandomObjects(int number, TerrainInfo info) {
        List<GameObject> targets = prob1(info);
        List<GameObject> result = new List<GameObject>();

        for (int i = 0; i < number; i++) {
            int randomIndex = Random.Range(0, targets.Count - 1);
            while (result.Contains(targets[randomIndex])) {
                randomIndex = Random.Range(0, targets.Count - 1);
            }

            result.Add(targets[randomIndex]);
        }

        for (int i = targets.Count - 1; i >= 0; i--) {
            if (!result.Contains(targets[i])) {
                targets[i] = null;
            }
        }

        return result;
    }
}

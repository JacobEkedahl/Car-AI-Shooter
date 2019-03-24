
using System.Collections.Generic;
using UnityEngine;

public class NodeGenerator {
    private TerrainInfo info;
    public NodeGenerator(TerrainInfo info) {
        this.info = info;
    }

    public List<GameObject> prob1() {
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
    public List<GameObject> prob2() {
        if (nodes == null) {
            initAdjNodes();
        }

        for (int i = 1; i <= 4; i++) {
            List<GameObject> enemies = new List<GameObject>();
            foreach (AdjNode node in nodes) {
                if (node.getCount() == i) {
                    enemies.Add(node.obj);
                }
            }
        }

        List<GameObject> result = new List<GameObject>();
        foreach (AdjNode node in nodes) {
            int count = node.getCount();
            Debug.Log("count: " + count);
            if (count < 3) {
                result.Add(node.obj);
            }

            if (count == 2) {
                node.obj.GetComponent<Renderer>().material.color = Color.red;
            }
        }

        Debug.Log("size of lonely nodes: " + result.Count + ", nodes count: " + nodes.Count);
        return result;
    }

    private void initAdjNodes() {
        nodes = new List<AdjNode>();

        int width = info.x_N;
        int height = info.z_N;
        float widthSquare = (info.x_high - info.x_low) / width;
        float heightSquare = (info.z_high - info.z_low) / height;

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (info.traversability[i, j] != 1) {
                    AdjNode curr = getNode(i, j);
                    if (curr == null) {
                        GameObject cube = createCube(i, j, widthSquare, heightSquare);
                        curr = new AdjNode(cube, i, j);
                        nodes.Add(curr);
                    }

                    //add all the neighbors to this node
                    GameObject west = GetNeighbor("West", info, i, j, widthSquare, heightSquare);
                    GameObject north = GetNeighbor("North", info, i, j, widthSquare, heightSquare);
                    GameObject east = GetNeighbor("East", info, i, j, widthSquare, heightSquare);
                    GameObject south = GetNeighbor("South", info, i, j, widthSquare, heightSquare);

                    curr.addNeighbor(west);
                    curr.addNeighbor(north);
                    curr.addNeighbor(east);
                    curr.addNeighbor(south);
                }
            }
        }
    }

    public AdjNode getNode(int i, int j) {
        for (int index = 0; index < nodes.Count; index++) {
            if (nodes[index].i == i && nodes[index].j == j) {
                return nodes[index];
            }
        }

        return null;
    }

    public GameObject GetNeighbor(string type, TerrainInfo info, int i, int j, float widthSquare, float heightSquare) {
        switch (type) {
            case "West":
                i--;
                break;
            case "North":
                j--;
                break;
            case "East":
                i++;
                break;
            case "South":
                j++;
                break;
        }
        if (i > 0 && info.traversability[i, j] != 1) {
            //create this neighbor if i already havent
            bool haveCreated = false;
            for (int index = 0; index < nodes.Count; index++) {
                if (nodes[index].i == i && nodes[index].j == j) //yes i have
                {
                    return nodes[index].obj;
                }
            }

            if (!haveCreated) {
                GameObject cube = createCube(i, j, widthSquare, heightSquare);
                AdjNode neighbor = new AdjNode(cube, i, j);
                nodes.Add(neighbor);
                return cube;
            }
        }

        return null;
    }

    public GameObject createCube(int i, int j, float widthSquare, float heightSquare) {

        float x = info.x_low + (i * widthSquare) + (widthSquare / 2);
        float z = info.x_low + (j * heightSquare) + (heightSquare / 2);
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.layer = 10; //Waypoint
        cube.transform.position = new Vector3(x, -0.0f, z);

        return cube;
    }

    public List<GameObject> prob2(List<GameObject> remainers) {
        //filter out the nodes which are not in remainers in our list of adjnodes
        return null;
    }

    public List<GameObject> prob3() {
        List<GameObject> enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        return enemies;
    }

    public List<GameObject> generateRandomObjects(int number) {
        List<GameObject> targets = prob1();
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

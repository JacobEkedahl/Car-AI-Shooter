using System.Collections.Generic;
using UnityEngine;

public class AdjNode {
    public List<AdjNode> visibleNodes { get; set; }
    public List<GameObject> neighbors;
    public GameObject obj { get; set; }
    public int i { get; set; }
    public int j { get; set; }

    public AdjNode(GameObject me, int i, int j) {
        visibleNodes = new List<AdjNode>();
        neighbors = new List<GameObject>();
        this.obj = me;
        this.i = i;
        this.j = j;
    }

    public void addNeighbor(GameObject neighbor) {
        neighbors.Add(neighbor);
    }

    public void addVisibleNode(AdjNode node) {
        visibleNodes.Add(node);
    }

    public int getCount() {
        int result = 0;
        for (int i = 0; i < neighbors.Count; i++) {
            if (neighbors[i] != null) {
                result++;
            }
        }

        return result;
    }


}

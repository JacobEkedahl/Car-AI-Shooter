using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeFilter {
    public static List<GameObject> filterBasedOnCount(List<AdjNode> nodes) {
        List<GameObject> result = new List<GameObject>();

        List<AdjNode> chosenTargets = new List<AdjNode>();
        //We have a set I, starting from empty
        
        //Load the targets from nodes, should do for count 0, 1, 2... until all targets have been covered
        for (int i = 0; i <= 2; i++) {

            List<AdjNode> I = new List<AdjNode>();
            //Find the targets, prioritize targets with low count
            List<AdjNode> enemies = new List<AdjNode>(nodes.Where(w => !w.hasBeenCovered && w.getCount() == i));
            
            while (!isEqual(I, enemies)) {
                int biggestDifference = int.MinValue;
                AdjNode chosenTarget = null;
                foreach (AdjNode target in nodes) {
                    HashSet<AdjNode> tmp = new HashSet<AdjNode>(I);
                    int sizeBefore = tmp.Count;

                    tmp.UnionWith(target.visibleNodes.Where(w => w.getCount() == i && !w.hasBeenCovered));
                    int sizeAfter = tmp.Count;
                    int difference = sizeAfter - sizeBefore;

                    if (difference > biggestDifference) {
                        biggestDifference = difference;
                        chosenTarget = target;
                    }
                }

                Debug.Log("biggestDiff: " + biggestDifference);

                chosenTargets.Add(chosenTarget);
                I.AddRange(chosenTarget.visibleNodes.Where(w => w.getCount() == i));
                markAsTaken(chosenTarget);

            }
        }
        return convert(chosenTargets);
    }

    private static void markAsTaken(AdjNode node) {
        foreach (AdjNode visibleNodes in node.visibleNodes) {
            visibleNodes.hasBeenCovered = true;
        }

        node.hasBeenCovered = true;
    }

    public static List<GameObject> convert(List<AdjNode> nodes) {
        List<GameObject> result = new List<GameObject>();

        foreach (AdjNode node in nodes) {
            node.obj.GetComponent<Renderer>().material.color = Color.green;
            result.Add(node.obj);
        }

        return result;
    }

    private static bool isEqual<T>(List<T> targets, List<T> enemies) {
        HashSet<T> tmpTargets = new HashSet<T>(targets);
        HashSet<T> tmpEnemies = new HashSet<T>(enemies);
        if (tmpTargets.Count != tmpEnemies.Count) {
            Debug.Log("not equal: " + tmpTargets.Count + ":" + tmpEnemies.Count);
            return false;
        }

        Debug.Log("equal: " + tmpTargets.Count + ":" + tmpEnemies.Count);
        return true;
    }
}
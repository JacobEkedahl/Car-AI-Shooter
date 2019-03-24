using System.Collections.Generic;

public class NodeFilter {
    public static List<AdjNode> filter(List<AdjNode> nodes, int count, int width, int height) {
        List<AdjNode> newTargets = new List<AdjNode>();
        //find the node that can see the most nodes of this count

        //go up, down, left, and right and count and if there is no node with those indexes stop that direction
        foreach (AdjNode node in nodes) {
            //look right
            for (int i = node.i + 1; i < width; i++) {
                AdjNode visibleNode = findNode(i, node.j, nodes);
                if (visibleNode == null) {
                    break;
                } else {
                    node.addVisibleNode(visibleNode);
                }
            }

            //look left
            for (int i = node.i - 1; i >= 0; i--) {
                AdjNode visibleNode = findNode(i, node.j, nodes);
                if (visibleNode == null) {
                    break;
                } else {
                    node.addVisibleNode(visibleNode);
                }
            }
        }


        return newTargets;
    }

    private static AdjNode findNode(int i, int j, List<AdjNode> nodes) {
        foreach (AdjNode node in nodes) {
            if (node.i == i && node.j == j) {
                return node;
            }
        }

        return null;
    }

}
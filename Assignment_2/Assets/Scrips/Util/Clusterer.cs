using Accord.MachineLearning;
using System.Collections.Generic;
using UnityEngine;

public class Clusterer {
    public static List<NodesMap> generate(NodesMap map, int k) {

        Accord.Math.Random.Generator.Seed = 0;
        List<NodesMap> result = new List<NodesMap>();
        List<int> nodes = map.getNodes();

        double[][] observations = new double[nodes.Count][];
        int obsIndex = 0;
        foreach (int index in nodes) {
            double[] observation = new double[nodes.Count];
            int currIndex = 0;
            foreach (int otherIndex in nodes) {
                if (index == otherIndex) {
                    observation[currIndex++] = 0.0;
                } else {
                    observation[currIndex++] = map.getDistance(index, otherIndex);
                }
            }

            observations[obsIndex++] = observation;
        }


        // Create a new K-Means algorithm
        KMeans kmeans = new KMeans(k: k);

        // Compute and retrieve the data centroids
        KMeansClusterCollection clusters = kmeans.Learn(observations);

        // Use the centroids to parition all the data
        int[] labels = clusters.Decide(observations);

        List<List<int>> buckets = new List<List<int>>();

        for (int i = 0; i < labels.Length; i++) {
            buckets.Add(new List<int>());
        }


        for (int i = 0; i < observations.Length; i++) {
            int bucket = clusters.Decide(observations[i]);
            buckets[bucket].Add(nodes[i]);
            Debug.Log("bucket " + bucket + " gets: " + nodes[i]);
        }

        foreach (List<int> bucket in buckets) {
            NodesMap newMap = new NodesMap();
            Debug.Log("bucket :" + bucket.Count);
            //get all the indexes in this bucket
            for (int i = 0; i < bucket.Count; i++) {
                for (int j = 0; j < bucket.Count; j++) {
                    if (i != j) {
                        int realI = bucket[i];
                        int realJ = bucket[j];

                        float distance = map.getDistance(i, j);
                        float startAngle = map.getStartAngle(i, j);
                        float endAngle = map.getEndAngle(i, j);

                        //        Debug.Log("i:" + realI + ", j:" + realJ + ", d: " + distance + ", s: " + startAngle + ", e: " + endAngle);

                        newMap.add(realI, realJ, distance, startAngle, endAngle);
                    }
                }
            }

            if (bucket.Count != 0) {
                result.Add(newMap);
            }
        }

        return result;
    }
}

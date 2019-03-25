using System;
using System.Collections.Generic;
using UnityEngine;

public class PathDivider {
    public static List<int> getChunks(float[] distances, int k, int startIndex) {
        //want to minimize the max size of a chunk
        int[] res = new int[k + 1];
        res[0] = startIndex;

        int chunkSize = distances.Length / k;
        int lastChunk = distances.Length - (chunkSize * k) + chunkSize;

        for (int i = 1; i <= k; i++) {
            if (i == k) {
                res[i] = lastChunk;
            } else {
                res[i] = chunkSize;
            }
        }

        //adjust the startIndex
        int[] currBest = new int[k + 1];
        float currMin = float.MaxValue;

        for (int j = 0; j < distances.Length; j++) {
            //vary the size of the first chunk
            for (int b = 0; b < distances.Length - j; b++) {
                int endChunk = distances.Length - j - b;

                res[1] = j;
                res[2] = b;
                res[3] = endChunk;

                float thisMin = getMax(distances, res);
                if (thisMin < currMin) {
                    currBest = (int[])res.Clone();
                    currMin = thisMin;
                }
            }
        }

        List<int> returnVal = new List<int>();
        for (int i = 0; i < currBest.Length; i++) {
            returnVal.Add(currBest[i]);
        }

        Debug.Log("maxdist: " + getMax(distances, currBest));
        return returnVal;
    }

    private static float getMax(float[] distances, int[] chunks) {
        int startIndex = chunks[0];
        float maxVal = float.MinValue;

        for (int i = 1; i < chunks.Length; i++) {
            float dist = 0.0f;
            for (int j = 0; j < chunks[i]; j++) {
                dist += distances[(startIndex + j) % distances.Length];
            }

            maxVal = Math.Max(maxVal, dist);
            startIndex += chunks[i];
        }

        return maxVal;
    }
}


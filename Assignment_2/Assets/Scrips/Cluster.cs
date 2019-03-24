using System.Collections.Generic;
using UnityEngine;

public class Cluster {

    public List<List<GameObject>> clusters;
    public List<Vector3> cluster_means;
    private TerrainManager terrain_manager;
    private List<GameObject> enemies;

    private AStar aStar;
    public int k;

    public Cluster(int k, TerrainManager terrain_manager, List<GameObject> enemies) {
        Debug.Log("in cluster: " + enemies.Count);
        this.k = k;
        this.terrain_manager = terrain_manager;
        this.clusters = new List<List<GameObject>>();
        this.cluster_means = new List<Vector3>();
        this.enemies = enemies;
        GridDiscretization grid = new GridDiscretization(terrain_manager.myInfo, 1, 1, 4);
        // aStar = new AStar(grid); //astar loads this grid into a internal voronoigrid

        for (int i = 0; i < k; i++) {
            clusters.Add(new List<GameObject>());
            float random_x = UnityEngine.Random.Range(terrain_manager.myInfo.x_low, terrain_manager.myInfo.x_high);
            float random_z = UnityEngine.Random.Range(terrain_manager.myInfo.z_low, terrain_manager.myInfo.z_high);
            cluster_means.Add(new Vector3(random_x, 0, random_z));
        }
    }

    public void run() {
        float diff = 0;

        int count = 0;
        do {
            diff = 0;
            List<Vector3> old_cluster_means = new List<Vector3>(cluster_means);
            update_clusters();
            for (int i = 0; i < k; i++) {
                diff += Vector3.Distance(old_cluster_means[i], cluster_means[i]);
            }
            count++;
            if (count > 500) {
                break;
            }

        } while (diff > 1);
    }

    public void update_clusters() {
        for (int i = 0; i < enemies.Count; i++) {
            int cluster_id = 0;
            float shortest_distance = float.MaxValue;
            for (int j = 0; j < k; j++) {
                //aStar.initAstar(cluster_means[j], enemies[i].transform.position);
                //float distance = aStar.dist_astar();
                Vector3 enemy_pos = enemies[i].transform.position;
                float distance = Vector3.Distance(enemy_pos, cluster_means[j]);
                RaycastHit hit;
                bool hits_wall = Physics.Raycast(cluster_means[j], enemy_pos - cluster_means[j], out hit, Vector3.Distance(cluster_means[j], enemy_pos));
                if (hits_wall) distance *= 2;
                if (distance < shortest_distance) {
                    shortest_distance = distance;
                    cluster_id = j;
                }
            }
            clusters[cluster_id].Add(enemies[i]);
        }

        for (int i = 0; i < k; i++) {
            float tot_x = 0;
            float tot_z = 0;
            for (int j = 0; j < clusters[i].Count; j++) {
                Vector3 pos = clusters[i][j].transform.position;
                tot_x += pos.x;
                tot_z += pos.z;
            }
            cluster_means[i] = new Vector3(tot_x / clusters[i].Count, 0, tot_z / clusters[i].Count);
        }
    }
}
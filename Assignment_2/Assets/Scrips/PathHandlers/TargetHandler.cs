using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler : MonoBehaviour
{
    public GameObject terrain_manager_game_object;
    TerrainManager terrain_manager;
    public List<GameObject> enemies;

    public int no_clusters {get; set;}
    public int no_enemies {get; set;}
                
    List<Vector3> cluster_means;
    Cluster cluster;
    List<List<GameObject>> clusters;
    
    // Start is called before the first frame update
    void Start()
    {
        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        Debug.Log("enemies size: " + enemies.Count);
        no_clusters = GameObject.FindGameObjectsWithTag("Player").Length;
        no_enemies = enemies.Count;
    }

    public int current_car = 0;
    public List<GameObject> getCluster() {
        return this.clusters[current_car++ % no_clusters];
    }

    private void generateCluster() {
        Debug.Log("enemies in generate: " + enemies.Count);
        Cluster cluster = new Cluster(no_clusters, terrain_manager, enemies);
        cluster.run();
        this.clusters = cluster.clusters;
    }                                  
    
    public bool has_clustered {get; set;} = false;
    // Update is called once per frame
    void Update()
    {
        if (no_enemies == 0) {
            enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
            no_enemies = enemies.Count;
        }

        if (no_clusters == 0) {
            no_clusters = GameObject.FindGameObjectsWithTag("Player").Length;
        }

        if (no_clusters != 0 && no_enemies != 0 && !has_clustered) {
            //do clustering
            generateCluster();

            //end clustering
            has_clustered = true;
        }
    }
}

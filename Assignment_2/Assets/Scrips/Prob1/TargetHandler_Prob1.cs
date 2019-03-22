using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler_Prob1 : MonoBehaviour
{
    public GameObject terrain_manager_game_object;
    TerrainManager terrain_manager;
    public List<GameObject> enemies;

    public int no_clusters { get; set; }
    public int no_enemies { get; set; }

    List<Vector3> cluster_means;
    Cluster cluster;
    List<List<GameObject>> clusters;

    // Start is called before the first frame update
    void Start()
    {

        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();

        //instantiate enemies
        //loop through the terrain and if not obstacle instatiate a cube
        TerrainInfo info = terrain_manager.myInfo;
        NodeGenerator generator = new NodeGenerator(info);
        enemies = generator.getObjects();

        no_clusters = GameObject.FindGameObjectsWithTag("Player").Length;
        no_enemies = enemies.Count;
    }

    //cars call this method to get one of the generated clusters
    public int current_car = 0;
    public List<GameObject> getCluster()
    {
        return this.clusters[current_car++ % no_clusters];
    }

    private void generateCluster(List<GameObject> enemies)
    {
        Cluster cluster = new Cluster(no_clusters, terrain_manager, enemies);
        cluster.run();
        this.clusters = cluster.clusters;
    }

    public bool has_clustered { get; set; } = false;
    // Update is called once per frame
    void Update()
    {
        if (no_clusters == 0)
        {
            no_clusters = GameObject.FindGameObjectsWithTag("Player").Length;
        }

        if (no_clusters != 0 && no_enemies != 0 && !has_clustered)
        {
            //do clustering
            generateCluster(enemies);
            //end clustering
            has_clustered = true;
        }
    }
}

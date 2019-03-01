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
        addEnemies(info);

        no_clusters = GameObject.FindGameObjectsWithTag("Player").Length;
        no_enemies = enemies.Count;
    }

    public void addEnemies(TerrainInfo info)
    {
        enemies = new List<GameObject>();
        int width = info.x_N;
        int height = info.z_N;
        float widthSquare = (info.x_high - info.x_low) / width;
        float heightSquare = (info.z_high - info.z_low) / height;
        Debug.Log("square width and height: " + widthSquare + ":" + heightSquare + ":" + info.x_N + ":" + info.x_low + ":" + info.x_high);
        List<string> res = new List<string>();

        for (int i = 0; i < width; i ++)
        {
            string row = "";
            for (int j = 0; j < height; j++)
            {
                row += info.traversability[i,j] + " ";
                if (info.traversability[i,j] != 1)
                {
                    float x = info.x_low + (i * widthSquare) + (widthSquare / 2);
                    float z = info.x_low + (j * heightSquare) + (heightSquare / 2);
                   // Debug.Log("creating cube at: " + x + ":" + z);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.layer = 10; //Waypoint
                    cube.transform.position = new Vector3(x, -0.40f, z);
                    enemies.Add(cube);
                    Debug.Log("cube: " + cube);
                }
            }
            res.Add(row);
        }

        foreach(string s in res)
        {
            Debug.Log(s);
        }
        Debug.Log("Width: " + width + ", height: " + height);
    }

    public int current_car = 0;
    public List<GameObject> getCluster()
    {
        return this.clusters[current_car++ % no_clusters];
    }

    private void generateCluster()
    {
        Cluster cluster = new Cluster(no_clusters, terrain_manager);
        cluster.run();
        this.clusters = cluster.clusters;
    }

    public bool has_clustered { get; set; } = false;
    // Update is called once per frame
    void Update()
    {
        if (no_enemies == 0)
        {
            enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
            no_enemies = enemies.Count;
        }

        if (no_clusters == 0)
        {
            no_clusters = GameObject.FindGameObjectsWithTag("Player").Length;
        }

        if (no_clusters != 0 && no_enemies != 0 && !has_clustered)
        {
            //do clustering
            generateCluster();

            //end clustering
            has_clustered = true;
        }
    }
}

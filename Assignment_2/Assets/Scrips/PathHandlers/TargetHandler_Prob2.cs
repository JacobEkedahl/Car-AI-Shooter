using Assets.Scrips.PathHandlers;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandler_Prob2 : MonoBehaviour
{
    public GameObject terrain_manager_game_object;
    TerrainManager terrain_manager;
    public List<GameObject> enemies;

    public int no_clusters { get; set; }
    public int no_enemies { get; set; }

    List<Vector3> cluster_means;
    Cluster cluster;
    List<List<GameObject>> clusters;
    private TerrainInfo info;

    // Start is called before the first frame update
    void Start()
    {
        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
        info = terrain_manager.myInfo;

        no_clusters = GameObject.FindGameObjectsWithTag("Player").Length;
    }

    public int current_car = 0;
    public List<GameObject> getCluster()
    {
        return this.clusters[current_car++ % no_clusters];
    }


    public void addEnemies(TerrainInfo info)
    {
        enemies = new List<GameObject>();
        int width = info.x_N;
        int height = info.z_N;
        float widthSquare = (info.x_high - info.x_low) / width;
        float heightSquare = (info.z_high - info.z_low) / height;
        //Debug.Log("square width and height: " + widthSquare + ":" + heightSquare + ":" + info.x_N + ":" + info.x_low + ":" + info.x_high);
        List<CubeTarget> targets = new List<CubeTarget>();
        List<GameObject> tmpCubes = new List<GameObject>();

        for (int i = 0; i < width; i++)
        {
            string row = "";
            for (int j = 0; j < height; j++)
            {
                row += info.traversability[i, j] + " ";
                if (info.traversability[i, j] != 1)
                {
                    float x = info.x_low + (i * widthSquare) + (widthSquare / 2);
                    float z = info.x_low + (j * heightSquare) + (heightSquare / 2);
                    // Debug.Log("creating cube at: " + x + ":" + z);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.layer = 10; //Waypoint
                    cube.transform.position = new Vector3(x, -0.40f, z);
                    targets.Add(new CubeTarget(cube));
                    tmpCubes.Add(cube);

                }
            }
        }

        enemies = TargetFilter.filter(targets);

        foreach(GameObject cube in tmpCubes) {
            if (!enemies.Contains(cube)) {
                Destroy(cube);
            }
        }

        Debug.Log("found enemies!");
        hasEnemies = true;
    }


    private void generateCluster()
    {
        Debug.Log("enemies in generate: " + enemies.Count);
        Cluster cluster = new Cluster(no_clusters, terrain_manager, enemies);
        cluster.run();
        this.clusters = cluster.clusters;
    }

    public bool hasEnemies { get; set; } = false;
    private bool isCalculating = false;
    public bool has_clustered { get; set; } = false;
    // Update is called once per frame
    void FixedUpdate()
    {
	    if (no_clusters == 0)
	    {
		    no_clusters = GameObject.FindGameObjectsWithTag("Player").Length;
	    }

	    if (GameObject.FindGameObjectsWithTag("Enemy").Length != 0 && !isCalculating)
        {
            Debug.Log("is finding enemies!");
            isCalculating = true;
            addEnemies(info);
        }

	    if (no_clusters != 0 && hasEnemies && !has_clustered)
	    {
		    //do clustering
		    generateCluster();

		    //end clustering
		    has_clustered = true;
	    }
    }
}

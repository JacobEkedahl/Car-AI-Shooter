using System.Collections.Generic;
using UnityEngine;

/*This script is communicating with the car and gives the cars the nodes which it should follow.
 *
 * 
 * 
 */
public class TargetHandler : MonoBehaviour {
    public string problem = "Prob3";
    public bool create_node_map = false;
    public GameObject terrain_manager_game_object;
    TerrainManager terrain_manager;
    public List<GameObject> enemies;

    public int no_clusters { get; set; }
    public int no_enemies { get; set; }

    List<List<GameObject>> vrpClusters;
    private GridDiscretization grid;
    private AStar astar;

    // Start is called before the first frame update
    /* Here we fetch the nodes dependent on our problem.
     * We cluster the nodes.
     * We perform VRP on each cluster.
     */
    void Start() {
        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
        GridDiscretization grid = new GridDiscretization(terrain_manager.myInfo, 1, 1, 4);
        NodeGenerator generator = NodeGenerator.getInstance();
        vrpClusters = new List<List<GameObject>>();
        no_clusters = GameObject.FindGameObjectsWithTag("Player").Length;

        switch (problem) {
            case "Prob1":
                Debug.Log("problem 1");
                enemies = generator.prob1(terrain_manager.myInfo);
                break;
            case "Prob2":
                Debug.Log("problem 2");
                enemies = generator.prob2(terrain_manager.myInfo);
                break;
            case "Prob3":
                enemies = generator.prob3();
                astar = new AStar(grid, true);

                //has fetch the enemies for this problem now need to cluster these enemies
                //first we have to log the distance and angles between all the nodes in each cluster.
                if (create_node_map) {
                    DataGenerator.generate(grid, enemies, problem + "/", true);
                }

                //now we have generated and saved our nodesmap, clustering should be based on astar distance
                List<GameObject> route = GAConnector.getPath(enemies, problem + "/");
                dividePath(route, no_clusters);
                has_clustered = true;
                break;
            case "Prob5":
                no_clusters = 1;
                enemies = generator.prob3();
                astar = new AStar(grid, true);

                //has fetch the enemies for this problem now need to cluster these enemies
                //first we have to log the distance and angles between all the nodes in each cluster.
                if (create_node_map) {
                    DataGenerator.generate(grid, enemies, problem + "/", true);
                }

                //now we have generated and saved our nodesmap, clustering should be based on astar distance
                vrpClusters.Add(GAConnector.getPath(enemies, problem + "/"));
                has_clustered = true;
                break;
            default:
                break;
        }

        no_enemies = enemies.Count;
    }

    private void generateCluster() {
        Debug.Log("enemies count: " + enemies.Count);
        Cluster cluster = new Cluster(no_clusters, terrain_manager, enemies);
        cluster.run();
        List<Vector3> means = cluster.cluster_means;
        this.vrpClusters = cluster.clusters;
    }

    private void dividePath(List<GameObject> route, int no_clusters) {
        int chunk = route.Count / no_clusters;
        int startIndex = 0;

        for (int i = 0; i < no_clusters; i++) {
            //last cluster
            if (i == no_clusters - 1) {
                chunk = chunk + (route.Count - chunk * no_clusters);
                Debug.Log("new chunk: " + chunk);
            }
            List<GameObject> cluster = new List<GameObject>();
            for (int j = 0; j < (route.Count + chunk); j++) {
                cluster.Add(route[(startIndex + j) % (route.Count)]);
            }
            vrpClusters.Add(cluster);
            startIndex += chunk;
        }

        //assuming all cars have a similar position, now trying to see which nodes car should go to first
        Vector3 car_pos = GameObject.FindGameObjectsWithTag("Player")[0].transform.position;
        foreach (List<GameObject> cluster in vrpClusters) {
            Vector3 startNode = cluster[0].transform.position;
            Vector3 endNode = cluster[cluster.Count - 1].transform.position;
            float distanceStart = astar.getDistance(startNode, endNode);
            float distanceEnd = astar.getDistance(startNode, endNode);

            if (distanceEnd < distanceStart) {
                cluster.Reverse();
            }
        }
    }

    private List<int> findMinimumSize(int nodes, int players) {
        //first divide them equally
        List<int> indexes = new List<int>();
        int nodes_total = nodes;
        int chunk = nodes_total / players;

        for (int i = 0; i < players; i++) {
            indexes.Add(players * chunk);
        }

        return indexes;
    }

    //car should only navigate the route if there is more than 3 nodes on route
    private List<GameObject> reorderRoute(Vector3 startPos, List<GameObject> route) {
        //setting the startnode closest to car position
        List<GameObject> result = new List<GameObject>();
        float minDistance = float.MaxValue;
        int startIndex = 0;

        for (int i = 0; i < route.Count; i++) {
            float distance = Vector3.Distance(route[i].transform.position, startPos);
            if (distance < minDistance) {
                minDistance = distance;
                startIndex = i;
            }
        }

        for (int i = 0; i < route.Count; i++) {
            result.Add(route[startIndex++ % route.Count]);
        }

        return result;
    }

    public int current_car = 0;
    public List<GameObject> getCluster() {
         return this.vrpClusters[current_car++ % no_clusters];
    }

    public bool has_clustered { get; set; } = false;
    // Update is called once per frame
    void Update() {
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

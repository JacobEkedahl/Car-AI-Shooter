using Assets.Scrips.GenData;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class CarManager: MonoBehaviour
{
    public bool measureNodes;
    public bool measureAngles;
    public GameObject car;
    public GameObject terrain_manager_game_object;
    TerrainManager terrain_manager;
    TargetHandler_GenData targetHandler;
    CarAI carAI;
    
    private float start_time;
    private float completion_time;
    
    //data to save
    private float distance;
    private float angle;
    private float time;

    private AStar astar;
    private static int noRandomNodes = 5;

    // Use this for initialization
    void Start()
    {
        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
        GridDiscretization grid = new GridDiscretization(terrain_manager.myInfo, 1, 1, 4);
        NodeGenerator generator = new NodeGenerator(terrain_manager.myInfo);
        List<GameObject> targets = generator.generateRandomObjects(noRandomNodes);
        targetHandler = new TargetHandler_GenData(targets);

        if (!measureAngles) {
            targetHandler.maxAngle = 0;
        }

        astar = new AStar(grid, false);
        carAI = car.GetComponent<CarAI>();
        car.transform.position = new Vector3(220.0f, 0.5f, 230.0f);
        car.transform.rotation = Quaternion.identity;

        calculateDist();
        setTime();
    }

    //is reseted when the car has planned its path
    public void setTime() {
        start_time = Time.time;
        completion_time = start_time - 1f;
    }

    public float offset_angle { get; set; }
    public void calculateDist() {
        Vector3 start = getStartPos();
        Vector3 goal = targetHandler.getTarget().transform.position;

        astar.initAstar(start, goal);
        List<Vector3> path = astar.getPath();
        
        path = astar.reconstructPath(path);
        distance = astar.dist_astar(path);
        car.transform.rotation = Quaternion.identity;
        Debug.Log("forward: " + car.transform.forward);
        offset_angle = astar.getAngleStart(path[1], car.transform.forward);
        astar.getAngleEnd(path[path.Count-2], car.transform.forward);
        // Debug.Log("offset: " + offset_angle);
    }

    public float getRotation() {
        angle = targetHandler.angle;
        return angle + offset_angle;
    }
    
    public Vector3 getStartPos() {
        Vector3 newPos = targetHandler.getStartPos().transform.position;
        newPos.y = 0.5f;
        return newPos;
    }

    public bool canFetch () {
        return targetHandler != null;
    }

    public bool hasFetched { get; set; } = false;
    public List<GameObject> getTargets() {
        List<GameObject> objects = new List<GameObject>();
        GameObject target = targetHandler.getTarget();

        objects.Add(target);
        hasFetched = true;
        return objects;
    }

   // private StringBuilder builder = new StringBuilder();
    private void updateBuilder() {
        if (distance != 0) {
            //Debug.Log(time + ":" + distance + ":" + angle);
            DataSaver.save(time, distance, angle);
        }
    }

    private void save() {
        Debug.Log("saving....");
      //  DataSaver.save(builder);
        EditorApplication.Exit(0);
    }

    public bool saveTime(float currTime) {
        completion_time = currTime - start_time;
        time = completion_time;
       // Debug.Log("time completion: " + time);
        updateBuilder();

        // Debug.Log("finished in : " + completion_time.ToString("n2") + "seconds!");
        if (!targetHandler.incrementAngle())
        { //new target
            if (!targetHandler.incrementTarget())
            { //new startpos
                if (!targetHandler.incrementStartPos())
                { //end life/gamee :)
                    save();
                } else
                {
                    calculateDist();
                }
            }
            else
            {
                calculateDist();
            }
        }
        hasFetched = false;
        return true;
    }
    
    public bool reachedTarget { get; set; } = false;
    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = true;
#endif
        Time.timeScale = 10.0f;
        if (Time.time - start_time > 160) {
            setTime();
            carAI.reset();
        }
    }
}

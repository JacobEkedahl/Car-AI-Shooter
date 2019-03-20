using Assets.Scrips.GenData;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class CarManager: MonoBehaviour
{
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

    private RealAstar astar;

    // Use this for initialization
    void Start()
    {
        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
        targetHandler = new TargetHandler_GenData(terrain_manager.myInfo);
        
        GridDiscretization grid = new GridDiscretization(terrain_manager.myInfo, 5,5, 1);
        astar = new RealAstar(grid);

        car.transform.position = new Vector3(220.0f, 0.5f, 230.0f);
        car.transform.rotation = Quaternion.identity;

        calculateDist();
        setTime();
    }

    private void setTime() {
        start_time = Time.time;
        completion_time = start_time - 1f;
    }

    private bool canCalc = false;
    public void calculateDist() {
        Debug.Log("calculating dist");
        canCalc = false;
        Vector3 start = getStartPos();
        Vector3 goal = targetHandler.getTarget().transform.position;

        astar.initAstar(start, goal);
        distance = astar.dist_astar();
        canCalc = true;
    }

    public float getRotation() {
        angle = targetHandler.angle;
        return angle;
    }
    
    public Vector3 getStartPos() {
        Vector3 newPos = targetHandler.getStartPos().transform.position;
        newPos.y = 0.5f;
        return newPos;
    }

    public bool canFetch () {
        return canCalc == true;
    }

    public bool hasFetched { get; set; } = false;
    public List<GameObject> getTargets() {
        List<GameObject> objects = new List<GameObject>();
        GameObject target = targetHandler.getTarget();

        objects.Add(target);
        hasFetched = true;
        return objects;
    }

    private StringBuilder builder = new StringBuilder();
    private void updateBuilder() {
        if (distance != 0) {
            Debug.Log(time + ":" + distance + ":" + angle);
            builder.Append(time + " " + distance + " " + angle + "\n");
        }
    }

    private void save() {
        DataSaver.save(builder);
    }

    private void tryToCalc() {
        if (canCalc)
        {
            calculateDist();
        }
        else
        {
          //  while (!canCalc) ;
        }
    }
    
    public bool reachedTarget { get; set; } = false;
    // Update is called once per frame
    void Update()
    {
        Time.timeScale = 25.0f;
        if (reachedTarget)
        {
            if (completion_time < start_time)
            {
                completion_time = Time.time - start_time;
                time = completion_time;
                updateBuilder();
            }

            Debug.Log("finished in : " + completion_time.ToString("n2") + "seconds!");
            if (!targetHandler.incrementAngle()) { //new target
                if (!targetHandler.incrementTarget()) { //new startpos
                    if (!targetHandler.incrementStartPos()) { //end life/gamee :)
                        save();
                        return; //finished
                    }
                } else if (canCalc)  {
                    calculateDist();
                }
            }
            reachedTarget = false;
            hasFetched = false;
            setTime();
        }
    }
}

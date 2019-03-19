using System.Collections;
using System.Collections.Generic;
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

    // Use this for initialization
    void Start()
    {
        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
        targetHandler = new TargetHandler_GenData(terrain_manager.myInfo);
        
        car.transform.position = new Vector3(220.0f, 0.5f, 230.0f);
        car.transform.rotation = Quaternion.identity;
        setTime();
    }

    private void setTime() {
        start_time = Time.time;
        completion_time = start_time - 1f;
    }

    public float getRotation() {
        return targetHandler.angle;
    }

    public bool canFetch () {
        Debug.Log("can fetch!");
        return targetHandler != null;
    }

    public bool hasFetched { get; set; } = false;
    public List<GameObject> getTargets() {
        List<GameObject> objects = new List<GameObject>();
        objects.Add(targetHandler.getTarget());
        hasFetched = true;
        return objects;
    }
    
    public bool reachedTarget { get; set; } = false;
    // Update is called once per frame
    void Update()
    {
        Time.timeScale = 5.0f;
        if (reachedTarget)
        {
            if (completion_time < start_time)
            {
                completion_time = Time.time - start_time;
            }
            Debug.Log("finished in : " + completion_time.ToString("n2") + "seconds!");
            if (!targetHandler.incrementAngle()) {
                if (!targetHandler.incrementTarget()) {
                    if (!targetHandler.incrementStartPos()) {
                        return; //finished
                    }
                }
            }
            reachedTarget = false;
            hasFetched = false;
            setTime();
        }
    }
}

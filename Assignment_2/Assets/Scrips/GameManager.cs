﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public GameObject terrain_manager_game_object;
    TerrainManager terrain_manager;

    public GameObject race_car;
    public GameObject turret;
    public int number_of_turrets = 10;
    public int number_of_extra_cars = 3;
    public int random_seed = 7;


    private float start_time;
    private float completion_time;

    public Text turret_text;

    public List<GameObject> turret_list;
    GameObject turret_clone;
    Destructable destructable_script;
    GatlingGun gatlinggun_script;
    public bool weak_turrets;
    public bool long_range_turrets;

    void Awake() {

        terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();

        race_car.transform.position = terrain_manager.myInfo.start_pos;
        race_car.transform.rotation = Quaternion.identity;

        Random.InitState(random_seed);
        for (int i = 0; i < number_of_turrets; i++) {
            Vector3 pos = terrain_manager.myInfo.GetRandomFreePos();
            pos.y = 2f;
            //turret_list.Add(Instantiate(turret, pos, Quaternion.identity));
            turret_clone = Instantiate(turret, pos, Quaternion.identity);
            destructable_script = (Destructable)turret_clone.GetComponent(typeof(Destructable));
            destructable_script.is_weak = weak_turrets;
            gatlinggun_script = (GatlingGun)turret_clone.GetComponent(typeof(GatlingGun));
            gatlinggun_script.is_long_range = long_range_turrets;
            turret_list.Add(turret_clone);
        }

        for (int i = 0; i < number_of_extra_cars; i++) {
            Vector3 pos = new Vector3(185f, 0, 135 + 10 * i);
            pos.y = 2f;
            turret_list.Add(Instantiate(race_car, pos, Quaternion.identity));
        }
    }

    // Use this for initialization
    void Start() {
        start_time = Time.time;
        completion_time = start_time - 1f;
    }

    // Update is called once per frame
    void Update() {
        Time.timeScale = 1.0f;
        turret_list.RemoveAll(item => item == null);
        turret_text.text = "Remaining turrets:" + turret_list.Count;

        if (turret_list.Count == 0) {
            if (completion_time < start_time) {
                completion_time = Time.time - start_time;

            }
            turret_text.text += " Mission Accomplished in " + completion_time.ToString("n2") + "seconds!";

        }
    }
}
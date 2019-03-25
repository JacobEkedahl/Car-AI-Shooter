

using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public abstract class MainCar {
    public MainCar(Coordinator coordinator, CarIndexAssign index_assigner, int my_index, CarController m_Car, TerrainManager terrain_manager, Transform car, int timer, bool go_forward, int coll_timer, bool go_back, TargetHandler target_handler, TerrainInfo info) {
        this.coordinator = coordinator;
        this.index_assigner = index_assigner;
        this.my_index = my_index;
        this.m_Car = m_Car;
        this.terrain_manager = terrain_manager;
        this.car = car;
        this.timer = timer;
        this.go_forward = go_forward;
        this.coll_timer = coll_timer;
        this.go_back = go_back;
        this.target_handler = target_handler;
        this.info = info;
    }

    //needed for follower car
    public Coordinator coordinator { get; set; }
    public CarIndexAssign index_assigner { get; set; }
    public int my_index { get; set; }

    //generell
    public CarController m_Car { get; set; }
    public TerrainManager terrain_manager { get; set; }
    public Transform car { get; set; }
    public int timer { get; set; } = 100;
    public bool go_forward { get; set; } = false;
    public int coll_timer { get; set; } = 100;
    public bool go_back { get; set; } = false;
    
    //leadercar
    public TargetHandler target_handler { get; set; }
    public TerrainInfo info { get; set; }

    //methods
    public abstract void go();
    public abstract void OnCollisionExit(Collision other);
    public abstract void OnCollisionStay(Collision other);

}
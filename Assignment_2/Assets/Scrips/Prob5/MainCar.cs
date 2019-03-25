

using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public abstract class MainCar {
    public MainCar(Coordinator coordinator, CarIndexAssign index_assigner, int my_index, CarController m_Car, Transform car, TargetHandler target_handler, TerrainInfo info) {
        this.coordinator = coordinator;
        this.index_assigner = index_assigner;
        this.my_index = my_index;
        this.m_Car = m_Car;
        this.car = car;
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
    public bool collision_handling {get; set;}
    //leadercar
    public TargetHandler target_handler { get; set; }
    public TerrainInfo info { get; set; }

    //methods
    public abstract void go();

    //used for car to know if it should go back or not
    protected bool normalRun() {
        if (collision_handling) {
            if (go_back) {
                go_back_routine(-1.0f);
            } else if (go_forward) {
                go_back_routine(1.0f);
            } else {
                return true;
            }

            return false;
        }
        return true;
    }

    //generic methods for all cars
    public void OnCollisionStay(Collision other) {
        if (coll_timer == 100) {
            if (other.gameObject.tag == "Player") {
                int choice = Random.Range(0, 2);
                if (choice == 0) {
                    go_back = true;
                } else {
                    go_forward = true;
                }
            } else if (is_coll_back()) {
                go_back = true;
            }
        }

        coll_timer--;
        if (coll_timer <= 0)
            coll_timer = 100;
    }

    public void OnCollisionExit(Collision other) {
        coll_timer = 100;
    }

    //private methods for collision handeling

    //Methods used for dealing with collisions --------------------------------
    private bool is_coll_back() {
        Vector3 offset_forward = car.forward * 3;
        Vector3 offset_side = car.right * 1;
        Vector3 left_pos = car.position + offset_forward + offset_side;
        Vector3 right_pos = car.position + offset_forward - offset_side;

        int layerMask = LayerMask.GetMask("CubeWalls");

        if ((Physics.Linecast(car.position, left_pos, layerMask)) ||
            (Physics.Linecast(car.position, right_pos, layerMask))) {
            return true;
        }
        return false;
    }

    private void go_back_routine(float dir) {
        if (timer > 0) {
            timer--;
            if(m_Car != null){
                m_Car.Move(0.0f, dir, dir, 0.0f);
            }
        } else {
            timer = 100;
            go_back = false;
            go_forward = false;
        }
    }




}
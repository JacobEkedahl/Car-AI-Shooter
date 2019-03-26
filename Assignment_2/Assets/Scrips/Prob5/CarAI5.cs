using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityStandardAssets.Vehicles.Car {
    [RequireComponent(typeof(CarController))]
    public class CarAI5 : MonoBehaviour {
        public GameObject terrain_manager_game_object;
        public GameObject cluster_manager_object;
        public GameObject index_assign_object;
        public GameObject[] friends;
        public GameObject[] enemies;
        public int my_index;
        
        MainCar car;

        //for init the car
        TargetHandler target_handler;
        CarController m_Car;
        TerrainManager terrain_manager;
        EnemyPlanner enemy_planner;
        Coordinator coordinator;
        LeaderManager leaderManager;

        private void Start() {
            //Cluster manager, car ask for manager for which targets to find
            target_handler = cluster_manager_object.GetComponent<TargetHandler>();
            friends = GameObject.FindGameObjectsWithTag("Player");

            // get the car controller
            m_Car = GetComponent<CarController>();
            terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
            enemy_planner = new EnemyPlanner();
            leaderManager = LeaderManager.getInstance();

            //for followers
            my_index = leaderManager.getID();
            coordinator = new Coordinator(5, 15, 0);
            
            //init my type of car
            if (my_index == leaderManager.leader) { 
                car = new LeaderCar(coordinator, m_Car, this.transform, target_handler, terrain_manager.myInfo);
            } else {
                car = new FollowerCar(my_index, coordinator, m_Car, this.transform, target_handler, terrain_manager.myInfo);
            }
        }

        private void tryBeLeader(){
            if (friends[leaderManager.leader-1] == null) {
                Debug.Log("leader is gone!");
                leaderManager.leader = my_index;
                car = new LeaderCar(coordinator, m_Car, this.transform, target_handler, terrain_manager.myInfo);
            }
        }

   
        private void FixedUpdate() {
            tryBeLeader();
            car.go();
        }

        private void OnCollisionExit(Collision other) {
            car.OnCollisionExit(other);
        }
        
        private void OnCollisionStay(Collision other) {
            car.OnCollisionStay(other);
        }
        
    }
}

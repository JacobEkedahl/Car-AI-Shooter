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

        private void Start() {
            //Cluster manager, car ask for manager for which targets to find
            TargetHandler target_handler = cluster_manager_object.GetComponent<TargetHandler>();

            // get the car controller
            CarController m_Car = GetComponent<CarController>();
            TerrainManager terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
            EnemyPlanner enemy_planner = new EnemyPlanner();

            friends = GameObject.FindGameObjectsWithTag("Player");
            enemies = GameObject.FindGameObjectsWithTag("Enemy");

            //for followers
            CarIndexAssign index_assigner = index_assign_object.GetComponent<CarIndexAssign>();
            my_index = index_assigner.get_my_index();
            Coordinator coordinator = new Coordinator(5, 5);

            if(is_leader()){
                car = new LeaderCar(coordinator, index_assigner, my_index, m_Car, this.transform, target_handler, terrain_manager.myInfo);
            } else {
                car = new FollowerCar(coordinator, index_assigner, my_index, m_Car, this.transform, target_handler, terrain_manager.myInfo);
            }
        }

        private bool is_leader(){
            for(int i = 0; i < friends.Length; i++){
                if(friends[i] != null && i == my_index){
                    return true;
                }
            }
            return false;
        }
        private bool peek(GameObject leader_car, float peek_distance){
            Vector3 look_ahead = leader_car.transform.position + leader_car.transform.forward * peek_distance;

            for(int i = 0; i < enemies.Length; i++){
                int layerMask = LayerMask.GetMask("CubeWalls");
                if (enemies[i] == null) continue;
                if (!Physics.Linecast(look_ahead, enemies[i].transform.position, layerMask)) {
                    Debug.DrawLine(look_ahead, enemies[i].transform.position, Color.cyan);
                    return true;
                }
            }
            return false;
        }


        private void FixedUpdate() {
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

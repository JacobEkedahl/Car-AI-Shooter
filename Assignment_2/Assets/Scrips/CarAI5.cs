using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car {
    [RequireComponent(typeof(CarController))]
    public class CarAI5 : MonoBehaviour {
        public GameObject terrain_manager_game_object;
        public GameObject cluster_manager_object;
        public GameObject index_assign_object;
        
        LeaderCar car;

        private void Start() {
            //Cluster manager, car ask for manager for which targets to find
            TargetHandler target_handler = cluster_manager_object.GetComponent<TargetHandler>();

            // get the car controller
            CarController m_Car = GetComponent<CarController>();
            TerrainManager terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();
            EnemyPlanner enemy_planner = new EnemyPlanner();

            //for followers
            CarIndexAssign index_assigner = index_assign_object.GetComponent<CarIndexAssign>();
            int my_index = index_assigner.get_my_index();
            Coordinator coordinator = new Coordinator();

            car = new LeaderCar(coordinator, index_assigner, my_index, m_Car, terrain_manager, this.transform, 100, false, 100, false, target_handler, terrain_manager.myInfo);
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

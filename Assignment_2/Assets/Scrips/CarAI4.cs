using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarAI4 : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        public GameObject terrain_manager_game_object;
        TerrainManager terrain_manager;

        public GameObject[] friends;
        public GameObject[] enemies;

        public GameObject index_assign_object;
        CarIndexAssign index_assigner;

        public int my_index;

        public Coordinator coordinator;

        private void Start()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
            terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager>();


            // note that both arrays will have holes when objects are destroyed
            // but for initial planning they should work
            friends = GameObject.FindGameObjectsWithTag("Player");
            enemies = GameObject.FindGameObjectsWithTag("Enemy");

            index_assigner = index_assign_object.GetComponent<CarIndexAssign>();
            my_index = index_assigner.get_my_index();
            coordinator = new Coordinator(45, 55, 10);
        }


        private void FixedUpdate()
        {
            Vector3 target_position = coordinator.get_target_position(friends[0], this.gameObject, my_index);

            List<float> car_input = get_car_input(target_position, friends[0]);
            float steering = car_input[0];
            float acceleration = car_input[1];
            float breaking = car_input[2];

            m_Car.Move(steering, acceleration, acceleration, breaking);

        }
        private List<float> get_car_input(Vector3 target, GameObject target_car)
        {
            Vector3 direction = (target - transform.position).normalized;
            List<float> car_input = new List<float>();
            float steering = (Vector3.Angle(direction, transform.forward));
            float acceleration = 1f;
            float breaking = 0f;

            if (steering >= 25f) {
                steering = 1.0f;
            } else {
                steering /= 25.0f;
            }

            bool is_to_the_right = Vector3.Dot(direction, transform.right) > 0f;
            bool is_to_the_front = Vector3.Dot(direction, transform.forward) > 0f;
            
            if(!is_to_the_front){
                direction = (target_car.transform.position - transform.position).normalized;
                is_to_the_right = Vector3.Dot(direction, transform.right) > 0f;
                if(!is_to_the_right) steering *= -1;
                if(m_Car.CurrentSpeed > 10) {
                    acceleration = 0;
                }
                car_input.Add(steering);
                car_input.Add(acceleration);
                car_input.Add(breaking);
                return car_input;
            }

            if (is_to_the_right && is_to_the_front)
            {
                //steering = 1f;
                acceleration = 1f;
            }
            else if (is_to_the_right && !is_to_the_front)
            {
                steering *= -1f;
                acceleration -= 1f;
            }
            else if (!is_to_the_right && is_to_the_front)
            {
                steering *= -1f;
                acceleration = 1f;
            }
            else if (!is_to_the_right && !is_to_the_front)
            {
                acceleration -= 1f;
            }

            if (m_Car.CurrentSpeed > 66) acceleration = 0;
            if (m_Car.CurrentSpeed > 40 && Vector3.Distance(target, transform.position) < 14) acceleration = 0;

            car_input.Add(steering);
            car_input.Add(acceleration);
            car_input.Add(breaking);

            return car_input;
        }
    }
    
}

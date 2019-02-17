using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car {
    [RequireComponent (typeof (CarController))]
    public class CarAI1 : MonoBehaviour {
        private CarController m_Car; // the car controller we want to use

        public GameObject terrain_manager_game_object;
        TerrainManager terrain_manager;
        EnemyPlanner enemy_planner;

        public GameObject[] friends;
        public List<GameObject> enemies;

        PathHandler pathHandler;
        List<Vector3> cluster_means;
        Cluster cluster;
        private List<Vector3> nodesToGoal = new List<Vector3> ();
        private int currIndex = 0;

        void OnDrawGizmos() {
            Debug.Log("Drawing gizmos...!");
            Gizmos.color = Color.blue;
            for(int i = 0; i< cluster_means.Count; i++){
                Gizmos.DrawSphere(cluster_means[i], 10);
            }
        }

        private void Start () {
            // get the car controller
            m_Car = GetComponent<CarController> ();
            terrain_manager = terrain_manager_game_object.GetComponent<TerrainManager> ();
            enemy_planner = new EnemyPlanner ();

            // note that both arrays will have holes when objects are destroyed
            // but for initial planning they should work
            friends = GameObject.FindGameObjectsWithTag ("Player");
            enemies = new List<GameObject> (GameObject.FindGameObjectsWithTag ("Enemy"));

            Cluster cluster = new Cluster(3, terrain_manager);
            cluster.run();
            this.cluster_means = cluster.cluster_means;
            Debug.Log(cluster_means[0]);
            Debug.Log(cluster_means[1]);
            Debug.Log(cluster_means[2]);

            // Plan your path here
            pathHandler = new PathHandler (terrain_manager);

            //retrieve the list of nodes from my position to next pos
            updatePath();
        }

        private List<float> get_car_input(Vector3 target_node){
            Vector3 direction = (target_node - transform.position).normalized;

            bool is_to_the_right = Vector3.Dot (direction, transform.right) > 0f;
            bool is_to_the_front = Vector3.Dot (direction, transform.forward) > 0f;

            float steering = (Vector3.Angle(direction, transform.forward) / 360) * m_Car.m_MaximumSteerAngle;
            float acceleration = 0;

            if (is_to_the_right && is_to_the_front) {
                //steering = 1f;
                acceleration = 1f;
            } else if (is_to_the_right && !is_to_the_front) {
                steering *= -1f;
                acceleration = -1f;
            } else if (!is_to_the_right && is_to_the_front) {
                steering *= -1f;
                acceleration = 1f;
            } else if (!is_to_the_right && !is_to_the_front) {
                //steering = 1f;
                acceleration = -1f;
            }

            if (m_Car.CurrentSpeed > 20) acceleration = 0;

            List<float> car_input = new List<float>();
            car_input.Add(steering);
            car_input.Add(acceleration);

            return car_input;
        }

        private void FixedUpdate () {
            Vector3 target_node = nodesToGoal[currIndex];

            while (Vector3.Distance (transform.position, target_node) < 3) {
                setNextTarget ();
                target_node = nodesToGoal[currIndex];
            }

            List<float> car_input = get_car_input(target_node);
            float steering = car_input[0];
            float acceleration = car_input[1];
            Debug.Log("steering: " + steering);
            Debug.Log("acceleration: " + acceleration);
            Debug.Log("acceleration input: " + m_Car.AccelInput);


            // this is how you access information about the terrain
            int i = terrain_manager.myInfo.get_i_index (transform.position.x);
            int j = terrain_manager.myInfo.get_j_index (transform.position.z);
            float grid_center_x = terrain_manager.myInfo.get_x_pos (i);
            float grid_center_z = terrain_manager.myInfo.get_z_pos (j);

            Debug.DrawLine (transform.position, new Vector3 (grid_center_x, 0f, grid_center_z));

            // this is how you control the car
            //Debug.Log("Steering:" + steering + " Acceleration:" + acceleration);
            m_Car.Move (steering, acceleration, acceleration, 0f);
            //m_Car.Move(0f, -1f, 1f, 0f);
        }

        private void setNextTarget () {
            if (currIndex < nodesToGoal.Count - 1)
                currIndex++;

            if (currIndex == nodesToGoal.Count - 1) {
                updatePath ();
            }
        }

        private void updatePath () {
            enemies = enemy_planner.remove_destroyed (enemies);
            GameObject target_enemy = enemy_planner.get_closest_object (enemies, transform.position);
            nodesToGoal = pathHandler.getPath (transform.position, target_enemy.transform.position);
            currIndex = 0;
            Debug.Log("enemy pos: " + target_enemy.transform.position + ", my pos: " + transform.position);
        }
    }
}
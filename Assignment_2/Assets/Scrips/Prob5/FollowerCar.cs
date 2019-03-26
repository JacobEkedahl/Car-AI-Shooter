using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class FollowerCar : MainCar {

    public GameObject[] friends;
    int my_index;
    public FollowerCar(int my_index, Coordinator coordinator, CarController m_Car, Transform car, TargetHandler target_handler, TerrainInfo info) : base(coordinator, m_Car, car, target_handler, info) {
        friends = GameObject.FindGameObjectsWithTag("Player");
        collision_handling = false;
        this.my_index = my_index;
    }

    public override void go() {
        Vector3 target_position = coordinator.get_target_position(friends[get_leader()], car.gameObject, my_index);

        List<float> car_input = get_car_input(target_position, friends[get_leader()]);
        float steering = car_input[0];
        float acceleration = car_input[1];
        float breaking = car_input[2];

        m_Car.Move(steering, acceleration, acceleration, breaking);
    }

    private List<float> get_car_input(Vector3 target, GameObject target_car) {
        Vector3 direction = (target - car.position).normalized;
        List<float> car_input = new List<float>();
        float steering = (Vector3.Angle(direction, car.forward));
        float acceleration = 1f;
        float breaking = 0f;

        if (steering >= 25f) {
            steering = 1.0f;
        } else {
            steering /= 25.0f;
        }

        bool is_to_the_right = Vector3.Dot(direction, car.right) > 0f;
        bool is_to_the_front = Vector3.Dot(direction, car.forward) > 0f;

        if (!is_to_the_front) {
            direction = (target_car.transform.position - car.position).normalized;
            is_to_the_right = Vector3.Dot(direction, car.right) > 0f;
            if (!is_to_the_right) steering *= -1;
            if (m_Car.CurrentSpeed > 10) {
                acceleration = 0;
            }
            car_input.Add(steering);
            car_input.Add(acceleration);
            car_input.Add(breaking);
            return car_input;
        }

        if (is_to_the_right && is_to_the_front) {
            //steering = 1f;
            acceleration = 1f;
        } else if (is_to_the_right && !is_to_the_front) {
            steering *= -1f;
            acceleration -= 1f;
        } else if (!is_to_the_right && is_to_the_front) {
            steering *= -1f;
            acceleration = 1f;
        } else if (!is_to_the_right && !is_to_the_front) {
            acceleration -= 1f;
        }

        CarController leader = friends[get_leader()].GetComponent<CarController>();

        if (m_Car.CurrentSpeed > 25) acceleration = 0;
        if (m_Car.CurrentSpeed > leader.CurrentSpeed && Vector3.Distance(target, car.position) < 10) acceleration = 0;

        car_input.Add(steering);
        car_input.Add(acceleration);
        car_input.Add(breaking);

        return car_input;
    }
    private int get_leader() {
        for (int i = 0; i < friends.Length; i++) {
            if (friends[i] != null) {
                return i;
            }
        }
        return -1;
    }
}
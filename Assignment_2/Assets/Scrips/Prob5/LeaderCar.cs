using UnityStandardAssets.Vehicles.Car;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeaderCar : MainCar {

    private AStar astar;
    private Transform car;
    public int currIndex = -1; //which node to target   
    public List<GameObject> enemies { get; set; }
    public List<Vector3> nodesToGoal { get; set; } = new List<Vector3>();
    public EnemyPlanner enemy_planner { get; set; }

    public LeaderCar(Coordinator coordinator, CarIndexAssign index_assigner, int my_index, CarController m_Car, Transform car, TargetHandler target_handler, TerrainInfo info) : base(coordinator, index_assigner, my_index, m_Car, car, target_handler, info) {
        enemies = new List<GameObject>();
        nodesToGoal = new List<Vector3>();
        enemy_planner = new EnemyPlanner();
        GridDiscretization grid = new GridDiscretization(info, 1, 1, 4);
        astar = new AStar(grid, true); //astar loads this grid into a internal voronoigrid, the targets are turrets
        this.car = car;
    }

    public override void go() {
        fetch_clusters();

        if (has_fetched) {
            runAstar();

            //Debug.Log("enemies: " + enemies.Count);

            List<float> car_input = get_car_input();
            if (car_input == null) {
                return;
            }

            float steering = car_input[0];
            float acceleration = car_input[1];
            float breaking = 0f;

            if (current_target == null) {
                replan();
            }

            if (normalRun()) {
                m_Car.Move(steering, acceleration, acceleration, 0f);
                if(peek(car, 10)){
                    Debug.Log("Positive peek!");
                    if(m_Car.CurrentSpeed > 6){
                        acceleration = 0f;
                        breaking = 1f;
                    }
                }
                m_Car.Move(steering, acceleration, acceleration, breaking);
            }
        }
    }
    private bool peek(Transform car, float peek_distance){
        Vector3 look_ahead = car.position + car.forward * peek_distance;

        for(int i = 0; i < enemies.Count; i++){
            int layerMask = LayerMask.GetMask("CubeWalls");
            if (enemies[i] == null) continue;
            if (!Physics.Linecast(look_ahead, enemies[i].transform.position, layerMask)) {
                Debug.DrawLine(look_ahead, enemies[i].transform.position, Color.cyan);
                return true;
            }
        }
        return false;
    }

    private List<float> get_car_input() {
        if (currIndex == -1 || nodesToGoal.Count == 0)
            return null;

        Vector3 target_node = nodesToGoal[currIndex];
        Vector3 direction = (target_node - car.position).normalized;

        bool is_to_the_right = Vector3.Dot(direction, car.right) > 0f;
        bool is_to_the_front = Vector3.Dot(direction, car.forward) > 0f;

        float steering = (Vector3.Angle(direction, car.forward));
        if (steering >= 25f) {
            steering = 1.0f;
        } else {
            steering /= 25.0f;
        }

        float acceleration = 0;

        if (is_to_the_right && is_to_the_front) {
            acceleration = 1f;
        } else if (is_to_the_right && !is_to_the_front) {
            steering *= -1f;
            acceleration = -1f;
        } else if (!is_to_the_right && is_to_the_front) {
            steering *= -1f;
            acceleration = 1f;
        } else if (!is_to_the_right && !is_to_the_front) {
            acceleration = -1f;
        }

        List<float> car_input = new List<float>();
        car_input.Add(steering);
        car_input.Add(acceleration);

        return car_input;
    }



    private bool can_update = true;
    private GameObject current_target;
    private GameObject get_next_target() {
        List<GameObject> possibleTargets = new List<GameObject>();
        GameObject next_target;

        for (int i = 0; i < enemies.Count; i++) {
            if (!(enemies[i] == null)) {
                possibleTargets.Add(enemies[i]);
            }
        }

        float minDistance = float.MaxValue;
        if (possibleTargets.Count == 2) {
            astar.initAstar(car.position, possibleTargets[0].transform.position);
            List<Vector3> path = astar.getPath();
            path = astar.reconstructPath(path);
            float dist_target_one = astar.dist_astar(path);

            astar.initAstar(car.position, possibleTargets[1].transform.position);
            List<Vector3> path2 = astar.getPath();
            path2 = astar.reconstructPath(path2);
            float dist_target_two = astar.dist_astar(path2);

            if (dist_target_one < dist_target_two) {
                return possibleTargets[0];
            } else {
                return possibleTargets[1];
            }

        } else if (possibleTargets.Count != 0) {
            return possibleTargets[0];
        }

        return null;
    }

    private List<GameObject> lineOfSight_enemies = new List<GameObject>();

    private void findFurthestTarget() {
        if (nodesToGoal.Count == 0) {
            return;
        }

        //sets target to the furthest visible node on path
        if (!car_can_see()) {
            //car has no vision to path
        }
    }

    private bool car_can_see() {
        Vector3 offset = car.right;
        Vector3 offset_backward = -car.forward * 2;
        Vector3 offset_forward = car.forward * 2;


        offset *= 2;
        Vector3 right_pos_forward = car.position + offset_forward + offset;
        Vector3 left_pos_forward = car.position + offset_forward + -offset;
        Vector3 right_pos_backward = car.position + offset_backward + offset;
        Vector3 left_pos_backward = car.position + offset_backward + -offset;


        for (int i = nodesToGoal.Count - 1; i >= 0; i--) {
            Vector3 left_other = nodesToGoal[i] + offset;
            Vector3 right_other = nodesToGoal[i] - offset;

            if (can_see(left_pos_forward, left_other) && can_see(right_pos_forward, left_other) &&
                can_see(left_pos_backward, left_other) && can_see(right_pos_backward, left_other) &&
                can_see(left_pos_forward, right_other) && can_see(right_pos_forward, right_other) &&
                can_see(left_pos_backward, right_other) && can_see(right_pos_backward, right_other)) {
                currIndex = i;
                return true;
            }
        }

        return false;
    }

    private bool can_see(Vector3 from, Vector3 other_pos) {
        Vector3 offset_right = car.right;
        Vector3 offset_top = car.forward;

        int layer_mask = LayerMask.GetMask("CubeWalls");
        if (!Physics.Linecast(from, other_pos, layer_mask)) {
            //Debug.DrawLine(from, other_pos, Color.green, 0.1f);
            return true;
        }
        return false;
    }

    private void load_lineOfSight() {
        lineOfSight_enemies.Clear();

        foreach (GameObject enemy in enemies) {
            if (enemy == null) {
                continue;
            }

            if (can_see(car.position, enemy.transform.position)) {
                lineOfSight_enemies.Add(enemy);
            }
        }
    }

    private void updatePath() {
        load_lineOfSight();
        current_target = get_next_target();
        if (current_target == null) {
            current_target = car.gameObject;
            Debug.Log("im current target!");
        }

        astar.initAstar(car.position, current_target.transform.position);
        currIndex = 0;

        //Debug.DrawLine(current_target.transform.position, car.position, Color.blue, 100);
        can_update = false;
    }

    private void replan() {
        enemies = enemy_planner.remove_destroyed(enemies);
        currIndex = 0;
        can_run = true;
        can_update = true;
        astar.openSet.Clear();
    }

    private void remove_close_box() {
        foreach (GameObject box in enemies) {
            if (box == null)
                continue;
            if (Vector3.Distance(car.position, box.transform.position) <= 6.0f) {
                Destroyer.destroyGameObject(box);
            }
        }
    }

    private bool can_run = true;
    private void runAstar() {
        remove_close_box();
        if (!can_update && can_run && enemies.Contains(current_target)) {
            nodesToGoal = astar.getPath(); //goal has already been loaded in updatePath
            if (nodesToGoal.Count > 0) {
                can_run = false;
            }
        } else if (can_update) {
            updatePath();
        } else {
            findFurthestTarget(); //normal running
        }
    }


    private bool has_fetched = false;
    private void fetch_clusters() {
        while (!target_handler.has_clustered) {
            //wait
        }

        //fetch clusters if not has fetched
        if (!has_fetched) {
            enemies = target_handler.getCluster();
            Debug.Log("fetching cluster");
            has_fetched = true;
        }
    }

    class Destroyer : MonoBehaviour {
        public static void destroyGameObject(GameObject objectToDestroy) {
            Destroy(objectToDestroy);
        }
    }

}
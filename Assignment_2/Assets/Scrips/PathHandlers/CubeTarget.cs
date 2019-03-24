using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scrips.PathHandlers {

    class CubeTarget {
        private GameObject cube;
        public List<GameObject> lineOfSight_enemies = new List<GameObject>();

        public CubeTarget(GameObject cube) {
            this.cube = cube;
            load_lineOfSight(cube);
        }

        public GameObject getCube() {
            return cube;
        }

        public List<GameObject> getLineofSightObj() {
            return this.lineOfSight_enemies;
        }

        public int getCount() {
            return lineOfSight_enemies.Count;
        }

        private void load_lineOfSight(GameObject cube) {
            List<GameObject> enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
            lineOfSight_enemies.Clear();

            foreach (GameObject enemy in enemies) {
                if (enemy == null) {
                    continue;
                }

                if (can_see(cube.transform.position, enemy.transform.position)) {
                    lineOfSight_enemies.Add(enemy);
                }
            }
        }

        private bool can_see(Vector3 from, Vector3 other_pos) {
            int layer_mask = LayerMask.GetMask("CubeWalls");
            if (!Physics.Linecast(from, other_pos, layer_mask)) {
                return true;
            }
            return false;
        }


    }
}

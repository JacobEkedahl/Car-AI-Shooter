using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scrips.PathHandlers
{
    class TargetFilter
    {
        public static List<GameObject> filter(List<CubeTarget> targets)
        {
            List<CubeTarget> chosenTargets = new List<CubeTarget>();
            List<GameObject> I = new List<GameObject>();
            List<GameObject> enemies = new List<GameObject>();

          //  while (enemies.Count == 0)
           // {
                Debug.Log("trying to find enemies");
                enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
          //  }
          //  Debug.Log("init size: " + enemies.Count);

            for (int i = 0; i < 10; i++) { 
          //while (!isEqual(I, enemies)) {
                int biggestDifference = int.MinValue;
                CubeTarget chosenTarget = null;
                foreach (CubeTarget target in targets)
                {
                    HashSet<GameObject> tmp = new HashSet<GameObject>(I);
                    int sizeBefore = tmp.Count;
                    tmp.UnionWith(target.getLineofSightObj());
                    int sizeAfter = tmp.Count;
                    int difference = sizeAfter - sizeBefore;

                    if (difference > biggestDifference)
                    {
                        biggestDifference = difference;
                        chosenTarget = target;
                    }
                }

                Debug.Log("biggestDiff: " + biggestDifference);

                chosenTargets.Add(chosenTarget);
                I.AddRange(chosenTarget.getLineofSightObj());
            }

            List<GameObject> result = new List<GameObject>();
            foreach (CubeTarget target in chosenTargets)
            {
                result.Add(target.getCube());
            }
            Debug.Log("returned result: " + result.Count);

            return result;
        }

        private static Boolean isEqual(List<GameObject> targets, List<GameObject> enemies)
        {
            HashSet<GameObject> tmpTargets = new HashSet<GameObject>(targets);
            HashSet<GameObject> tmpEnemies = new HashSet<GameObject>(enemies);
            if (tmpTargets.Count != tmpEnemies.Count)
            {
                Debug.Log("not equal: " + tmpTargets.Count + ":" + tmpEnemies.Count);
                return false;
            }

            Debug.Log("equal: " + tmpTargets.Count + ":" + tmpEnemies.Count);
            return true;
        }
        
    }
}

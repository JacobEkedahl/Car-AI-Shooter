using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scrips.GenData
{
    class DataGenerator
    {
        public static void generate(GridDiscretization grid, List<GameObject> objects)
        {
            List<Vector3> nodes = new List<Vector3>();
            NodesMap map = new NodesMap();

            AStar astar = new AStar(grid, false);

            foreach (GameObject obj in objects)
            {
                nodes.Add(obj.transform.position);
            }
            int nodesSize = nodes.Count;
            Vector3 forward = new Vector3(0.0f, 0.0f, 1.0f);

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (i != j)
                    { //start not equal to finish
                        astar.initAstar(nodes[i], nodes[j]);
                        List<Vector3> path = astar.getPath();
                        path = astar.reconstructPath(path);
                        float distance = astar.dist_astar(path);
                        float startAngle = astar.getAngleStart(path[1], forward);
                        float endAngle = astar.getAngleEnd(path[path.Count - 2], forward);
                        map.add(i, j, distance, startAngle, endAngle);
                    }
                }
            }

            DataSaver.saveMap(map);
        }
    }
}

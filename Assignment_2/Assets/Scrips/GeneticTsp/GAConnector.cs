using System.Collections.Generic;
using UnityEngine;

public class GAConnector {
    //will get a list of game objects and return the list which should be followed
    //problemtype = DataSaver.prob1 for example
    public static List<GameObject> getPath(List<GameObject> targets, string problem) {
        if (targets.Count < 3) {
            return targets;
        }

        NodesMap map = DataLoader.fetchMap(problem);
        GARunner runner = new GARunner(map);
        List<TspNode> cities = runner.getPath();
        for (int i = 0; i < cities.Count - 1; i++) {
            Debug.DrawLine(targets[cities[i].Position].transform.position, targets[cities[i + 1].Position].transform.position, Color.green, 100);
        }

        List<GameObject> enemies = new List<GameObject>();
        foreach (TspNode node in cities) {
            enemies.Add(targets[node.Position]);
        }

        return enemies;
    }

    public static List<float> distance_route(List<GameObject> original, List<GameObject> route, string problem) {
        NodesMap map = DataLoader.fetchMap(problem);
        List<float> result = new List<float>();

        for (int i = 0; i < route.Count - 1; i++) {
            int startIndex = original.IndexOf(route[i]);
            int nextIndex = original.IndexOf(route[i + 1]);
            float distance = map.getDistance(startIndex, nextIndex);
            result.Add(distance);
        }
        return result;
    }

    public static List<GameObject> logAndGetPath(GridDiscretization grid, List<GameObject> objects, string problem, bool know_turrets) {
        DataGenerator.generate(grid, objects, problem, know_turrets);
        return getPath(objects, problem);
    }
}

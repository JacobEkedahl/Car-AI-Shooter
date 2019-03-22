using Assets.Scrips.GenData;
using System.Collections.Generic;
using UnityEngine;

public class GAConnector
{
    //will get a list of game objects and return the list which should be followed
    //problemtype = DataSaver.prob1 for example
    public static List<GameObject> getPath(List<GameObject> targets, string problem)
    {
        NodesMap map = DataLoader.fetchMap(problem);
        GARunner runner = new GARunner(map, targets.Count);
        List<TspNode> cities = runner.getPath();
        for (int i = 0; i < cities.Count - 1; i++)
        {
            Debug.DrawLine(targets[cities[i].Position].transform.position, targets[cities[i + 1].Position].transform.position, Color.green, 100);
        }

        List<GameObject> enemies = new List<GameObject>();
        foreach (TspNode node in cities)
        {
            enemies.Add(targets[node.Position]);
        }

        return enemies;
    }

    public static List<GameObject> logAndGetPath(GridDiscretization grid, List<GameObject> objects, string problem)
    {
        DataGenerator.generate(grid, objects, problem);
        return getPath(objects, problem);
    }
}

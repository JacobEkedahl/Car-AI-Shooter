using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class TspFitness : IFitness {
    private Rect m_area;
    private static NodesMap map;
    public TspFitness(NodesMap incMap) {
        map = incMap;
        Cities = new List<TspNode>();

        var size = Camera.main.orthographicSize - 1;
        m_area = new Rect(-size, -size, size * 2, size * 2);

        foreach (int index in map.getNodes()) {
            Debug.Log("adding.. " + index);
            var city = new TspNode { Position = index };
            Cities.Add(city);
        }
    }

    public IList<TspNode> Cities { get; private set; }

    public double Evaluate(IChromosome chromosome) {
        var genes = chromosome.GetGenes();
        var distanceSum = 0.0;
        var lastCityIndex = Convert.ToInt32(genes[0].Value, CultureInfo.InvariantCulture);
        var citiesIndexes = new List<int>();
        var nextStartAngle = -500.0f;
        citiesIndexes.Add(lastCityIndex);

        // Calculates the total route distance.
        //needs to save the angle from previous end
        foreach (var g in genes) {
            var currentCityIndex = Convert.ToInt32(g.Value, CultureInfo.InvariantCulture);
            // Debug.Log("currentCityIndex: " + currentCityIndex);
            distanceSum += CalcDistanceTwoNodes(nextStartAngle, Cities[currentCityIndex], Cities[lastCityIndex]);
            lastCityIndex = currentCityIndex;
            citiesIndexes.Add(lastCityIndex);
            nextStartAngle = map.getEndAngle(Cities[currentCityIndex].Position, Cities[lastCityIndex].Position);
        }

        //  distanceSum += CalcDistanceTwoNodes(nextStartAngle, Cities[citiesIndexes.Last()], Cities[citiesIndexes.First()]);
        var fitness = 1.0 - (distanceSum / (Cities.Count * 1000.0));

        ((TspChromosome)chromosome).Distance = distanceSum;

        // There is repeated cities on the indexes?
        var diff = Cities.Count - citiesIndexes.Distinct().Count();

        if (diff > 0) {
            fitness /= diff;
        }

        if (fitness < 0) {
            fitness = 0;
        }

        return fitness;
    }

    private Vector2 GetCityRandomPosition() {
        return new Vector2(
            RandomizationProvider.Current.GetFloat(m_area.xMin, m_area.xMax + 1),
            RandomizationProvider.Current.GetFloat(m_area.yMin, m_area.yMax + 1));
    }

    private static double CalcDistanceTwoNodes(float startAngle, TspNode one, TspNode two) {
        //if (startAngle < -400.0f)
        //{
        return map.getTime(one.Position, two.Position);
        //}
        //  return map.getTime(startAngle, one.Position, two.Position);
    }
}

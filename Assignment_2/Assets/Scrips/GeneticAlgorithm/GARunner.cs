

using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Threading;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GARunner
{
    //TspFitness have the internal implementation of distance. It needs a NodesMap.
    //TspCity should be a integer
    //This class should have the same functionality as GAController but return the List of GameObjects to navigate to in order
    private GeneticAlgorithm m_ga;
    private Thread m_gaThread;
    private NodesMap map;
    private int noNodes;

    public GARunner(NodesMap map)
    {
        this.map = map;
        this.noNodes = map.getNodes().Count;
        Debug.Log("no nodes: " + noNodes);
    }
    
    public List<TspNode> getPath()
    {
        var fitness = new TspFitness(map);
        var chromosome = new TspChromosome(noNodes);

        // This operators are classic genetic algorithm operators that lead to a good solution on TSP,
        // but you can try others combinations and see what result you get.
        var crossover = new OrderedCrossover();
        var mutation = new ReverseSequenceMutation();
        var selection = new RouletteWheelSelection();
        var population = new Population(50, 100, chromosome);

        m_ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
        m_ga.Termination = new FitnessStagnationTermination();

        // The fitness evaluation of whole population will be running on parallel.
        m_ga.TaskExecutor = new ParallelTaskExecutor
        {
            MinThreads = 100,
            MaxThreads = 200
        };

        // Everty time a generation ends, we log the best solution.
        m_ga.GenerationRan += delegate
        {
            var distance = ((TspChromosome)m_ga.BestChromosome).Distance;
            Debug.Log($"Generation: {m_ga.GenerationsNumber} - Distance: ${distance}");
        };

        // Starts the genetic algorithm in a separate thread.
        m_gaThread = new Thread(() => m_ga.Start());
        m_gaThread.Start();
        m_gaThread.Join();

        return findBestResult();
    }

    private List<TspNode> findBestResult()
    {
        List<TspNode> path = new List<TspNode>();
        var c = m_ga.Population.CurrentGeneration.BestChromosome as TspChromosome;
        if (c != null)
        {
            var genes = c.GetGenes();
            var cities = ((TspFitness)m_ga.Fitness).Cities;

            for (int i = 0; i < genes.Length; i++)
            {
                var city = cities[(int)genes[i].Value];
                Debug.Log("city " + i + ": " + city.Position);
                path.Add(city);
            }

            //if you include this you also have to include the same commented out lines in tspfitness
         //   var firstCity = cities[(int)genes[0].Value];
          //  path.Add(firstCity);
        }

        return path;
    }

    private void OnDestroy()
    {
        // When the script is destroyed we stop the genetic algorithm and abort its thread too.
        m_ga.Stop();
        m_gaThread.Abort();
    }
}
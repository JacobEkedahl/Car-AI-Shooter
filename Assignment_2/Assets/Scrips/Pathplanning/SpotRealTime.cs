using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotRealTime : Spot {
    public SpotRealTime(float x, float z, int i, int j, bool wall) : base(x, z, i, j, wall)
    {
    }

    public SpotRealTime(float x, float z, int i, int j, bool wall, float value) : base(x, z, i, j, wall, value)
    {
    }

    public override void addNeighbors(Spot[,] grid)
    {
        neighbors = new List<Spot>();

        int o1 = 3;
        int o2 = 1;
        //corners
        if (this.i >= o1 && this.j >= o2)
        {
            neighbors.Add(grid[this.i - o1, this.j - o2]);
        }
        if (this.i > o2 && this.j > o1)
        {
            neighbors.Add(grid[this.i - o2, this.j - o1]);
        }

        // Debug.Log("i: " + i + ", j: " + j);
        if (this.i > o1 && this.j + o2 < grid.GetLength(1) - 1)
        {
            neighbors.Add(grid[this.i - o1, this.j + o2]);
        }
        if (this.i + o2 < grid.GetLength(0) - 1 && this.j > o1)
        {
            neighbors.Add(grid[this.i + o2, this.j - o1]);
        }

        if (this.i >= o2 && this.j + o1 < grid.GetLength(1) - 1)
        {
            neighbors.Add(grid[this.i - o2, this.j + o1]);
        }
        if (this.i + o1 < grid.GetLength(0) - 1 && this.j > o2)
        {
            neighbors.Add(grid[this.i + o1, this.j - o2]);
        }

        if (this.i + o1 < grid.GetLength(0) - 1 && this.j + o2 < grid.GetLength(1) - 1)
        {
            neighbors.Add(grid[this.i + o1, this.j + o2]);
        }
        if (this.i + o2 < grid.GetLength(0) - 1 && this.j + o1 < grid.GetLength(1) - 1)
        {
            neighbors.Add(grid[this.i + o2, this.j + o1]);
        }
    }
}
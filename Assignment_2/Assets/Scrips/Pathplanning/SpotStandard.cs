using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotStandard : Spot {
    public SpotStandard(float x, float z, int i, int j, bool wall) : base(x, z, i, j, wall)
    {
    }

    public SpotStandard(float x, float z, int i, int j, bool wall, float value) : base(x, z, i, j, wall, value)
    {
    }

    public override void addNeighbors(Spot[,] grid)
    {
        neighbors = new List<Spot>();

        if (this.i < grid.GetLength(0) - 1)
        {
            neighbors.Add(grid[this.i + 1, this.j]);
        }

        if (this.i > 0)
        {
            neighbors.Add(grid[this.i - 1, this.j]);
        }

        if (this.j < grid.GetLength(1) - 1)
        {
            neighbors.Add(grid[this.i, this.j + 1]);
        }

        if (this.j > 0)
        {
            neighbors.Add(grid[this.i, this.j - 1]);
        }

        //corners
        if (this.i > 0 && this.j > 0)
        {
            neighbors.Add(grid[this.i - 1, this.j - 1]);
        }

        if (this.i < grid.GetLength(0) - 1 && this.j > 0)
        {
            neighbors.Add(grid[this.i + 1, this.j - 1]);
        }

        if (this.i > 0 && this.j < grid.GetLength(1) - 1)
        {
            neighbors.Add(grid[this.i - 1, this.j + 1]);
        }

        if (this.i < grid.GetLength(0) - 1 && this.j < grid.GetLength(1) - 1)
        {
            neighbors.Add(grid[this.i + 1, this.j + 1]);
        }
    }
}
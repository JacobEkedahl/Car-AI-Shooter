using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot {
    public Vector3 pos { get; set; }
    public float f { get; set; }
    public float g { get; set; }
    public float h { get; set; }
    public int i { get; set; }
    public int j { get; set; }
    public List<Spot> neighbors { get; set; }
    public Spot previous { get; set; }
    public bool wall { get; set; }
    public float value { get; set; }

    public Spot (float x, float z, int i, int j, bool wall, float value) {
        previous = null;
        this.i = i;
        this.j = j;
        f = 0.0f;
        g = 1.0f;
        h = 0.0f;
        pos = new Vector3 (x, 0.0f, z);
        this.wall = wall;
        this.value = value;
    }

    public void clear() {
        this.g = 1.0f;
        this.f = 0.0f;
        this.h = 0.0f;
        previous = null;
    }

    public void addNeighbors (Spot[, ] grid) {
        neighbors = new List<Spot> ();
        /*
        if (this.i < grid.GetLength (0) - 1) {
            neighbors.Add (grid[this.i + 1, this.j]);
        }

        if (this.i > 0) {
            neighbors.Add (grid[this.i - 1, this.j]);
        }

        if (this.j < grid.GetLength (1) - 1) {
            neighbors.Add (grid[this.i, this.j + 1]);
        }

        if (this.j > 0) {
            neighbors.Add (grid[this.i, this.j - 1]);
        }*/

        int o1 = 3;
        int o2 = 1;
        //corners
        if (this.i >= o1 && this.j >= o2) {
            neighbors.Add (grid[this.i - o1, this.j - o2]);
        }
        if (this.i > o2 && this.j > o1) {
            neighbors.Add (grid[this.i - o2, this.j - o1]);
        }

       // Debug.Log("i: " + i + ", j: " + j);
        if (this.i >o1 && this.j  + o2 < grid.GetLength (1) - 1) {
            neighbors.Add (grid[this.i - o1, this.j + o2]);
        }
        if (this.i + o2 < grid.GetLength (0) - 1 && this.j > o1) {
            neighbors.Add (grid[this.i + o2, this.j - o1]);
        }

        if (this.i >= o2 && this.j + o1 < grid.GetLength (1) - 1) {
            neighbors.Add (grid[this.i - o2, this.j + o1]);
        }
        if (this.i+ o1 < grid.GetLength (0) - 1 && this.j > o2) {
            neighbors.Add (grid[this.i + o1, this.j - o2]);
        }

        if (this.i + o1 < grid.GetLength (0) - 1 && this.j + o2 < grid.GetLength (1) - 1) {
            neighbors.Add (grid[this.i + o1, this.j + o2]);
        }
        if (this.i + o2 < grid.GetLength (0) - 1 && this.j + o1 < grid.GetLength (1) - 1) {
            neighbors.Add (grid[this.i + o2, this.j + o1]);
        }
    }

    public void Draw (Color col) {
        if (this.wall) {
            col = Color.black;
        }
        float offset = 0.1f;
        float size = 0.8f;

        Vector3 orig = new Vector3 (pos.x + 0.1f, pos.y, pos.z + 0.1f);
        Vector3 otherPos = new Vector3 (orig.x + size, orig.y, orig.z + 0.0f);
        Debug.DrawLine (orig, otherPos, col, 0);
        orig = otherPos;

        otherPos = new Vector3 (orig.x + 0.0f, orig.y, orig.z + size);
        Debug.DrawLine (orig, otherPos, col, 0);

        orig = pos;
        otherPos = new Vector3 (orig.x + 0.0f, orig.y, orig.z + size);
        Debug.DrawLine (orig, otherPos, col, 0);
        orig = otherPos;

        otherPos = new Vector3 (orig.x + size, orig.y, orig.z + 0);
        Debug.DrawLine (orig, otherPos, col, 0);

    }
}
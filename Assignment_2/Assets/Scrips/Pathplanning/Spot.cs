using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spot {
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


    public abstract void addNeighbors(Spot[,] grid);

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

    public Spot(float x, float z, int i, int j, bool wall)
    {
        previous = null;
        this.i = i;
        this.j = j;
        f = 0.0f;
        g = 1.0f;
        h = 0.0f;
        pos = new Vector3(x, 0.0f, z);
        this.wall = wall;
    }

    public void clear() {
        this.g = 1.0f;
        this.f = 0.0f;
        this.h = 0.0f;
        previous = null;
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
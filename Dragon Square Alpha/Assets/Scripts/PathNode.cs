using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    //private Grid<PathNode> grid;
    public int x, y;

    public int gCost, hCost, fCost;

    public PathNode previousNode;

    public bool walkable;

    public List<PathNode> neighbourList;

    public PathNode(int x, int y, bool walkable = true)
    {
        //this.grid = grid;
        this.x = x;
        this.y = y;
        this.walkable = walkable;
    }

    public void CalculateFCost() => fCost = gCost + hCost;
}


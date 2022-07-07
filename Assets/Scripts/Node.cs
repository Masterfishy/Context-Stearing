using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Tile tile;
    public Vector3Int mapPosition;

    public int gCost; // Traveling to this node cost
    public int hCost; // Node cost (The heuristic)

    public Node parent;

    public Node(Tile newTile, Vector3Int newPosition)
    {
        tile = newTile;
        mapPosition = newPosition;
    }

    public int Cost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public bool Equals(Node other)
    {
        return mapPosition == other.mapPosition;
    }
}

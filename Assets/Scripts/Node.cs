using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public Tile tile;
    public Vector3Int mapPosition;

    public int gCost; // Traveling to this node cost
    public int hCost; // Node cost (The heuristic)

    public Node parent;

    private int heapIndex;

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

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    /// <summary>
    /// Compares two nodes based on their costs.
    /// </summary>
    /// <remarks>
    /// Uses hCost for tie breakers.
    /// </remarks>
    /// <param name="other">The other node to compare this node to</param>
    /// <returns>Return 1 if the current item has higher priority than the one we are comparing to.</returns>
    public int CompareTo(Node other)
    {
        int compare = Cost.CompareTo(other.Cost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }

        return -compare;
    }
}

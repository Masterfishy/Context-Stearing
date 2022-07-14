using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for pathfinding.
/// </summary>
public class Pathfinding : MonoBehaviour
{
    public int diagonalMoveCost = 14;
    public int adjacentMoveCost = 10;

    /// <summary>
    /// Starts a coroutine to find a path from start to target.
    /// </summary>
    /// <param name="start">The starting position</param>
    /// <param name="target">The target position</param>
    public void StartFindPath(Vector3 start, Vector3 target)
    {
        StartCoroutine(FindPath(start, target));
    }

    /// <summary>
    /// Find a path between the starting position and target position using
    /// the A* algorithm.
    /// </summary>
    /// <param name="_startPos">The starting position</param>
    /// <param name="_targetPos">The target position</param>
    private IEnumerator FindPath(Vector3 _startPos, Vector3 _targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = MapManager.Instance.GetNodeFromPoint(MapManager.Instance.WorldToMapPoint(_startPos));
        Node targetNode = MapManager.Instance.GetNodeFromPoint(MapManager.Instance.WorldToMapPoint(_targetPos));

        //Debug.Log($"{gameObject.name} | Start Pos: {startNode.mapPosition} | Target Pos: {targetNode.mapPosition}");

        Heap<Node> openSet = new Heap<Node>(MapManager.Instance.MapSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Push(startNode);

        while (openSet.Count > 0)
        {
            // Find the cheapest node to travel to first
            Node _currentNode = openSet.Pop();
            closedSet.Add(_currentNode);

            // If we reach the target
            //Debug.Log($"{gameObject.name} | Are {_currentNode.mapPosition} and {targetNode.mapPosition} equal? {_currentNode.Equals(targetNode)}");
            if (_currentNode.Equals(targetNode))
            {
                pathSuccess = true;
                break;
            }

            // Search for the next best neighbor
            //Debug.Log($"{gameObject.name} | Finding neighbors...");
            foreach (Node _neighbor in MapManager.Instance.GetNeighbors(_currentNode))
            {
                if (_neighbor.tile == TileType.Wall || closedSet.Contains(_neighbor))
                {
                    continue;
                }

                int _neighborCost = _currentNode.gCost + NodeDistanceCost(_currentNode, _neighbor);
                if (_neighborCost < _neighbor.gCost || !openSet.Contains(_neighbor))
                {
                    _neighbor.gCost = _neighborCost;
                    _neighbor.hCost = NodeDistanceCost(_neighbor, targetNode); // Maybe change to use Vector3Int.Distance
                    _neighbor.parent = _currentNode;

                    if (!openSet.Contains(_neighbor))
                    {
                        openSet.Push(_neighbor);
                    }
                }
            }

        }
        
        yield return null;

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        PathRequestManager.Instance.FinishedProcessingPath(waypoints, pathSuccess);
    }

    /// <summary>
    /// Retraces the path from the start node to the end node by following the parents from the end node.
    /// </summary>
    /// <param name="startNode">The starting node</param>
    /// <param name="endNode">The ending node</param>
    /// <returns>A list of Vector3s from start to end.</returns>
    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
    }

    private Vector3[] GeneratePath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        
        for(int i = 0; i < path.Count; i++)
        {
            waypoints.Add(path[i].mapPosition);
        }

        return waypoints.ToArray();
    }

    /// <summary>
    /// Simplifies a path of nodes by removing nodes which do not change direction.
    /// </summary>
    /// <param name="path">The path to simplify</param>
    /// <returns>A simplified path.</returns>
    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2 directionNew = new Vector2(path[i].mapPosition.x - path[i + 1].mapPosition.x, 
                                               path[i].mapPosition.y - path[i + 1].mapPosition.y);
            if (directionNew != directionOld)
            {
                waypoints.Add(new Vector3(path[i].mapPosition.x + 0.5f, path[i].mapPosition.y + 0.5f));
            }

            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    /// <summary>
    /// Get euclidean distance cost between two nodes.
    /// </summary>
    /// <param name="nodeA">The starting node</param>
    /// <param name="nodeB">The ending node</param>
    /// <returns>The distance cost for traveling from one node to the next.</returns>
    private int NodeDistanceCost(Node nodeA, Node nodeB)
    {
        int _distX = Mathf.Abs(nodeA.mapPosition.x - nodeB.mapPosition.x);
        int _distY = Mathf.Abs(nodeA.mapPosition.y - nodeB.mapPosition.y);

        if (_distX > _distY)
        {
            return diagonalMoveCost * _distY + adjacentMoveCost * (_distX - _distY);
        }

        return diagonalMoveCost * _distX + adjacentMoveCost * (_distY - _distX);
    }
}

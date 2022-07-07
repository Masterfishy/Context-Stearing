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

    private List<Vector3Int> path;

    private void Awake()
    {
        path = new List<Vector3Int>();
    }

    private void Update()
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], Color.blue);
        }
    }

    /// <summary>
    /// Find a path between the starting position and target position using
    /// the A* algorithm.
    /// </summary>
    /// <param name="_startPos">The starting position</param>
    /// <param name="_targetPos">The target position</param>
    public void FindPath(Vector3 _startPos, Vector3 _targetPos)
    {
        Node startNode = MapManager.Instance.GetNodeFromPoint(MapManager.Instance.WorldToMapPoint(_startPos));
        Node targetNode = MapManager.Instance.GetNodeFromPoint(MapManager.Instance.WorldToMapPoint(_targetPos));

        //Debug.Log($"Start Pos: {startNode.mapPosition} | Target Pos: {targetNode.mapPosition}");

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node _currentNode = openSet[0];

            // Find the cheapest node to travel to first
            //Debug.Log("Finding cheapest node...");
            for (int _i = 1; _i < openSet.Count; _i++)
            {
                Node _node = openSet[_i];

                if (_node.Cost < _currentNode.Cost ||
                    _node.Cost == _currentNode.Cost && _node.hCost < _currentNode.hCost)
                {
                    _currentNode = openSet[_i];
                }
            }

            openSet.Remove(_currentNode);
            closedSet.Add(_currentNode);

            // If we reach the target
            //Debug.Log($"Are {_currentNode.mapPosition} and {targetNode.mapPosition} equal? {_currentNode.Equals(targetNode)}");
            if (_currentNode.Equals(targetNode))
            {
                SetPath(_currentNode);
                return; // TODO we don't want to stop :) good luck with that ass hat
            }

            // Search for the next best neighbor
            //Debug.Log("Finding neighbors...");
            foreach (Node _neighbor in MapManager.Instance.GetNeighbors(_currentNode))
            {
                if (_neighbor.tile == Tile.Wall || closedSet.Contains(_neighbor))
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
                        openSet.Add(_neighbor);
                    }
                }
            }
        }

        //Debug.Log("Fuck, no path...");
    }

    private void SetPath(Node node)
    {
        Node _currentNode = node;

        path.Clear();

        while (_currentNode != null)
        {
            path.Add(_currentNode.mapPosition);

            _currentNode = _currentNode.parent;
        }
    }

    /// <summary>
    /// Get euclidean distance cost between two nodes.
    /// </summary>
    /// <param name="nodeA">The starting node.</param>
    /// <param name="nodeB">The ending node.</param>
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

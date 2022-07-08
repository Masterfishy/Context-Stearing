using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : Singleton<MapManager>
{
    [Header("World Values")]
    public Vector2 worldSize;

    [Header("Tilemaps")]
    public Tilemap groundMap;
    public Tilemap wallMap;

    [Header("Tile Time Bitches")]
    public TileBase groundTile;
    public TileBase wallTile;

    private Dictionary<Vector3Int, Node> m_worldMap;

    public int MapSize
    {
        get
        {
            return (int)(worldSize.x * worldSize.y);
        }
    }

    public Node GetNodeFromPoint(Vector3Int point)
    {
        if (m_worldMap.ContainsKey(point))
        {
            return m_worldMap[point];
        }

        return null;
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        Vector3Int _locus = node.mapPosition;
        Vector3Int _searchPoint = Vector3Int.zero;

        for (int x = _locus.x - 1; x <= _locus.x + 1; x++)
        {
            for (int y = _locus.y - 1; y <= _locus.y + 1; y++)
            {
                if (x == _locus.x && y == _locus.y)
                {
                    continue;
                }

                _searchPoint.x = x;
                _searchPoint.y = y;

                if (m_worldMap.TryGetValue(_searchPoint, out Node _node))
                {
                    switch (_node.tile)
                    {
                        case Tile.Ground:
                            neighbors.Add(_node);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        return neighbors;
    }

    public Vector3Int WorldToMapPoint(Vector3 position)
    {
        return groundMap.WorldToCell(position);
    }

    private void Awake()
    {
        m_worldMap = new Dictionary<Vector3Int, Node>();

        Vector3Int _placePosition = Vector3Int.zero;
        for (int _x = -(int)worldSize.x / 2; _x < (int)worldSize.x / 2; _x++)
        {
            for (int _y = -(int)worldSize.y / 2; _y < (int)worldSize.y / 2; _y++)
            {
                _placePosition.x = _x;
                _placePosition.y = _y;
                AddTile(_placePosition, Tile.Ground);
            }
        }

        Vector3Int[] _pointsToPlace = new Vector3Int[]{
            new Vector3Int(0, 4, 0),
            new Vector3Int(0, 3, 0),
            new Vector3Int(0, 2, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, -3, 0),
            new Vector3Int(4, 0, 0),
        };

        foreach (Vector3Int pos in _pointsToPlace)
        {
            AddTile(pos, Tile.Wall);
        }
    }

    private void AddTile(Vector3Int position, Tile tile)
    {
        if (m_worldMap.ContainsKey(position))
        {
            RemoveTile(position);
        }

        Node _newNode = new Node(tile, position);
        m_worldMap.Add(position, _newNode);

        switch(tile)
        {
            case Tile.Ground:
                groundMap.SetTile(position, groundTile);
                break;
            case Tile.Wall:
                wallMap.SetTile(position, wallTile);
                break;
            default:
                Debug.Log("That aint a tile type BITCH!");
                break;
        }
    }

    private void RemoveTile(Vector3Int position)
    {
        if (!m_worldMap.ContainsKey(position))
        {
            return;
        }

        Tile tile = m_worldMap[position].tile;
        m_worldMap.Remove(position);

        switch (tile)
        {
            case Tile.Ground:
                groundMap.SetTile(position, null);
                break;
            case Tile.Wall:
                wallMap.SetTile(position, null);
                break;
            default:
                Debug.Log("That aint a tile type BITCH!");
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, worldSize);
    }
}
public enum Tile
{
    Ground,
    Wall
}

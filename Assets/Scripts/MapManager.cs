using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : Singleton<MapManager>
{
    [Header("Tilemaps")]
    public Tilemap groundMap;
    public Tilemap wallMap;
    public Tilemap makerMap;

    [Header("Tile Time Bitches")]
    public TileBase groundTile;
    public TileBase wallTile;

    private Dictionary<Vector3Int, Node> m_worldMap;
    private BoundsInt mapBounds;

    public int MapSize
    {
        get
        {
            return mapBounds.size.x * mapBounds.size.y;
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
                        case TileType.Ground:
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

        // Use the map maker tile map to make the world
        mapBounds = makerMap.cellBounds;
        Vector3Int _searchPosition = Vector3Int.zero;

        for (int _x = mapBounds.xMin; _x < mapBounds.xMax; _x++)
        {
            for (int _y = mapBounds.yMin; _y < mapBounds.yMax; _y++)
            {
                _searchPosition.x = _x;
                _searchPosition.y = _y;

                TileBase _tileBase = makerMap.GetTile(_searchPosition);
                TileType _tileType = TileBaseToTileType(_tileBase);

                AddTile(_searchPosition, _tileType);
            }
        }

        makerMap.enabled = false;
    }

    private void AddTile(Vector3Int position, TileType tile)
    {
        if (m_worldMap.ContainsKey(position))
        {
            RemoveTile(position);
        }

        Node _newNode = new Node(tile, position);
        m_worldMap.Add(position, _newNode);

        switch(tile)
        {
            case TileType.Ground:
                groundMap.SetTile(position, groundTile);
                break;
            case TileType.Wall:
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

        TileType tile = m_worldMap[position].tile;
        m_worldMap.Remove(position);

        switch (tile)
        {
            case TileType.Ground:
                groundMap.SetTile(position, null);
                break;
            case TileType.Wall:
                wallMap.SetTile(position, null);
                break;
            default:
                Debug.Log("That aint a tile type BITCH!");
                break;
        }
    }

    private TileType TileBaseToTileType(TileBase tileBase)
    {
        if (tileBase == groundTile)
        {
            return TileType.Ground;
        }

        if (tileBase == wallTile)
        {
            return TileType.Wall;
        }

        return TileType.Default;
    }
}
public enum TileType
{
    Default,
    Ground,
    Wall
}

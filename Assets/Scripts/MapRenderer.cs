using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] Tilemap roomTilemap;
    [SerializeField] Tilemap backgroundTilemap;
    [SerializeField] RuleTile wallTile;
    [SerializeField] RuleTile backgroundTile;

    MapGenerator mapGenerator;

    void Awake()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    void Start()
    {
        MapGenerator.CellType[,] mapData = mapGenerator.GetMapData();

        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0; j < mapData.GetLength(1); j++)
            {
                backgroundTilemap.SetTile(new Vector3Int(i, j, 0), backgroundTile);
            }
        }

        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0; j < mapData.GetLength(1); j++)
            {
                if (mapData[i,j] == MapGenerator.CellType.Wall)
                {
                    roomTilemap.SetTile(new Vector3Int(i, j, 0), wallTile);
                }
            }
        }
    }
}

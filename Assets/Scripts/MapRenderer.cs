using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] Tilemap roomTilemap;
    [SerializeField] Tilemap backgroundTilemap;

    [SerializeField] RuleTile wallTile;
    [SerializeField] RuleTile backgroundTile;
    [SerializeField] RuleTile pathTile;

    MapGenerator mapGenerator;
    MazeGenerator mazeGenerator;

    void Awake()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        mazeGenerator = FindObjectOfType<MazeGenerator>();
    }

    void Start()
    {
        DrawNewMap();
    }

    public void DrawNewMap()
    {
        backgroundTilemap.ClearAllTiles();
        roomTilemap.ClearAllTiles();

        MapGenerator.CellType[,] mapData = mapGenerator.GenerateNewMap();

        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0; j < mapData.GetLength(1); j++)
                backgroundTilemap.SetTile(new Vector3Int(i, j, 0), backgroundTile);
        }

        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0; j < mapData.GetLength(1); j++)
            {
                if (mapData[i,j] == MapGenerator.CellType.Wall)
                    roomTilemap.SetTile(new Vector3Int(i, j, 0), wallTile);
            }
        }

        int[] shortestPath = mapGenerator.FindShortestPath();
        for (int i = 0; i < shortestPath.Length; i++)
            roomTilemap.SetTile(new Vector3Int(i * 2, shortestPath[i], 0), pathTile);
        
        for (int i = 0; i < shortestPath.Length - 1; i++)
        {
            int start = System.Math.Min(shortestPath[i], shortestPath[i + 1]);
            int end = System.Math.Max(shortestPath[i], shortestPath[i + 1]);
            for (int j = start; j <= end; j++)
                roomTilemap.SetTile(new Vector3Int(i * 2 + 1, j, 0), pathTile);
        }
    }

    public void DrawNewMaze()
    {
        backgroundTilemap.ClearAllTiles();
        roomTilemap.ClearAllTiles();

        bool[,] mapData = mazeGenerator.GenerateNewMaze();

        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0; j < mapData.GetLength(1); j++)
                backgroundTilemap.SetTile(new Vector3Int(i, j, 0), backgroundTile);
        }

        for (int i = 0; i < mapData.GetLength(0); i++)
        {
            for (int j = 0; j < mapData.GetLength(1); j++)
            {
                if (mapData[i,j] == false)
                    roomTilemap.SetTile(new Vector3Int(i, j, 0), wallTile);
            }
        }

        (int,int)[] mazeSolution = mazeGenerator.SolveMaze();
        foreach((int,int) item in mazeSolution)
        {
            roomTilemap.SetTile(new Vector3Int(item.Item1, item.Item2, 0), pathTile);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] int mapSize = 51;
    [SerializeField] int minNumberOfWallPaths = 2;
    [SerializeField] int maxNumberOfWallPaths = 7;

    public enum CellType
    {
        Empty,
        Wall,
        WallExit,
        MapStart,
        MapExit
    }
    CellType[,] mapCells;

    public CellType[,] GetMapData()
    {
        return mapCells;
    }

    void Start()
    {
        GenerateNewMap();
    }

    void GenerateNewMap()
    {
        mapCells = new CellType[mapSize, mapSize];

        // Add outer walls
        for (int i = 0; i < mapSize; i++)
        {
            mapCells[0, i] = CellType.Wall;
            mapCells[mapSize - 1, i] = CellType.Wall;
            mapCells[i, 0] = CellType.Wall;
            mapCells[i, mapSize - 1] = CellType.Wall;
        }

        // Add entry point
        int entryPoint = Random.Range(1, mapSize - 1);
        mapCells[0, entryPoint] = CellType.MapStart;

        // Add exit point
        int exitPoint = Random.Range(1, mapSize - 1);
        mapCells[mapSize - 1, exitPoint] = CellType.MapExit;

        // Add Inner Walls
        for (int i = 2; i < mapSize - 2; i+=2)
        {
            int numberOfWallPaths = Random.Range(minNumberOfWallPaths, maxNumberOfWallPaths + 1);

            int j = 1;
            while(j < mapSize - 1)
            {
                int remainingWallLength = mapSize - j - 1;
                if (numberOfWallPaths > 0 && 
                    (remainingWallLength <= 2 * numberOfWallPaths - 1 ||
                    Random.Range(0f, 1f) <= (float)numberOfWallPaths / remainingWallLength))
                {
                    mapCells[i, j] = CellType.WallExit;
                    numberOfWallPaths -= 1;

                    mapCells[i, j + 1] = CellType.Wall;
                    j++;
                }
                else
                {
                    mapCells[i, j] = CellType.Wall;
                }
                j++;
            }
        }
    }

    void DebugPrint()
    {
        string line = "";
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                line += (int) mapCells[i, j];
            }
            line += "\n";
        }
        Debug.Log(line);
    }

    void ShortestPath()
    {
        
    }
}

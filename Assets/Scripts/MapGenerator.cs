using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public CellType[,] GenerateNewMap()
    {
        if (mapSize % 2 == 0)
            mapSize += 1;

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

        return mapCells;
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

    // shortest path (modified BFS)
    Stack<int> shortestPath;

    public int[] FindShortestPath()
    {
        int i = 0;
        while (mapCells[0,i] != CellType.MapStart)
            i++;

        var currentPath = new Stack<int>();
        currentPath.Push(i);
        var distanceMemory = new Dictionary<(int, int), int>();
        distanceMemory[(0,i)] = 0;
        TraverseToNextLayer(2, currentPath, distanceMemory);

        return shortestPath.ToArray();
    }

    void TraverseToNextLayer(int layer, Stack<int> path, Dictionary<(int, int), int> memory)
    {
        if (layer == mapCells.GetLength(0) - 1)
            HandleFinalLayer(layer, path, memory);
        else
        HandleIntermediateLayer(layer, path, memory);
    }

    void HandleIntermediateLayer(int layer, Stack<int> path, Dictionary<(int, int), int> memory)
    {
        int lastExit = path.Peek();

        // search first exit directly above or up
        for (int i = lastExit; i < mapCells.GetLength(1); i++)
        {
            if (mapCells[layer, i] == CellType.WallExit)
            {
                int currentDistance = memory[(layer - 2, lastExit)] + System.Math.Abs(lastExit - i);
                if (!memory.ContainsKey((layer, i)) || currentDistance < memory[(layer, i)])
                {
                    memory[(layer, i)] = currentDistance;
                    path.Push(i);
                    TraverseToNextLayer(layer + 2, path, memory);
                    path.Pop();
                }
                break;
            }
        }

        // search first exit down
        for (int i = lastExit - 1; i > 0; i--)
        {
            if (mapCells[layer, i] == CellType.WallExit)
            {
                int currentDistance = memory[(layer - 2, lastExit)] + System.Math.Abs(lastExit - i);
                if (!memory.ContainsKey((layer, i)) || currentDistance < memory[(layer, i)])
                {
                    memory[(layer, i)] = currentDistance;
                    path.Push(i);
                    TraverseToNextLayer(layer + 2, path, memory);
                    path.Pop();
                }
                break;
            }
        }
    }

    void HandleFinalLayer(int layer, Stack<int> path, Dictionary<(int, int), int> memory)
    {
        int i = 0;
        while (mapCells[layer, i] != CellType.MapExit)
            i++;
        
        int lastExit = path.Peek();
        int currentDistance = memory[(layer - 2, lastExit)] + System.Math.Abs(lastExit - i);
        if (memory.ContainsKey((layer, i)) && currentDistance >= memory[(layer, i)])
            return;

        memory[(layer, i)] = currentDistance;

        path.Push(i);
        shortestPath = new Stack<int>(path);
        path.Pop();
    }
}

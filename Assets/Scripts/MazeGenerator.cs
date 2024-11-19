using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] int mazeSize = 51;

    bool[,] grid;

    // Wilson Maze algorithm
    public bool[,] GenerateNewMaze()
    {
        // initialize grid
        mazeSize -= 2;
        if (mazeSize % 2 == 0)
            mazeSize += 1;
        
        grid = new bool[mazeSize, mazeSize];

        var unvisited = new List<(int,int)>();
        for (int i = 0; i < mazeSize; i++)
            for (int j = 0; j < mazeSize; j++)
                if (i % 2 == 0 && j % 2 == 0)
                    unvisited.Add((i, j));
        
        var visited = new List<(int,int)>();

        var path = new Dictionary<(int, int), int>();

        // choose the first cell to put in the visited list
        var current = unvisited[Random.Range(0, unvisited.Count)];
        unvisited.Remove(current);
        visited.Add(current);
        Cut(current);

        while (unvisited.Count > 0)
        {
            var first = unvisited[Random.Range(0, unvisited.Count)];
            current = first;

            while (true)
            {
                int dirNum = Random.Range(0,4);
                while (!IsValidDirection(current,dirNum))
                    dirNum = Random.Range(0,4);
                path[current] = dirNum;
                current = GetNextCell(current,dirNum,2);
                if (visited.IndexOf(current) != -1)
                    break;
            }

            current = first;
            while (true)
            {
                visited.Add(current);
                unvisited.Remove(current);
                Cut(current);

                var dirNum = path[current];
                var crossed = GetNextCell(current,dirNum,1);
                Cut(crossed);

                current = GetNextCell(current,dirNum,2);
                if (visited.IndexOf(current) != -1)
                {
                    path = new Dictionary<(int, int), int>();
                    break;
                }
            }
        }

        mazeSize += 2;
        bool[,] res = new bool[mazeSize, mazeSize];
        for (int i = 0; i < grid.GetLength(0); i++)
            for (int j = 0; j < grid.GetLength(1); j++)
                res[i + 1,j + 1] = grid[i,j];
        while (true)
        {
            int num = Random.Range(1, mazeSize - 1);
            if (res[1, num])
            {
                res[0, num] = true;
                break;
            }
        }
        while (true)
        {
            int num = Random.Range(1, mazeSize - 1);
            if (res[mazeSize - 2, num])
            {
                res[mazeSize - 1, num] = true;
                break;
            }
        }
        return res;
    }

    void Cut((int, int) cell)
    {
        grid[cell.Item1, cell.Item2] = true;
    }

    (int,int) GetNextCell((int,int) cell, int dirNum, int fact)
    {
        var directions = new (int,int) [] { (0,1), (1,0), (0,-1), (-1,0) };
        var dirTup = directions[dirNum];
        return (cell.Item1 + fact * dirTup.Item1, cell.Item2 + fact * dirTup.Item2);
    }

    bool IsValidDirection((int,int) cell, int dirNum)
    {
        var newCell = GetNextCell(cell,dirNum,2);
        bool tooSmall = (newCell.Item1 < 0 || newCell.Item2 < 0);
        bool tooBig = (newCell.Item1 >= mazeSize || newCell.Item2 >= mazeSize);
        return !(tooSmall || tooBig);
    }
}

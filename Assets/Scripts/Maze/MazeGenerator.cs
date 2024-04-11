using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell _mazeCellPrefab;

    //(0,0.5,0.55) pkt startowy
    [Header("Main Settings")]
    [SerializeField] private int _mazeWidth;
    [SerializeField] private int _mazeDepth;
    
    [Header("Other")]
    [SerializeField] [Tooltip("For cell height (half of cell height)")]
    private float fixedYPos = 1f;
    
    private MazeCell[,] _mazeGrid;

    private Vector3 cornerA;
    private Vector3 cornerB;
    
    public Vector3[] Corners
    {
        get => new[] { cornerA, cornerB };
    }

    public float FixedYPos
    {
        get => fixedYPos;
    }

    public void StartMaze()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                GameObject currentCell = Instantiate(_mazeCellPrefab.gameObject, new Vector3(x, fixedYPos, z), Quaternion.identity);
                _mazeGrid[x, z] = currentCell.GetComponent<MazeCell>();
                
                if (x == 0 && z == 0)
                {
                    
                    cornerA = FindObject(currentCell.transform);
                }

                if (x == _mazeWidth-1 && z == _mazeDepth-1)
                {
                    cornerB = FindObject(currentCell.transform);
                }
            }
        }

        GenerateMaze(null, _mazeGrid[0, 0]);
        
        ChangeState(States.SetTwoRandomPoints);
    }

    private Vector3 FindObject(Transform currentCell)
    {
        foreach (Transform childTransform in currentCell)
        {
            if (childTransform.CompareTag("Block"))
            {
                return childTransform.position;
            }
        }

        Debug.LogError("It shouldn't happen!");
        return new Vector3(0,3,0);
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;

        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisitedCells = GetUnvisitedCells(currentCell);

        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = (int)currentCell.transform.position.x;
        int z = (int)currentCell.transform.position.z;

        if (x + 1 < _mazeWidth)
        {
            var cellToRight = _mazeGrid[x + 1, z];
            
            if (cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }

        if (x - 1 >= 0)
        {
            var cellToLeft = _mazeGrid[x - 1, z];

            if (cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }

        if (z + 1 < _mazeDepth)
        {
            var cellToFront = _mazeGrid[x, z + 1];

            if (cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }

        if (z - 1 >= 0)
        {
            var cellToBack = _mazeGrid[x, z - 1];

            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null)
        {
            return;
        }

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }
    
    private void ChangeState(States newState)
    {
        GetComponent<GameManager>().StartState(newState);
    }

}

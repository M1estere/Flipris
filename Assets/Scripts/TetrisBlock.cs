using System;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    private static int Height = 17;
    private static int Width = 10;
    
    private static Transform[,] _grid = new Transform[Width, Height];

    [SerializeField] private Vector3 _rotationPoint;
    [Space(5)]
    
    [SerializeField] private float _fallTime = 0.8f;

    private float _previousTime;

    private GameManager _gameManager;
    private SpawnBlock _spawner;
    
    private void Start()
    {
        _spawner = FindObjectOfType<SpawnBlock>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        MoveDown();
    }

    #region Movement Methods
    public void MoveSidewards(int index)
    {
        transform.position += new Vector3(index, 0, 0);
        if (!CanMove())
            transform.position -= new Vector3(index, 0, 0);
    }

    public void Rotate(int index)
    {
        transform.RotateAround(transform.TransformPoint(_rotationPoint), new Vector3(0, 0, 1), 90 * index);
        if (!CanMove())
            transform.RotateAround(transform.TransformPoint(_rotationPoint), new Vector3(0, 0, 1), -90 * index);
    }

    private void MoveDown()
    {
        if (!(Time.time - _previousTime > (_gameManager.Boosted ? _fallTime / 10 : _fallTime))) return;
        
        transform.position += new Vector3(0, -1, 0);
        if (!CanMove())  // reached bottom | other block
        {
            transform.position -= new Vector3(0, -1, 0);
            this.enabled = false;

            AddToGrid();
            CheckLines();

            _spawner.SpawnNewBlock();
        }

        _previousTime = Time.time;
    }
    #endregion
    
    #region Logic Methods
    private void AddToGrid()
    {
        foreach (Transform child in transform)
        {
            var position = child.transform.position;
            var roundedX = Mathf.RoundToInt(position.x);
            var roundedY = Mathf.RoundToInt(position.y);

            try 
            {
                _grid[roundedX, roundedY] = child;
            } catch (IndexOutOfRangeException e)
            {
                _spawner.EndGame();
                throw;
            }
        }
    }

    private void CheckLines()
    {
        for (var i = Height - 1; i >= 0; i--)
        {
            if (!HasLine(i)) continue;
            DeleteLine(i);
            RowDown(i);
        }
    }

    private bool HasLine(int i)
    {
        for (var j = 0; j < Width; j++)
        {
            if (_grid[j, i] == null)
                return false;
        }

        return true;
    }

    private void DeleteLine(int i)
    {
        for (var j = 0; j < Width; j++)
        {
            Destroy(_grid[j, i].gameObject);
            _grid[j, i] = null;
        }
        _gameManager.IncreaseScore(100);
    }

    private void RowDown(int i)
    {
        for (var y = i; y < Height; y++)
        {
            for (var j = 0; j < Width; j++)
            {
                if (_grid[j, y] == null) continue;
                
                _grid[j, y - 1] = _grid[j, y];
                _grid[j, y] = null;

                _grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
            }
        }
    }
    
    private bool CanMove()
    {
        foreach (Transform child in transform)
        {
            var position = child.transform.position;
            var roundedX = Mathf.RoundToInt(position.x);
            var roundedY = Mathf.RoundToInt(position.y);

            if (roundedX < 0 || roundedX >= Width || roundedY < 5 || roundedY >= Height)
                return false;

            if (_grid[roundedX, roundedY] != null)
                return false;
        }

        return true;
    }
    #endregion
}

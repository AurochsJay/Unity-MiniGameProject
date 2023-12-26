using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisManager : MonoBehaviour
{
    [SerializeField] public Grid2D grid;
    [SerializeField] private TetrominoSpawner spawner;
    [SerializeField] private TetrominoController tetromino;

    public readonly int width = 10;
    public readonly int height = 24;

    private void Start()
    {
        //grid = GameObject.Find("GameManager").GetComponent<Grid2D>(width, height);
        grid.SetGrid(height, width);

        //CheckGridArrayDebug();
    }

    private void Update()
    {
        // grid.array[1,1]
        CheckLine();
    }

    private void CheckLine()
    {

    }

    public void SyncGridPos(Vector3 block) => grid.array[(int)block.z, (int)block.x] = 1;

    //private void CheckGridArrayDebug()
    //{
    //    for (int i = 0; i < height; i++)
    //    {
    //        for (int j = 0; j < width; j++)
    //        {
    //            Debug.Log($"grid[{i},{j}] : {grid.array[i, j]}");
    //        }
    //    }
    //}

}

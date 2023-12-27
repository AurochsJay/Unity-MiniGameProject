using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisManager : MonoBehaviour
{
    [SerializeField] public Grid2D grid;
    [SerializeField] public TetrominoSpawner spawner;
    [SerializeField] private TetrominoController tetromino;

    public readonly int width = 10;
    public readonly int height = 24;

    private int lineCount = 0;
    private Vector3 offset = new Vector3(0.5f, 0, -0.5f);

    private void Start()
    {
        //grid = GameObject.Find("GameManager").GetComponent<Grid2D>(width, height);
        grid.SetGrid(height, width);

        //CheckGridArrayDebug();
    }

    private void Update()
    {
        // grid.array[1,1]
        FindTetromino();
        //CheckLine();
    }

    private void FindTetromino()
    {

    }

    public void CheckLine()
    {
        for(int j = 0; j < height; j++)
        {
            int rowCount = 0;
            for(int i = 0; i < width; i++)
            {
                if(grid.array[j,i] == 1)
                {
                    rowCount++;
                }
            }

            if(rowCount == width) // 줄이 다 채워졌음
            {
                Debug.Log($"줄이 다 채워지면 말좀.{j}열");
                ClearLine(j);
            }
        }
    }

    private void ClearLine(int column)
    {
        for(int i = 0; i < width; i++)
        {
            grid.array[column, i] = 0;
            Vector3 destroyPoint = new Vector3(i, -0.5f, column) + offset;
            RaycastHit hit;
            if(Physics.Raycast(destroyPoint, Vector3.up * 2f, out hit))
            {
                Debug.DrawRay(destroyPoint, Vector3.up * 5f, Color.red, Mathf.Infinity);
                Destroy(hit.transform.gameObject);
            }
        }
    }

    // Line Clear하면 윗줄 밑으로 땡겨와야함 // 두 줄 이상 동시에 사라지면? 한번에 최대 4줄삭제
    private void PullLine()
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

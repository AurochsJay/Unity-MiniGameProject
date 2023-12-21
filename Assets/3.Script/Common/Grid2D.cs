using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid2D : MonoBehaviour
{
    public int width; // 가로
    public int height; // 세로

    // 그리드 배열
    public int[,] array;

    private void Start()
    {
        InitGrid();
    }

    private void InitGrid()
    {
        array = new int[height, width]; // 그리드 사이즈 정하고 index에 값부여해서 true,false 판단
        
        for(int i = 0; i< height; i++)
        {
            for(int j = 0; j< width; j++)
            {
                array[i, j] = 0; // false == 0
            }
        }
    }
}

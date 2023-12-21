using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid2D : MonoBehaviour
{
    public int width; // ����
    public int height; // ����

    // �׸��� �迭
    public int[,] array;

    private void Start()
    {
        InitGrid();
    }

    private void InitGrid()
    {
        array = new int[height, width]; // �׸��� ������ ���ϰ� index�� ���ο��ؼ� true,false �Ǵ�
        
        for(int i = 0; i< height; i++)
        {
            for(int j = 0; j< width; j++)
            {
                array[i, j] = 0; // false == 0
            }
        }
    }
}
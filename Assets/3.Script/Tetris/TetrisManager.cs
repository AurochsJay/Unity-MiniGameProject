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

        
    }

    private void Update()
    {
        FindTetromino();
        //CheckLine();
        if (Input.GetMouseButton(0))
        {
            Time.timeScale = 1;
        }
        CheckGridArrayDebug();
    }

    private void FindTetromino()
    {

    }

    public void CheckLine()
    {
        List<int> clearRows = new List<int>(); // �갡 ���� ���ĸ� Ŭ������ ���� ���� ���� ����Ʈ��
        bool isClear = false;

        for(int j = 0; j < 20; j++)
        {
            int rowCount = 0;
            for(int i = 0; i < width; i++)
            {
                if(grid.array[j,i] == 1)
                {
                    rowCount++;
                }
            }

            if(rowCount == width) // ���� �� ä������ 
            {
                // Clear�� ������ �ѹ��� �� üũ�ϰ� Clear�� Line ���� ��������
                // ���� üũ�ϰ� �ٷ� Clear�ع����� ������ �ٷ� Pull�� �Ǽ� �����̻� �پ����� �� ������ ���Եȴ�.
                Debug.Log($"���� �� ä������ ����.{j}��");
                clearRows.Add(j);
                isClear = true;
                //ClearLine(j);
            }
        }

        for(int j = 0; j < clearRows.Count; j++)
        {
            ClearLine(clearRows[j]);
        }


        if(isClear)
        {
            Time.timeScale = 0;
            PullLine(clearRows);
        }
    }

    private void ClearLine(int column)
    {
        for(int i = 0; i < width; i++)
        {
            grid.array[column, i] = 0;
            //Debug.Log($"Ŭ������� ���԰��� : grid.array[{column},{i}] �� : {grid.array[column, i]}");
            Vector3 destroyPoint = new Vector3(i, -0.5f, column) + offset;
            RaycastHit hit;
            if(Physics.Raycast(destroyPoint, Vector3.up * 2f, out hit))
            {
                Debug.DrawRay(destroyPoint, Vector3.up * 5f, Color.red, Mathf.Infinity);
                Destroy(hit.transform.gameObject);
            }
        }

        //PullLine(column); // ��Ȯ��-Ŭ����-Ǯ
    }

    // Line Clear�ϸ� ���� ������ ���ܿ;��� // �� �� �̻� ���ÿ� �������? �ѹ��� �ִ� 4�ٻ���
    private void PullLine(List<int> clearRows)
    {
        // Clear�� �� ���� ������ ���پ� �����.
        // 1���� clear�Ǹ� 2~20�� ���� �� 3���� clear�Ǹ� 4~20 �� ����� �ǰ���
        // clear�� ���� ���ٺ��� ��ĭ�� ����� �ǰ���.
        // clearRows[0] -> ó�� ������ �� value, �� ������ ����°� �´�
        int pullCount = clearRows.Count; // ���ܾ��ϴ� count �ִ� 4�� ����� �ְ���, ���� �� ���̿� �ִ� ���� �� ī��Ʈ���� ���°ɷ� ����� Ƚ���� ������

        Debug.Log("PullLine ������");

        for(int j = clearRows[0]; j < 20; j++)
        {
            int count = 0;
            for (int row = 0; row < clearRows.Count; row++)
            {
                if (j > clearRows[row])
                {
                    count++;
                }
            }

            for(int i = 0; i < width; i++)
            {
                Vector3 rayPoint = new Vector3(i, -0.5f, j) + offset;
                RaycastHit hit;
                if (Physics.Raycast(rayPoint, Vector3.up * 2f, out hit))
                {
                    // ó�� clear�� ������ �ݺ��� ���ư��ϱ� ���࿡ �¾Ҵٸ� �� ���� clear�� �ȵ� ���̴�.
                    // �� j ���� clearRows[0] clearRows[clearRows.Count] ���̿� �ִٸ� ���ܾ��ϴ� ī��Ʈ�� ����

                    //if(clearRows[0] <= j && j <= clearRows[clearRows.Count-1])
                    //{
                    //    count = pullCount - (j - clearRows[0]);
                    //}

                    //hit.transform.position += new Vector3(0,0, -count);
                    grid.array[j, i] = 0; // ���� ��ġ�� ����� ���ԵǴϱ� index value 0
                    grid.array[j - count, i] = 1; // ����� ��ġ�� ����� �ְԵǴϱ� index value 1
                }
                Debug.Log($"Ŭ������� ���԰��� : grid.array[{0},{i}] �� : {grid.array[0, i]}");
            }
            
        }
    }

    public void SyncGridPos(Vector3 block) => grid.array[(int)block.z, (int)block.x] = 1;

    private void CheckGridArrayDebug()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Debug.Log($"grid[{i},{j}] : {grid.array[i, j]}");
            }
        }
    }

}

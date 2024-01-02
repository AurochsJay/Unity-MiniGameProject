using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        grid.SetGrid(height, width);
    }

    private void Update()
    {
        FindTetromino();
        //CheckGridArrayDebug();
    }

    private void FindTetromino()
    {

    }

    public void CheckLine()
    {
        List<int> clearRows = new List<int>(); // �갡 ���� ���ĸ� Ŭ������ ���� ���� ���� ����Ʈ��
        bool isClear = false;
        bool isGameOver = false;

        for(int j = 0; j < 21; j++) // j==20 -> GameOver
        {
            int rowCount = 0;
            for(int i = 0; i < width; i++)
            {
                if(grid.array[j,i] == 1)
                {
                    rowCount++;
                }

                if(grid.array[20,i] == 1)
                {
                    isGameOver = true;
                }
            }

            if(rowCount == width) // ���� �� ä������ 
            {
                // Clear�� ������ �ѹ��� �� üũ�ϰ� Clear�� Line ���� ��������
                // ���� üũ�ϰ� �ٷ� Clear�ع����� ������ �ٷ� Pull�� �Ǽ� �����̻� �پ����� �� ������ ���Եȴ�.
                Debug.Log($"���� �� ä������ ����.{j}��");
                clearRows.Add(j);
                isClear = true;
            }
        }

        for(int j = 0; j < clearRows.Count; j++)
        {
            ClearLine(clearRows[j]);
        }


        if(isClear)
        {
            //Time.timeScale = 0;
            StartCoroutine(PullLine(clearRows));
        }

        if(isGameOver)
        {
            GameOver();
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
                //Debug.DrawRay(destroyPoint, Vector3.up * 5f, Color.red, Mathf.Infinity);
                Destroy(hit.transform.gameObject);
            }
        }

        //PullLine(column); // ��Ȯ��-Ŭ����-Ǯ
    }

    // Line Clear�ϸ� ���� ������ ���ܿ;��� // �� �� �̻� ���ÿ� �������? �ѹ��� �ִ� 4�ٻ���
    private IEnumerator PullLine(List<int> clearRows)
    {
        yield return new WaitForSeconds(0.05f);

        for(int j = clearRows[0]; j < 20; j++)
        {
            int count = 0;
            for (int row = 0; row < clearRows.Count; row++)
            {
                Debug.Log("clearRows[row]�� ��" + clearRows[row]);
                if (j > clearRows[row])
                {
                    count++;
                }
            }
            Debug.Log($"j�� ���� ���� count �� j: {j}, count:{count}");

            for(int i = 0; i < width; i++)
            {
                Vector3 rayPoint = new Vector3(i, -0.5f, j) + offset;
                RaycastHit hit;
                if (Physics.Raycast(rayPoint, Vector3.up * 2f, out hit))
                {
                    // ó�� clear�� ������ �ݺ��� ���ư��ϱ� ���࿡ �¾Ҵٸ� �� ���� clear�� �ȵ� ���̴�.
                    Debug.DrawRay(rayPoint, Vector3.up * 5f, Color.red, Mathf.Infinity);
                    hit.transform.position += new Vector3(0,0, -count);
                    grid.array[j, i] = 0; // ���� ��ġ�� ����� ���ԵǴϱ� index value 0
                    grid.array[j - count, i] = 1; // ����� ��ġ�� ����� �ְԵǴϱ� index value 1
                    Debug.Log($"PullLine���� ray ���� �༮������ ���԰��� : grid.array[{0},{i}] �� : {grid.array[0, i]}");
                }
            }
            
        }
    }

    // ���ӿ���
    private void GameOver()
    {

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

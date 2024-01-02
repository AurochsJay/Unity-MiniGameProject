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
        List<int> clearRows = new List<int>(); // 얘가 지금 뭐냐면 클리어한 줄의 열을 담을 리스트다
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

            if(rowCount == width) // 줄이 다 채워졌음 
            {
                // Clear될 라인을 한번에 다 체크하고 Clear할 Line 열만 보내주자
                // 한줄 체크하고 바로 Clear해버리면 윗줄이 바로 Pull이 되서 두줄이상 붙어있을 때 한줄이 남게된다.
                Debug.Log($"줄이 다 채워지면 말좀.{j}열");
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
            //Debug.Log($"클리어라인 들어왔겠지 : grid.array[{column},{i}] 값 : {grid.array[column, i]}");
            Vector3 destroyPoint = new Vector3(i, -0.5f, column) + offset;
            RaycastHit hit;
            if(Physics.Raycast(destroyPoint, Vector3.up * 2f, out hit))
            {
                //Debug.DrawRay(destroyPoint, Vector3.up * 5f, Color.red, Mathf.Infinity);
                Destroy(hit.transform.gameObject);
            }
        }

        //PullLine(column); // 줄확인-클리어-풀
    }

    // Line Clear하면 윗줄 밑으로 땡겨와야함 // 두 줄 이상 동시에 사라지면? 한번에 최대 4줄삭제
    private IEnumerator PullLine(List<int> clearRows)
    {
        yield return new WaitForSeconds(0.05f);

        for(int j = clearRows[0]; j < 20; j++)
        {
            int count = 0;
            for (int row = 0; row < clearRows.Count; row++)
            {
                Debug.Log("clearRows[row]의 값" + clearRows[row]);
                if (j > clearRows[row])
                {
                    count++;
                }
            }
            Debug.Log($"j의 값에 따른 count 값 j: {j}, count:{count}");

            for(int i = 0; i < width; i++)
            {
                Vector3 rayPoint = new Vector3(i, -0.5f, j) + offset;
                RaycastHit hit;
                if (Physics.Raycast(rayPoint, Vector3.up * 2f, out hit))
                {
                    // 처음 clear된 열부터 반복문 돌아가니까 만약에 맞았다면 그 줄은 clear가 안된 줄이다.
                    Debug.DrawRay(rayPoint, Vector3.up * 5f, Color.red, Mathf.Infinity);
                    hit.transform.position += new Vector3(0,0, -count);
                    grid.array[j, i] = 0; // 맞은 위치에 블록이 없게되니까 index value 0
                    grid.array[j - count, i] = 1; // 끌어온 위치에 블록이 있게되니까 index value 1
                    Debug.Log($"PullLine에서 ray 맞은 녀석있으면 들어왔겠지 : grid.array[{0},{i}] 값 : {grid.array[0, i]}");
                }
            }
            
        }
    }

    // 게임오버
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

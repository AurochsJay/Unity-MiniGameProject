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
        List<int> clearRows = new List<int>(); // 얘가 지금 뭐냐면 클리어한 줄의 열을 담을 리스트다
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

            if(rowCount == width) // 줄이 다 채워졌음 
            {
                // Clear될 라인을 한번에 다 체크하고 Clear할 Line 열만 보내주자
                // 한줄 체크하고 바로 Clear해버리면 윗줄이 바로 Pull이 되서 두줄이상 붙어있을 때 한줄이 남게된다.
                Debug.Log($"줄이 다 채워지면 말좀.{j}열");
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
            //Debug.Log($"클리어라인 들어왔겠지 : grid.array[{column},{i}] 값 : {grid.array[column, i]}");
            Vector3 destroyPoint = new Vector3(i, -0.5f, column) + offset;
            RaycastHit hit;
            if(Physics.Raycast(destroyPoint, Vector3.up * 2f, out hit))
            {
                Debug.DrawRay(destroyPoint, Vector3.up * 5f, Color.red, Mathf.Infinity);
                Destroy(hit.transform.gameObject);
            }
        }

        //PullLine(column); // 줄확인-클리어-풀
    }

    // Line Clear하면 윗줄 밑으로 땡겨와야함 // 두 줄 이상 동시에 사라지면? 한번에 최대 4줄삭제
    private void PullLine(List<int> clearRows)
    {
        // Clear된 줄 위의 열들을 한줄씩 땡긴다.
        // 1열이 clear되면 2~20열 땡긴 후 3열이 clear되면 4~20 열 땡기면 되겠지
        // clear된 줄의 윗줄부터 한칸씩 땡기면 되겠지.
        // clearRows[0] -> 처음 삭제된 열 value, 이 열부터 땡기는게 맞다
        int pullCount = clearRows.Count; // 땡겨야하는 count 최대 4번 땡길수 있겠지, 남은 줄 사이에 있는 열은 저 카운트에서 빼는걸로 땡기는 횟수를 줄이자

        Debug.Log("PullLine 들어오나");

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
                    // 처음 clear된 열부터 반복문 돌아가니까 만약에 맞았다면 그 줄은 clear가 안된 줄이다.
                    // 그 j 열이 clearRows[0] clearRows[clearRows.Count] 사이에 있다면 땡겨야하는 카운트에 변동

                    //if(clearRows[0] <= j && j <= clearRows[clearRows.Count-1])
                    //{
                    //    count = pullCount - (j - clearRows[0]);
                    //}

                    //hit.transform.position += new Vector3(0,0, -count);
                    grid.array[j, i] = 0; // 맞은 위치에 블록이 없게되니까 index value 0
                    grid.array[j - count, i] = 1; // 끌어온 위치에 블록이 있게되니까 index value 1
                }
                Debug.Log($"클리어라인 들어왔겠지 : grid.array[{0},{i}] 값 : {grid.array[0, i]}");
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

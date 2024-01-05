using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveDirection
{
    right = 0,
    left = 1,
    up = 2,
    down = 3
}

public class SnakeController : MonoBehaviour
{
    [SerializeField] private InputManager input;
    [SerializeField] private SnakeManager snakeManager;
    [SerializeField] private GameObject snakeBody_Prefab;

    // Snake 변수
    private List<GameObject> snake = new List<GameObject>();
    private MoveDirection direction = MoveDirection.right;
    private Vector3 startPos = new Vector3(5, 0, 15);

    // Move 메서드 변수
    private float timer = 0.5f;
    private float move_ElapsedTime = 0;

    private void Start()
    {
        input = GameObject.Find("GameManager").GetComponent<InputManager>();
        snakeManager = GameObject.Find("SnakeManager").GetComponent<SnakeManager>();

        snake.Add(this.gameObject);
        snake[0].transform.position = startPos;
        snakeManager.grid.array[(int)startPos.z, (int)startPos.x] = 1;
        //CheckGridArrayDebug();
    }

    private void Update()
    { 
        if(input.snake_Start)
        {
            CheckDirection();
            Move();
        }
        CheckGridArrayDebug();
    }

    // Input에서 입력받아서 방향설정
    private void CheckDirection()
    {
        if(input.snake_Move_X == 1)
        {
            direction = MoveDirection.right;
        }
        else if(input.snake_Move_X == -1)
        {
            direction = MoveDirection.left;
        }
        else if (input.snake_Move_Y == 1)
        {
            direction = MoveDirection.up;
        }
        else if (input.snake_Move_Y == -1)
        {
            direction = MoveDirection.down;
        }
    }

    // 일정 시간마다 움직임
    // 그리드에 Wall 설정, grid.array[y,x] == 0:빈칸, 1:Snake칸, 2:Target칸, 3:Wall칸
    private void Move()
    {
        //Debug.Log("Move 들어오나?");
        move_ElapsedTime += Time.deltaTime;

        if(move_ElapsedTime > timer)
        {
            Debug.Log("Timer 시간되어서 Move로 들어옴");
            Vector3 previousPos = transform.position;
            move_ElapsedTime = 0;

            switch (direction)
            {
                case MoveDirection.right:
                    snake[0].transform.position += Vector3.right;
                    SyncPosToGrid(snake[0], previousPos);
                    //snakeManager.grid.array[(int)snake[0].transform.position.z, (int)snake[0].transform.position.x] = 1;
                    //snakeManager.grid.array[(int)previousPos.z, (int)previousPos.x] = 0;
                    break;
                case MoveDirection.left:
                    snake[0].transform.position += Vector3.left;
                    SyncPosToGrid(snake[0], previousPos);
                    break;
                case MoveDirection.up:
                    snake[0].transform.position += Vector3.forward;
                    SyncPosToGrid(snake[0], previousPos);
                    break;
                case MoveDirection.down:
                    snake[0].transform.position += Vector3.back;
                    SyncPosToGrid(snake[0], previousPos);
                    break;
            }

            EatObject();
            //Follow();
        }

    }

    // 머리 뒤로 따라오는 몸통
    private void Follow()
    {
        //if(snake.Count > 1)
        //{
        //    snake[snake.Count - 1].transform.position = snake[snake.Count - 2].transform.position;
        //    snakeManager.grid.array[]
        //}

        for(int i = 1; i < snake.Count; i++)
        {
            Vector3 previousPos = snake[snake.Count - i].transform.position;

            snake[snake.Count - i].transform.position = snake[snake.Count - (i + 1)].transform.position;
            SyncPosToGrid(snake[snake.Count - i], previousPos);
        }
    }

    // Target 먹음
    private void EatObject()
    {
        //array index용 변수
        int column = (int)snake[0].transform.position.z;
        int row = (int)snake[0].transform.position.x;

        Debug.Log($"이동한 후의 snake[0]의 x값 : {(int)snake[0].transform.position.x}, z값 : {(int)snake[0].transform.position.z}");
        if (snakeManager.grid.array[column, row] == 1)
        {
            GameOver();
        }
        else if(snakeManager.grid.array[column, row] == 2)
        {
            CreateSnakeBody(column, row);
            snakeManager.CreateTarget();
        }
        else if(snakeManager.grid.array[column, row] == 3)
        {
            GameOver();
        }                                                                                                     
    }

    private void SyncPosToGrid(GameObject snake, Vector3 previousPos)
    {
        Debug.Log("SyncPosToGrid");
        Debug.Log($"snake[0]의 x값 : {(int)snake.transform.position.x}, z값 : {(int)snake.transform.position.z}");
        Debug.Log($"previousPos의 x값 : {(int)previousPos.x}, z값 : {(int)previousPos.z}");
        snakeManager.grid.array[(int)snake.transform.position.z, (int)snake.transform.position.x] = 1; // 이 이후에 eat메서드면 당연히 gameover뜨지 먹은 후에 위치를 갱신하자
        snakeManager.grid.array[(int)previousPos.z, (int)previousPos.x] = 0;
    }

    // SnakeBody 생성 <- Snake가 Target을 먹으면 실행
    public void CreateSnakeBody(int column, int row)
    {
        Vector3 offset = CalculateOffsetByDirection();
        GameObject snakeBody = Instantiate(snakeBody_Prefab, Vector3.zero, Quaternion.identity);
        snake.Add(snakeBody);
        snake[snake.Count-1].transform.position = snake[snake.Count-2].transform.position + offset;
    }

    // Snake의 새 몸통이 생성될 때 앞에 두 몸통으로 방향을 계산해서 그 반대 방향으로 offset을 준다.
    // Snake 머리가 어디서 방향을 꺾었던 간에 꺾이는 부분은 항상 3개의 몸통으로 이루어진다.
    private Vector3 CalculateOffsetByDirection()
    {
        Vector3 offset = Vector3.zero;

        if(snake.Count > 1)
        {
            Vector3 directionByTwoBody = snake[snake.Count - 2].transform.position - snake[snake.Count - 1].transform.position;

            if(directionByTwoBody == Vector3.right)
            {
                offset = Vector3.left;
            }
            else if(directionByTwoBody == Vector3.left)
            {
                offset = Vector3.right;
            }
            else if (directionByTwoBody == Vector3.up)
            {
                offset = Vector3.down;
            }
            else if (directionByTwoBody == Vector3.down)
            {
                offset = Vector3.up;
            }
        }
        else // 시작할 때 == snake.Count == 1
        {
            switch (direction)
            {
                case MoveDirection.right:
                    offset = Vector3.left;
                    break;
                case MoveDirection.left:
                    offset = Vector3.right;
                    break;
                case MoveDirection.up:
                    offset = Vector3.down;
                    break;
                case MoveDirection.down:
                    offset = Vector3.up;
                    break;
            }
        }

        return offset;
    }

    private void GameOver()
    {
        Debug.Log("GameOver됨");
    }

    private void CheckGridArrayDebug()
    {
        for (int i = 0; i < 22; i++)
        {
            for (int j = 0; j < 22; j++)
            {
                //Debug.Log($"grid[{i},{j}] : {snakeManager.grid.array[0, 0]}");
                Debug.Log($"grid[{i},{j}] : {snakeManager.grid.array[i, j]}");
            }
        }
    }

}

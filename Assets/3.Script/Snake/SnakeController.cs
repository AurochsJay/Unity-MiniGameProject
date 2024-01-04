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
    private float timer = 0.8f;
    private float move_ElapsedTime = 0;

    private void Start()
    {
        snake.Add(this.gameObject);
        snake[0].transform.position = startPos;
        snakeManager.grid.array[(int)startPos.z, (int)startPos.x] = 1;
    }

    private void Update()
    { 
        if(input.snake_Start)
        {
            CheckDirection();
            Move();
            Follow();
            EatObject();
        }
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
    private void Move()
    {
        move_ElapsedTime += Time.deltaTime;
        Vector3 previousPos = transform.position;

        if(move_ElapsedTime > timer)
        {
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

        }
    }

    // 머리 뒤로 따라오는 몸통
    private void Follow()
    {
        
    }

    // Target 먹음
    private void EatObject()
    {
        //array index용 변수
        int column = (int)snake[0].transform.position.z;
        int row = (int)snake[0].transform.position.x;

        if(snakeManager.grid.array[column, row] == 1)
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
        snakeManager.grid.array[(int)snake.transform.position.z, (int)snake.transform.position.x] = 1;
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

    private Vector3 CalculateOffsetByDirection()
    {
        Vector3 offset = Vector3.zero;

         

        return offset;
    }

    private void GameOver()
    {

    }

}

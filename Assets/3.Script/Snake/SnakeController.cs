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

    // Snake ����
    private List<GameObject> snake = new List<GameObject>();
    private MoveDirection direction = MoveDirection.right;
    private Vector3 startPos = new Vector3(5, 0, 15);

    // Move �޼��� ����
    private float timer = 0.2f;
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
        if(input.snake_Start && !snakeManager.isGameover)
        {
            CheckDirection();
            Move();
        }
        CheckGridArrayDebug();
    }

    // Input���� �Է¹޾Ƽ� ���⼳��
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

    // ���� �ð����� ������
    // �׸��忡 Wall ����, grid.array[y,x] == 0:��ĭ, 1:Snakeĭ, 2:Targetĭ, 3:Wallĭ
    private void Move()
    {
        move_ElapsedTime += Time.deltaTime;

        if(move_ElapsedTime > timer)
        {
            Debug.Log("Timer �ð��Ǿ Move�� ����");
            Vector3 previousPos = transform.position;
            move_ElapsedTime = 0;

            switch (direction)
            {
                case MoveDirection.right:
                    snake[0].transform.position += Vector3.right;
                    break;
                case MoveDirection.left:
                    snake[0].transform.position += Vector3.left;
                    break;
                case MoveDirection.up:
                    snake[0].transform.position += Vector3.forward;
                    break;
                case MoveDirection.down:
                    snake[0].transform.position += Vector3.back;
                    break;
            }

            EatObject(); // �̵��ϰ� ������ Ȯ���ϰ� 
            SyncPosToGrid(snake[0], previousPos); // �� ��ġ�� �°� �׸��忡 ����
            Follow();
        }

    }

    // Target ����
    private void EatObject()
    {
        //array index�� ����
        int column = (int)snake[0].transform.position.z;
        int row = (int)snake[0].transform.position.x;

        Debug.Log($"�̵��� ���� snake[0]�� x�� : {(int)snake[0].transform.position.x}, z�� : {(int)snake[0].transform.position.z}");
        if (snakeManager.grid.array[column, row] == 1)
        {
            snakeManager.GameOver();
        }
        else if(snakeManager.grid.array[column, row] == 2)
        {
            CreateSnakeBody(column, row); // ������ũ ����
            snakeManager.CreateTarget(); // ���ο� Target ��ġ
            snakeManager.coin_Count++;
        }
        else if(snakeManager.grid.array[column, row] == 3)
        {
            snakeManager.GameOver();
        }                                                                                                     
    }

    // �Ӹ� �ڷ� ������� ����
    private void Follow()
    {
        for (int i = 1; i < snake.Count; i++)
        {
            Vector3 previousPos = snake[snake.Count - i].transform.position;

            if (snake.Count-i ==1)
            {
                Vector3 offset = Vector3.zero;
                switch (direction)
                {
                    case MoveDirection.right:
                        offset = Vector3.left;
                        break;
                    case MoveDirection.left:
                        offset = Vector3.right;
                        break;
                    case MoveDirection.up:
                        offset = Vector3.back;
                        break;
                    case MoveDirection.down:
                        offset = Vector3.forward;
                        break;
                }
                snake[1].transform.position = snake[0].transform.position + offset;
            }
            else
            {
                snake[snake.Count - i].transform.position = snake[snake.Count - (i + 1)].transform.position;
            }
            
            if(i == 1)
            {
                SyncPosToGrid(snake[snake.Count - i], previousPos);
            }
        }

        foreach (GameObject snakebody in snake)
        {
            snakeManager.grid.array[(int)snakebody.transform.position.z, (int)snakebody.transform.position.x] = 1;
        }
    }

    private void SyncPosToGrid(GameObject snake, Vector3 previousPos)
    {
        Debug.Log("SyncPosToGrid");
        Debug.Log($"snake[0]�� x�� : {(int)snake.transform.position.x}, z�� : {(int)snake.transform.position.z}");
        Debug.Log($"previousPos�� x�� : {(int)previousPos.x}, z�� : {(int)previousPos.z}");
        snakeManager.grid.array[(int)snake.transform.position.z, (int)snake.transform.position.x] = 1; // �� ���Ŀ� eat�޼���� �翬�� gameover���� ���� �Ŀ� ��ġ�� ��������
        snakeManager.grid.array[(int)previousPos.z, (int)previousPos.x] = 0;
    }

    // SnakeBody ���� <- Snake�� Target�� ������ ����
    public void CreateSnakeBody(int column, int row)
    {
        Vector3 offset = CalculateOffsetByDirection();
        GameObject snakeBody = Instantiate(snakeBody_Prefab, Vector3.zero, Quaternion.identity);
        snake.Add(snakeBody);
        snake[snake.Count-1].transform.position = snake[snake.Count-2].transform.position + offset;
    }

    // Snake�� �� ������ ������ �� �տ� �� �������� ������ ����ؼ� �� �ݴ� �������� offset�� �ش�.
    // Snake �Ӹ��� ��� ������ ������ ���� ���̴� �κ��� �׻� 3���� �������� �̷������.
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
                offset = Vector3.back;
            }
            else if (directionByTwoBody == Vector3.down)
            {
                offset = Vector3.forward;
            }
        }
        else // ������ �� == snake.Count == 1
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
                    offset = Vector3.back;
                    break;
                case MoveDirection.down:
                    offset = Vector3.forward;
                    break;
            }
        }

        return offset;
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

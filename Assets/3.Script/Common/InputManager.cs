using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Third-Person")]
    //이동 변수
    public float move_X;
    public float move_Z;

    //상호작용 키
    public bool isG = false;

    //마우스 회전 변수
    public float mouse_Rotate_X; // 상하 Mouse Y
    public float mouse_Rotate_Y; // 죄우 Mouse X

    [Header("Tetris")]
    //테트리스 이동 변수
    public int tetromino_Move_X; // 좌우 이동
    public int tetromino_Move_Y; // 밑으로 이동
    public bool tetromino_Move = false;

    //테트리스 회전 변수
    public bool tetromino_Rotate = false;

    //테트리스 드랍 변수
    public bool tetromino_Drop = false;

    //테트리스 홀드 변수
    public bool tetromino_Hold = false;

    [Header("Snake")] // 스네이크 변수
    public int snake_Move_X; // 좌우
    public int snake_Move_Y; // 상하
    public bool snake_Start; // 게임시작 

    private void Update()
    {
        move_X = Input.GetAxis("Horizontal");
        move_Z = Input.GetAxis("Vertical");

        mouse_Rotate_X = Input.GetAxis("Mouse Y");
        mouse_Rotate_Y = Input.GetAxis("Mouse X");

        CheckG();

        TetrisInput();
        SnakeInput();
    }

    private void CheckG()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isG = true;
        }
        else
        {
            isG = false;
        }
    }

    private void TetrisInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            tetromino_Move_X = 1;
            tetromino_Move = true;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            tetromino_Move_X = -1;
            tetromino_Move = true;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            tetromino_Move_Y = -1;
            tetromino_Move = true;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            tetromino_Rotate = true;
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            tetromino_Drop = true;
        }
        else if(Input.GetKeyDown(KeyCode.Tab))
        {
            tetromino_Hold = true;
        }
        else
        {
            tetromino_Move_X = 0;
            tetromino_Move_Y = 0;
            tetromino_Move = false;
            tetromino_Rotate = false;
            tetromino_Drop = false;
            tetromino_Hold = false;
        }
    }

    private void SnakeInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            snake_Move_X = 1;
            snake_Move_Y = 0;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            snake_Move_X = -1;
            snake_Move_Y = 0;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            snake_Move_Y = 1;
            snake_Move_X = 0;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            snake_Move_Y = -1;
            snake_Move_X = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            snake_Start = true;
        }
        //else
        //{
        //    snake_Move_X = 0;
        //    snake_Move_Y = 0;
        //}
    }


}

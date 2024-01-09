using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Player")]
    public float move_X; // �̵� ����
    public float move_Z; // �̵� ����
    public float mouse_Rotate_X; // ���� Mouse Y
    public float mouse_Rotate_Y; // �˿� Mouse X
    public bool press_Space = false; // ����Ű
    public bool press_G = false; // ��ȣ�ۿ� Ű
    public bool press_Shift = false;

    [Header("Tetris")]
    //��Ʈ���� �̵� ����
    public int tetromino_Move_X; // �¿� �̵�
    public int tetromino_Move_Y; // ������ �̵�
    public bool tetromino_Move = false;
    public bool tetromino_Rotate = false; // ��Ʈ���� ȸ�� ����
    public bool tetromino_Drop = false; // ��Ʈ���� ��� ����
    public bool tetromino_Hold = false; // ��Ʈ���� Ȧ�� ����
    public bool tetromino_Start = false; // ���ӽ���


    [Header("Snake")] // ������ũ ����
    public int snake_Move_X; // �¿�
    public int snake_Move_Y; // ����
    public bool snake_Start; // ���ӽ��� 

    private void Update()
    {
        PlayerInput();
        TetrisInput();
        SnakeInput();
    }

    private void PlayerInput()
    {
        move_X = Input.GetAxis("Horizontal");
        move_Z = Input.GetAxis("Vertical");

        mouse_Rotate_X = Input.GetAxis("Mouse Y");
        mouse_Rotate_Y = Input.GetAxis("Mouse X");

        if (Input.GetKeyDown(KeyCode.G))
        {
            press_G = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            press_Space = true;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            press_Shift = true;
        }
        else
        {
            press_G = false;
            press_Space = false;
            press_Shift = false;
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
            if(tetromino_Start)
            {
                tetromino_Drop = true;
            }
            tetromino_Start = true;
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

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

    //테트리스 회전 변수
    public bool tetromino_Rotate = false;

    //테트리스 드랍 변수
    public bool tetromino_Drop = false;

    private void Update()
    {
        move_X = Input.GetAxis("Horizontal");
        move_Z = Input.GetAxis("Vertical");

        mouse_Rotate_X = Input.GetAxis("Mouse Y");
        mouse_Rotate_Y = Input.GetAxis("Mouse X");

        CheckG();

        TetrisInput();
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
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            tetromino_Move_X = -1;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            tetromino_Move_Y = -1;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            tetromino_Rotate = true;
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            tetromino_Drop = true;
        }
        else
        {
            tetromino_Move_X = 0;
            tetromino_Move_Y = 0;
            tetromino_Rotate = false;
            tetromino_Drop = false;
        }




    }


}

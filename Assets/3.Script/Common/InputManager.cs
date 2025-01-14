using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Player")]
    public float move_X; // 이동 변수
    public float move_Z; // 이동 변수
    public float mouse_Rotate_X; // 상하 Mouse Y
    public float mouse_Rotate_Y; // 죄우 Mouse X
    public bool press_Space = false; // 점프키
    public bool press_G = false; // 상호작용 키
    public bool press_Shift = false;

    [Header("MouseCursorSetting")]
    public bool cursorLocked = true;

    [Header("Lobby")]
    public bool press_UpArrow = false;
    public bool press_DownArrow = false;
    public bool press_Enter = false;

    [Header("Tetris")]
    //테트리스 이동 변수
    public int tetromino_Move_X; // 좌우 이동
    public int tetromino_Move_Y; // 밑으로 이동
    public bool tetromino_Move = false;
    public bool tetromino_Rotate = false; // 테트리스 회전 변수
    public bool tetromino_Drop = false; // 테트리스 드랍 변수
    public bool tetromino_Hold = false; // 테트리스 홀드 변수
    public bool tetromino_Start = false; // 게임시작


    [Header("Snake")] // 스네이크 변수
    public int snake_Move_X; // 좌우
    public int snake_Move_Y; // 상하
    public bool snake_Start; // 게임시작 

    [Header("JJump")]
    [SerializeField] private JJumpManager jjumpManager;
    public bool JJump_Start = false; // 게임시작 
    public string str = "변수 필요없을듯";

    private void Update()
    {
        PlayerInput();
        LobbyInput();
        TetrisInput();
        SnakeInput();
        JJumpInput();

        MasterInput();
    }

    private void PlayerInput()
    {
        if (GameObject.Find("Player") == null) return;

        if (GameManager.instance.presentScene == Scene.JJump && !JJump_Start) return;

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

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(cursorLocked)
            {
                cursorLocked = false;
            }
            else
            {
                cursorLocked = true;
            }

        }

        SetCursorState(cursorLocked);
    }

    private void LobbyInput()
    {
        if(GameManager.instance.presentScene == Scene.Lobby)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                press_UpArrow = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                press_DownArrow = true;
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                press_Enter = true;
            }
            else
            {
                press_UpArrow = false;
                press_DownArrow = false;
                press_Enter = false;
            }
        }
    }

    private void TetrisInput()
    {
        if(GameManager.instance.presentScene == Scene.Tetris)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                tetromino_Move_X = 1;
                tetromino_Move = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                tetromino_Move_X = -1;
                tetromino_Move = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                tetromino_Move_Y = -1;
                tetromino_Move = true;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                tetromino_Rotate = true;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                if (tetromino_Start)
                {
                    tetromino_Drop = true;
                }
                tetromino_Start = true;
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
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
        
    }

    private void SnakeInput()
    {
        if(GameManager.instance.presentScene == Scene.Snake)
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
        }
        
        //else
        //{
        //    snake_Move_X = 0;
        //    snake_Move_Y = 0;
        //}
    }

    private void JJumpInput()
    {
        if(GameManager.instance.presentScene == Scene.JJump)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                JJump_Start = true;
            }
        }
    }

    private void MasterInput()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            GameManager.instance.coin += 1000;
        }
    }

    private void SetCursorState(bool state)
    {
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
    }

}

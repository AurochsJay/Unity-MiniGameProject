using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoController : MonoBehaviour
{
    //모든 그룹에 컴포넌트로 달아놓는다. 현재 움직일 블록이면 움직이게 한다.
    //현재 선택된 블록인지 판단하는 것은 TetrisManager에서 받아온다.
    //입력은 InputManager에서 가져온다.
    [SerializeField] private InputManager input;
    [SerializeField] private TetrisManager tetris;
    private int rotate_Count;

    private void Start()
    {
        input = GameObject.Find("GameManager").GetComponent<InputManager>();
        tetris = GameObject.Find("TetrisManager").GetComponent<TetrisManager>();
        rotate_Count = 0;
    }

    private void Update()
    {
        if(tetris.isFieldTetromino)
        {
            Move();
            Rotate();
            Drop();
        }
    }

    private void Move()
    {
        transform.position += new Vector3(input.tetromino_Move_X, 0, input.tetromino_Move_Y);
    }

    private void Rotate()
    {
        if(input.tetromino_Rotate)
        {
            rotate_Count++;
            transform.rotation = Quaternion.Euler(0, 90*rotate_Count, 0);
        }
    }

    private void Drop()
    {

    }

}

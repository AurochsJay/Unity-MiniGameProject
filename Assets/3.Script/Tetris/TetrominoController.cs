using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoController : MonoBehaviour
{
    //��� �׷쿡 ������Ʈ�� �޾Ƴ��´�. ���� ������ ����̸� �����̰� �Ѵ�.
    //���� ���õ� ������� �Ǵ��ϴ� ���� TetrisManager���� �޾ƿ´�.
    //�Է��� InputManager���� �����´�.
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

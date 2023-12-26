using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rot
{
    Rot0,
    Rot90,
    Rot180,
    Rot270
}

public class TetrominoController : MonoBehaviour
{
    //모든 그룹에 컴포넌트로 달아놓는다. 현재 움직일 블록이면 움직이게 한다.
    //현재 선택된 블록인지 판단하는 것은 TetrisManager에서 받아온다.
    //입력은 InputManager에서 가져온다.
    [SerializeField] private InputManager input;
    [SerializeField] private TetrisManager tetris;
    [SerializeField] private BlockHandler[] blocks;
    public int rotate_Count;
    public bool isFieldTetromino = false;

    private int grid_X;
    private int grid_Y;

    private bool canMove;
    private int canMoveCount = 0;

    //전 위치로 되돌리는 변수
    private Transform prevTransform;
    public Vector3[] pivotPoint;
    private Vector3 offset = new Vector3(0.5f, 0, 0.5f);

    public Rot rot = Rot.Rot0;

    private void Start()
    {
        input = GameObject.Find("GameManager").GetComponent<InputManager>();
        tetris = GameObject.Find("TetrisManager").GetComponent<TetrisManager>();
        
        rotate_Count = 0;

        if (transform.CompareTag("Tetromino_O") || transform.CompareTag("Tetromino_I")) offset = Vector3.zero;

        grid_X = (int)(transform.position.x + offset.x);
        grid_Y = (int)(transform.position.z + offset.z);
        prevTransform = GetComponent<Transform>();

        FindBlocks();
        //CheckGridArrayDebug();
    }

    private void Update()
    {
        // Input 조작
        if(isFieldTetromino)
        {
            UpdateBlocksWorldPosition();
            CopyPreviousTransform();
            Move();
            Rotate();
            Drop();
        }
        
    }

    private void Move()
    {
        if(CheckTetrominoCanMove())
        {
            grid_X += input.tetromino_Move_X;
            grid_Y += input.tetromino_Move_Y;
            transform.position = new Vector3(grid_X, 0, grid_Y) + offset;

            if(CheckBlocksOverlap()) // 이동을 했는데, block들이 겹쳤다면
            {
                RollBackMove();
            }

            //todo 이동했을때 블록이 자동으로 떨어지는 timer 초기화 // 아래로만 이동했을때?
        }
        //else if(false) // 이동할 수 없고 밑줄 블록에 닿았다면 놓아진다. 옆 블록이라면? 되돌린다.
        //{

        //}
        

    }

    private void Rotate()
    {
        if(input.tetromino_Rotate && CheckTetrominoCanMove())
        {
            if (rot > Rot.Rot270) rot = 0;

            transform.rotation = Quaternion.Euler(0, 90 * (int)(rot+1), 0);
            //transform.RotateAround(pivotPoint[rotate_Count] + transform.position, Vector3.up, 90);
            rot++;

            if (CheckBlocksOverlap())
            {
                RollBackRotate();
            }
        }
    }

    private void Drop()
    {

    }

    // 테트로미노를 놓았다면, 놓아졌다면
    private bool isPlaceTetromino()
    {

        return true;
    }

    // BlockHandler 컴포넌트 할당
    private void FindBlocks()
    {
        blocks = new BlockHandler[transform.childCount];

        for(int i = 0; i< transform.childCount; i++)
        {
            blocks[i] = transform.GetChild(i).GetComponent<BlockHandler>();
        }
    }

    // Tetromino가 이동할 수 있는지 체크, Board 범위안에서
    private bool CheckTetrominoCanMove()
    {
        int count = 0;

        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i].CheckBlockCanMove())
            {
                count++;
            }
        }

        if(blocks.Length == count)
        {
            return true;
        }
        else
        {
            Debug.Log("들어오니");
            RollBackTransform(prevTransform); 
            return false;
        }
    }

    // Block들이 다른 Block들과 겹쳐있는지 체크
    private bool CheckBlocksOverlap()
    {
        int count = 0;

        for(int i = 0; i < blocks.Length; i++)
        {
            if(tetris.grid.array[(int)blocks[i].worldPosition.z, (int)blocks[i].worldPosition.x] == 1)
            {
                count++;
            }
        }

        if(count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Move시 겹치면 롤백
    private void RollBackMove()
    {
        grid_X -= input.tetromino_Move_X;
        grid_Y -= input.tetromino_Move_Y;
        transform.position = new Vector3(grid_X, 0, grid_Y);
    }

    // Rotate시 겹치면 롤백
    private void RollBackRotate()
    {
        transform.rotation = Quaternion.Euler(0, 90 * (rotate_Count-1), 0);
    }

    // 저번 위치 설정
    private void CopyPreviousTransform()
    {
        prevTransform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    // Board 범위를 벗어날 시 전 위치로 롤백
    private void RollBackTransform(Transform prev)
    {
        transform.SetPositionAndRotation(prev.position, prev.rotation);
    }

    // Block WorldPosition 갱신 // Rotate순서에 따라 위치값 보정
    private void UpdateBlocksWorldPosition()
    {
        Vector3 rotOffset = Vector3.zero;

        switch (rot)
        {
            case Rot.Rot0:
                rotOffset = Vector3.zero;
                break;
            case Rot.Rot90:
                rotOffset = Vector3.left;
                break;
            case Rot.Rot180:
                rotOffset = Vector3.left + Vector3.forward;
                break;
            case Rot.Rot270:
                rotOffset = Vector3.forward;
                break;
            default:
                Debug.Log("그럴일은 없겠지만 ");
                break;
        }


        foreach (BlockHandler block in blocks)
        {
            block.UpdateWorldPositionToInt(rotOffset);
        }
    }

    private void CheckGridArrayDebug()
    {
        for (int i = 0; i < 22; i++)
        {  
            for (int j = 0; j < 10; j++)
            {
                Debug.Log($"grid[{i},{j}] : {tetris.grid.array[i, j]}");
            }
        }
    }

}

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
    //��� �׷쿡 ������Ʈ�� �޾Ƴ��´�. ���� ������ ����̸� �����̰� �Ѵ�.
    //���� ���õ� ������� �Ǵ��ϴ� ���� TetrisManager���� �޾ƿ´�.
    //�Է��� InputManager���� �����´�.
    [SerializeField] private InputManager input;
    [SerializeField] private TetrisManager tetris;
    [SerializeField] private BlockHandler[] blocks;
    public int rotate_Count;
    public bool isFieldTetromino = false;

    private int grid_X;
    private int grid_Y;

    private bool canMove;
    private int canMoveCount = 0;

    //�� ��ġ�� �ǵ����� ����
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
        // Input ����
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

            if(CheckBlocksOverlap()) // �̵��� �ߴµ�, block���� ���ƴٸ�
            {
                RollBackMove();
            }

            //todo �̵������� ����� �ڵ����� �������� timer �ʱ�ȭ // �Ʒ��θ� �̵�������?
        }
        //else if(false) // �̵��� �� ���� ���� ��Ͽ� ��Ҵٸ� ��������. �� ����̶��? �ǵ�����.
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

    // ��Ʈ�ι̳븦 ���Ҵٸ�, �������ٸ�
    private bool isPlaceTetromino()
    {

        return true;
    }

    // BlockHandler ������Ʈ �Ҵ�
    private void FindBlocks()
    {
        blocks = new BlockHandler[transform.childCount];

        for(int i = 0; i< transform.childCount; i++)
        {
            blocks[i] = transform.GetChild(i).GetComponent<BlockHandler>();
        }
    }

    // Tetromino�� �̵��� �� �ִ��� üũ, Board �����ȿ���
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
            Debug.Log("������");
            RollBackTransform(prevTransform); 
            return false;
        }
    }

    // Block���� �ٸ� Block��� �����ִ��� üũ
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

    // Move�� ��ġ�� �ѹ�
    private void RollBackMove()
    {
        grid_X -= input.tetromino_Move_X;
        grid_Y -= input.tetromino_Move_Y;
        transform.position = new Vector3(grid_X, 0, grid_Y);
    }

    // Rotate�� ��ġ�� �ѹ�
    private void RollBackRotate()
    {
        transform.rotation = Quaternion.Euler(0, 90 * (rotate_Count-1), 0);
    }

    // ���� ��ġ ����
    private void CopyPreviousTransform()
    {
        prevTransform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    // Board ������ ��� �� �� ��ġ�� �ѹ�
    private void RollBackTransform(Transform prev)
    {
        transform.SetPositionAndRotation(prev.position, prev.rotation);
    }

    // Block WorldPosition ���� // Rotate������ ���� ��ġ�� ����
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
                Debug.Log("�׷����� �������� ");
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

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
    //[SerializeField] private TetrominoSpawner spawner;
    [SerializeField] private BlockHandler[] blocks;
    public int rotate_Count = 0;
    public bool isFieldTetromino = false;

    private int grid_X;
    private int grid_Y;

    //�� ��ġ�� �ǵ����� ����
    private Transform prevTransform;
    public Vector3[] pivotPoint;
    private Vector3 offset = new Vector3(0.5f, 0, 0.5f);

    public Rot rot = Rot.Rot0;
    private Rot aheadRot = Rot.Rot0;

    private float timer = 0.8f;
    private float fall_ElapsedTime = 0f;

    private bool isOverlap = false; // ��� ��ħ ����

    private void Start()
    {
        input = GameObject.Find("GameManager").GetComponent<InputManager>();
        tetris = GameObject.Find("TetrisManager").GetComponent<TetrisManager>();
        
        if (transform.CompareTag("Tetromino_O") || transform.CompareTag("Tetromino_I")) offset = Vector3.zero;

        grid_X = (int)(transform.position.x + offset.x);
        grid_Y = (int)(transform.position.z + offset.z);
        transform.position = new Vector3(grid_X, 0, grid_Y) + offset;
        prevTransform = GetComponent<Transform>();

        FindBlocks();
        UpdateBlocksWorldPosition();
        //CheckGridArrayDebug();
    }

    private void Update()
    {
        // Input ����
        if (isFieldTetromino)
        {
            //UpdateBlocksWorldPosition();
            Fall();
            //CopyPreviousTransform();
            Move();
            Rotate();
            Drop();
            Place();
        }
        
    }

    // Tetromino�� �������°�
    private void Fall()
    {
        fall_ElapsedTime += Time.deltaTime;
        if(fall_ElapsedTime > timer)
        {
            fall_ElapsedTime = 0f;
            grid_Y -= 1;
            UpdateBlocksWorldPosition();
        }
    }

    private void Move()
    {
        if(input.tetromino_Move && CheckTetrominoCanMove())
        {
            grid_X += input.tetromino_Move_X;
            grid_Y += input.tetromino_Move_Y;
            transform.position = new Vector3(grid_X, 0, grid_Y) + offset;
            UpdateBlocksWorldPosition();

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
        if(input.tetromino_Rotate && CheckTetrominoCanRotate())
        {
            rot++;
            if (rot > Rot.Rot270) rot = Rot.Rot0;

            transform.rotation = Quaternion.Euler(0, 90 * (int)rot, 0);
            UpdateBlocksWorldPosition();

            if (CheckBlocksOverlap())
            {
                RollBackRotate();
            }
        }
    }

    private void Drop()
    {
        if(input.tetromino_Drop)
        {
            // block���� position.z���� ���ؾ߰���
            bool canDrop = true;
            while(canDrop) // grid index�� 1�̰ų� �� �����϶�����
            {
                grid_Y -= 1;
                UpdateBlocksWorldPosition();

                int count = 0;
                foreach (BlockHandler block in blocks)
                {
                    if (block.worldPosition.z == 0 || tetris.grid.array[(int)block.worldPosition.z - 1, (int)block.worldPosition.x] == 1)
                    {
                        count++;
                    }
                }

                if (count > 0)
                {
                    canDrop = false;
                }
                else
                {
                    canDrop = true;
                }

            }
        }
    }

    private void Place()
    {
        if(!isOverlap)
        {
            int count = 0;

            foreach (BlockHandler block in blocks)
            {
                if (block.IsPlaceBlock())
                {
                    count++;
                }
            }

            if (count > 0) // �ϳ��� ����̶� �������� ��Ȳ�̶��
            {
                //UpdateBlocksWorldPosition();
                foreach (BlockHandler block in blocks)
                {
                    block.PlaceBlock();
                }

                isFieldTetromino = false;
                tetris.spawner.Invoke("CreateTetromino", 0.2f);
                //tetris.spawner.CreateTetromino();
                //tetris.CheckLine();
                tetris.Invoke("CheckLine", 0.15f);
                //Time.timeScale = 0;

                
            }
        }
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

    #region CheckMove
    // Tetromino�� �̵��� �� �ִ��� üũ, Board �����ȿ���
    private bool CheckTetrominoCanMove()
    {
        // Move �� �� �������� �ѹ� �� �ռ������� ������ �������� �ƴ��� üũ
        Vector3 aheadInputPos = new Vector3(input.tetromino_Move_X, 0f, input.tetromino_Move_Y);

        int count = 0;
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i].CheckAheadBlockCanMove(aheadInputPos))
            {
                count++;
            }
        }

        if (blocks.Length == count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region CheckRotate
    // Tetromino�� Board ���� �ȿ��� ȸ���� �� �ִ� �� üũ
    private bool CheckTetrominoCanRotate()
    {
        if (input.tetromino_Rotate) aheadRot++;

        if (aheadRot > Rot.Rot270) aheadRot = Rot.Rot0;

        transform.rotation = Quaternion.Euler(0, 90 * (int)aheadRot, 0);

        Vector3[] aheadBlockPos = new Vector3[blocks.Length];
        Vector3 aheadOffset = RotOffset(aheadRot);

        int count = 0;
        for (int i = 0; i < blocks.Length; i++)
        {
            aheadBlockPos[i] = blocks[i].transform.position + aheadOffset;

            if (CheckAheadBlockCanRotate(aheadBlockPos[i]))
            {
                if (i == 3)
                {
                    //Debug.Log("Block���� worldposition�� ������ " + aheadBlockPos[i]);
                }

                count++;
            }
        }
        transform.rotation = Quaternion.Euler(0, -90 * (int)aheadRot, 0);

        if (blocks.Length == count)
        {
            Debug.Log("rotate true");
            return true;
        }
        else
        {
            Debug.Log("rotate false");
            return false;
        }
    }
    #endregion


    // Block���� �ٸ� Block��� �����ִ��� üũ
    private bool CheckBlocksOverlap()
    {
        UpdateBlocksWorldPosition();
        int count = 0;

        Debug.Log("���� ��ϰ�ġ�°� Ȯ���ϴ°Ŵ� ���´ܸ�����?");

        for(int i = 0; i < blocks.Length; i++)
        {                              
            if(tetris.grid.array[(int)blocks[i].worldPosition.z, (int)blocks[i].worldPosition.x] == 1)
            {
                count++;
            }
        }

        if(count > 0)
        {
            isOverlap = true;
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
        Debug.Log("RollBackMove ������ ������.");
        grid_X -= input.tetromino_Move_X;
        grid_Y -= input.tetromino_Move_Y;
        transform.position = new Vector3(grid_X, 0, grid_Y) + offset;
        UpdateBlocksWorldPosition();
    }

    // Rotate�� ��ġ�� �ѹ�
    private void RollBackRotate()
    {
        transform.rotation = Quaternion.Euler(0, 90 * (int)(rot - 1), 0);
        rot--;
        UpdateBlocksWorldPosition();
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

    // AheadBlock WorldPosition ����, Rotate
    private Vector3 RotOffset(Rot aheadRot)
    {
        Vector3 aheadRotOffset = Vector3.zero;

        switch (aheadRot)
        {
            case Rot.Rot0:
                aheadRotOffset = Vector3.zero;
                break;
            case Rot.Rot90:
                aheadRotOffset = Vector3.left;
                break;
            case Rot.Rot180:
                aheadRotOffset = Vector3.left + Vector3.forward;
                break;
            case Rot.Rot270:
                aheadRotOffset = Vector3.forward;
                break;
        }

        return aheadRotOffset;
    }

    // Block WorldPosition ���� // Rotate������ ���� ��ġ�� ����
    private void UpdateBlocksWorldPosition()
    {
        // �ڵ� ������ ���� �� ó���� block �������� �����Ҷ� ������ �ϴϱ�. block�� ���õ� �����ʱ�ȭ�� �� �޼��忡 �ִ°��� �´°� ������
        isOverlap = false;

        transform.position = new Vector3(grid_X, 0, grid_Y) + offset;

        Vector3 rotOffset = RotOffset(rot);

        foreach (BlockHandler block in blocks)
        {
            block.UpdateWorldPositionToInt(rotOffset);
        }
    }

    // board ���� �� üũ
    private bool CheckAheadBlockCanRotate(Vector3 aheadBlockPos)
    {
        if ((0 <= aheadBlockPos.x && aheadBlockPos.x < tetris.width) && (0 <= aheadBlockPos.y && aheadBlockPos.y <= tetris.height))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Drop ���� üũ
    private bool CanDrop()
    {
        int count = 0;
        foreach (BlockHandler block in blocks)
        {
            if (block.worldPosition.z == 0 || tetris.grid.array[(int)block.worldPosition.z - 1, (int)block.worldPosition.x] == 1)
            {
                count++;
            }
        }

        if(count > 0)
        {
            return false;
        }
        else
        {
            return true;
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

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
    //[SerializeField] private TetrominoSpawner spawner;
    [SerializeField] private BlockHandler[] blocks;
    public int rotate_Count = 0;
    public bool isFieldTetromino = false;

    private int grid_X;
    private int grid_Y;

    //전 위치로 되돌리는 변수
    private Transform prevTransform;
    public Vector3[] pivotPoint;
    private Vector3 offset = new Vector3(0.5f, 0, 0.5f);

    public Rot rot = Rot.Rot0;
    private Rot aheadRot = Rot.Rot0;

    private float timer = 0.8f;
    private float fall_ElapsedTime = 0f;

    private bool isOverlap = false; // 블록 겹침 변수

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
        // Input 조작
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

    // Tetromino가 떨어지는것
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
            // block들의 position.z들을 비교해야겠지
            bool canDrop = true;
            while(canDrop) // grid index가 1이거나 맨 밑줄일때까지
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

            if (count > 0) // 하나의 블록이라도 놓여지는 상황이라면
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

    // BlockHandler 컴포넌트 할당
    private void FindBlocks()
    {
        blocks = new BlockHandler[transform.childCount];

        for(int i = 0; i< transform.childCount; i++)
        {
            blocks[i] = transform.GetChild(i).GetComponent<BlockHandler>();
        }
    }

    #region CheckMove
    // Tetromino가 이동할 수 있는지 체크, Board 범위안에서
    private bool CheckTetrominoCanMove()
    {
        // Move 시 그 방향으로 한번 더 앞서나가서 범위를 나갔는지 아닌지 체크
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
    // Tetromino가 Board 범위 안에서 회전할 수 있는 지 체크
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
                    //Debug.Log("Block들의 worldposition과 같은지 " + aheadBlockPos[i]);
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


    // Block들이 다른 Block들과 겹쳐있는지 체크
    private bool CheckBlocksOverlap()
    {
        UpdateBlocksWorldPosition();
        int count = 0;

        Debug.Log("여기 블록겹치는거 확인하는거는 들어온단말이지?");

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

    // Move시 겹치면 롤백
    private void RollBackMove()
    {
        Debug.Log("RollBackMove 들어오긴 했으나.");
        grid_X -= input.tetromino_Move_X;
        grid_Y -= input.tetromino_Move_Y;
        transform.position = new Vector3(grid_X, 0, grid_Y) + offset;
        UpdateBlocksWorldPosition();
    }

    // Rotate시 겹치면 롤백
    private void RollBackRotate()
    {
        transform.rotation = Quaternion.Euler(0, 90 * (int)(rot - 1), 0);
        rot--;
        UpdateBlocksWorldPosition();
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

    // AheadBlock WorldPosition 갱신, Rotate
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

    // Block WorldPosition 갱신 // Rotate순서에 따라 위치값 보정
    private void UpdateBlocksWorldPosition()
    {
        // 코드 구조를 볼때 맨 처음에 block 포지션을 갱신할때 시작을 하니까. block과 관련된 변수초기화는 이 메서드에 넣는것이 맞는거 같은데
        isOverlap = false;

        transform.position = new Vector3(grid_X, 0, grid_Y) + offset;

        Vector3 rotOffset = RotOffset(rot);

        foreach (BlockHandler block in blocks)
        {
            block.UpdateWorldPositionToInt(rotOffset);
        }
    }

    // board 범위 안 체크
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

    // Drop 조건 체크
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

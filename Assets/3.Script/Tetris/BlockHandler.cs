using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHandler : MonoBehaviour
{
    // 테트로미노의 한 블록단위
    // 얘가 갖고있는 한 점은 grid의 한 index를 차지한다.
    // 얘의 포지션을 가지고 TetrominoController의 이동가능 유무를 판단하자.

    [SerializeField] private TetrisManager tetris;
    public Vector3 worldPosition;

    private void Start()
    {
        tetris = GameObject.Find("TetrisManager").GetComponent<TetrisManager>();
    }

    private void Update()
    {
        
    }

    public bool CheckBlockCanMove()
    {
        // 블록이 움직일 수 있는 공간 - Board, 좌표는 x 0~10, y 0~22
        // 블록이 움직일 수 없는 상황 - 블록이 놓여졌을 때,

        // 우선 보드안에서 움직일 수 있으면 참
        // 보드안에서 블록이 있다면 그냥 되돌리기해버려 움직일수 없는 상황을 구체적으로 말하면,
        // 다음 칸에 이동했을 때 블록이 겹쳐져있다면 이전 위치로 되돌리기
        if ((0 <= worldPosition.x && worldPosition.x <= tetris.width) && (0 <= worldPosition.y && worldPosition.y <= tetris.height))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckAheadBlockCanMove(Vector3 pos)
    {
        Vector3 aheadWorldPosition = worldPosition + pos;
        //Debug.Log("aheadpos : " + aheadWorldPosition);

        if ((0 <= aheadWorldPosition.x && aheadWorldPosition.x < tetris.width) && (0 <= aheadWorldPosition.y && aheadWorldPosition.y <= tetris.height))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckAheadBlockCanRotate(Vector3 aheadRotOffset)
    {
        Vector3 aheadWorldPosition = UpdateAheadWorldPositionToInt(aheadRotOffset);
        //Debug.Log("block의 aheadpos : " + aheadWorldPosition);

        if ((0 <= aheadWorldPosition.x && aheadWorldPosition.x < tetris.width) && (0 <= aheadWorldPosition.y && aheadWorldPosition.y <= tetris.height))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector3 UpdateAheadWorldPositionToInt(Vector3 aheadRotOffset)
    {
        Vector3 aheadWorldPosition = Vector3.zero;

        int x = Mathf.RoundToInt(transform.position.x + aheadRotOffset.x);
        int z = Mathf.RoundToInt(transform.position.z + aheadRotOffset.z);

        aheadWorldPosition.x = x;
        aheadWorldPosition.z = z;

        return aheadWorldPosition;
    }

    public void UpdateWorldPositionToInt(Vector3 rotOffset)
    {
        int x = Mathf.RoundToInt(transform.position.x + rotOffset.x);
        int z = Mathf.RoundToInt(transform.position.z + rotOffset.z);

        worldPosition.x = x;
        worldPosition.z = z;
    }

    // 하나의 블록이라도 놓여지는 상황이 발생된다면 Place 발생해야겠지
    public bool IsPlaceBlock()
    {
        // 1. 블록이 첫줄에 위치할때
        if(worldPosition.z == 0)
        {
            return true;
        }

        // 2. 해당 grid에 블록이 놓여져 있을때 배열의 x, z-1 값이 1이라면 // 바로 위에 위치해있다고 생각
        if(tetris.grid.array[(int)worldPosition.z-1, (int)worldPosition.x] == 1)
        {
            return true;
        }

        return false;
    }

    // 블록이 놓여지게 된다면 발생할 메서드, controller에서 호출하도록
    public void PlaceBlock()
    {
        tetris.grid.array[(int)worldPosition.z, (int)worldPosition.x] = 1;
    }
    
    

}

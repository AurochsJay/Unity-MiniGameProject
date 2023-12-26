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
        //worldPosition = transform.TransformPoint(transform.position);
        //UpdateWorldPositionToInt();
        //worldPosition = transform.position;
        Debug.Log($"x : {worldPosition.x}, z : {worldPosition.z}");
        //tetris.SyncGridPos(transform.position);
    }

    public bool CheckBlockCanMove()
    {
        // 블록이 움직일 수 있는 공간 - Board, 좌표는 x 0~10, y 0~22
        // 블록이 움직일 수 없는 상황 - 블록이 놓여졌을 때,
        bool canMove;

        // 우선 보드안에서 움직일 수 있으면 참
        // 보드안에서 블록이 있다면 그냥 되돌리기해버려 움직일수 없는 상황을 구체적으로 말하면,
        // 다음 칸에 이동했을 때 블록이 겹쳐져있다면 이전 위치로 되돌리기
        if ((0 <= worldPosition.x && worldPosition.x <= tetris.width) || (0 <= worldPosition.y && worldPosition.y <= tetris.height))
        {
            canMove = true;
        }
        else
        {
            canMove = false;
        }

        return canMove;
    }

    public void UpdateWorldPositionToInt(Vector3 rotOffset)
    {
        int x = Mathf.RoundToInt(transform.position.x + rotOffset.x);
        int z = Mathf.RoundToInt(transform.position.z + rotOffset.z);

        worldPosition.x = x;
        worldPosition.z = z;
    }

    
    

}

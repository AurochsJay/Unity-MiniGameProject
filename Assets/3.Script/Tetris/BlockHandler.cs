using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHandler : MonoBehaviour
{
    // ��Ʈ�ι̳��� �� ��ϴ���
    // �갡 �����ִ� �� ���� grid�� �� index�� �����Ѵ�.
    // ���� �������� ������ TetrominoController�� �̵����� ������ �Ǵ�����.

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
        // ����� ������ �� �ִ� ���� - Board, ��ǥ�� x 0~10, y 0~22
        // ����� ������ �� ���� ��Ȳ - ����� �������� ��,
        bool canMove;

        // �켱 ����ȿ��� ������ �� ������ ��
        // ����ȿ��� ����� �ִٸ� �׳� �ǵ������ع��� �����ϼ� ���� ��Ȳ�� ��ü������ ���ϸ�,
        // ���� ĭ�� �̵����� �� ����� �������ִٸ� ���� ��ġ�� �ǵ�����
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

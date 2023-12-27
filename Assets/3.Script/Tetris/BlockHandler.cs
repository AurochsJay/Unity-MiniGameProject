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
        
    }

    public bool CheckBlockCanMove()
    {
        // ����� ������ �� �ִ� ���� - Board, ��ǥ�� x 0~10, y 0~22
        // ����� ������ �� ���� ��Ȳ - ����� �������� ��,

        // �켱 ����ȿ��� ������ �� ������ ��
        // ����ȿ��� ����� �ִٸ� �׳� �ǵ������ع��� �����ϼ� ���� ��Ȳ�� ��ü������ ���ϸ�,
        // ���� ĭ�� �̵����� �� ����� �������ִٸ� ���� ��ġ�� �ǵ�����
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
        //Debug.Log("block�� aheadpos : " + aheadWorldPosition);

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

    // �ϳ��� ����̶� �������� ��Ȳ�� �߻��ȴٸ� Place �߻��ؾ߰���
    public bool IsPlaceBlock()
    {
        // 1. ����� ù�ٿ� ��ġ�Ҷ�
        if(worldPosition.z == 0)
        {
            return true;
        }

        // 2. �ش� grid�� ����� ������ ������ �迭�� x, z-1 ���� 1�̶�� // �ٷ� ���� ��ġ���ִٰ� ����
        if(tetris.grid.array[(int)worldPosition.z-1, (int)worldPosition.x] == 1)
        {
            return true;
        }

        return false;
    }

    // ����� �������� �ȴٸ� �߻��� �޼���, controller���� ȣ���ϵ���
    public void PlaceBlock()
    {
        tetris.grid.array[(int)worldPosition.z, (int)worldPosition.x] = 1;
    }
    
    

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoSpawner : MonoBehaviour
{
    // �� ����Ŭ�� 7���� ��Ʈ�ι̳�, �ΰ��� �迭�� �ʿ��ϴ�.
    // �� ���� �迭�� �ʿ��� ������ ���� ����� �̸� ������� �ϱ� ������
    // �� ��ũ��Ʈ�� ���������� ����� �����ϹǷ� ������ ������ TetrisManager���� �˷���� �Ѵ�.

    [SerializeField] private GameObject[] tetrominoGroup;
    private Vector3 spawnPos = new Vector3(5.5f, 0, 18.5f);
    public GameObject fieldTetromino;

    private void Start()
    {
        fieldTetromino = CreateTetromino();
    }

    public GameObject CreateTetromino()
    {
        int rand = Random.Range(0, tetrominoGroup.Length);

        GameObject tetromino = Instantiate(tetrominoGroup[1], spawnPos, Quaternion.identity);
        tetromino.GetComponent<TetrominoController>().isFieldTetromino = true;

        return tetromino;
    }
}

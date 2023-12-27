using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoSpawner : MonoBehaviour
{
    // 한 사이클에 7개의 테트로미노, 두개의 배열이 필요하다.
    // 두 개의 배열이 필요한 이유는 다음 블록을 미리 보여줘야 하기 때문에
    // 이 스크립트가 실질적으로 블록을 생성하므로 생성의 순서를 TetrisManager에게 알려줘야 한다.

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

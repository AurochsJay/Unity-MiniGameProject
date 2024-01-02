using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoSpawner : MonoBehaviour
{
    // 한 사이클에 7개의 테트로미노, 두개의 배열이 필요하다.
    // 두 개의 배열이 필요한 이유는 다음 블록을 미리 보여줘야 하기 때문에
    // 이 스크립트가 실질적으로 블록을 생성하므로 생성의 순서를 TetrisManager에게 알려줘야 한다.

    [SerializeField] private GameObject[] tetrominoGroup;
    [SerializeField] private Transform[] nextPos;
    private Vector3 spawnPos = new Vector3(4.5f, 0, 19.5f);
    //private Vector3 spawnPos = new Vector3(5.5f, 0, 14.5f);
    public GameObject fieldTetromino;

    private List<int> number_List = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
    public List<GameObject> createdTetrominos = new List<GameObject>();

    private void Start()
    {
        SetTetromino();
    }

    // 초기 설정, 필드 1개, Next칸에 3개
    private void SetTetromino()
    {
        int selectedNumber = NumberSelector();
        
        GameObject tetromino = Instantiate(tetrominoGroup[selectedNumber], spawnPos, Quaternion.identity);
        tetromino.GetComponent<TetrominoController>().isFieldTetromino = true;
        createdTetrominos.Add(tetromino);

        for(int i = 0; i< nextPos.Length; i++)
        {
            CreateTetromino(nextPos[i].position);
        }
    }

    // Tetromino 놓았을 때 실행될 메서드
    public void NextTetromino()
    {
        createdTetrominos.RemoveAt(0); // 필드(0번) Tetromino 리스트에서 제거
        
        for(int i = 0; i < 3; i++) // 위치 이동
        {
            if(i == 0)
            {
                createdTetrominos[i].transform.position = spawnPos;
                createdTetrominos[i].GetComponent<TetrominoController>().isFieldTetromino = true;
                createdTetrominos[i].GetComponent<TetrominoController>().isChangedFieldTetromino = true;
                createdTetrominos[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                createdTetrominos[i].transform.position = nextPos[i-1].position;
            }
        }

        CreateTetromino(nextPos[nextPos.Length - 1].position);
    }

    private void CreateTetromino(Vector3 Pos)
    {
        int selectedNumber = NumberSelector();

        GameObject tetromino = Instantiate(tetrominoGroup[selectedNumber], Pos, Quaternion.identity);
        if(tetromino.transform.CompareTag("Tetromino_I") || tetromino.transform.CompareTag("Tetromino_O"))
        {
            tetromino.transform.position += Vector3.forward*2;
            tetromino.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        createdTetrominos.Add(tetromino);
    }

    private int NumberSelector()
    {
        if (number_List.Count == 0)
        {
            FillNumber();
        }

        int randomIndex = Random.Range(0, number_List.Count);
        int selectedNumber = number_List[randomIndex];
        number_List.RemoveAt(randomIndex);

        return selectedNumber;
    }

    private void FillNumber()
    {
        for(int i = 0; i< tetrominoGroup.Length; i++)
        {
            number_List.Add(i);
        }
    }
}

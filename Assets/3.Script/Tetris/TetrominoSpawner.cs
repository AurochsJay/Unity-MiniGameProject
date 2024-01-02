using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrominoSpawner : MonoBehaviour
{
    // �� ����Ŭ�� 7���� ��Ʈ�ι̳�, �ΰ��� �迭�� �ʿ��ϴ�.
    // �� ���� �迭�� �ʿ��� ������ ���� ����� �̸� ������� �ϱ� ������
    // �� ��ũ��Ʈ�� ���������� ����� �����ϹǷ� ������ ������ TetrisManager���� �˷���� �Ѵ�.

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

    // �ʱ� ����, �ʵ� 1��, Nextĭ�� 3��
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

    // Tetromino ������ �� ����� �޼���
    public void NextTetromino()
    {
        createdTetrominos.RemoveAt(0); // �ʵ�(0��) Tetromino ����Ʈ���� ����
        
        for(int i = 0; i < 3; i++) // ��ġ �̵�
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

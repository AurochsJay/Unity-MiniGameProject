using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TetrisManager : MonoBehaviour
{
    [SerializeField] private InputManager input;
    [SerializeField] public Grid2D grid;
    [SerializeField] public TetrominoSpawner spawner;
    [SerializeField] private TetrominoController tetromino;
    [SerializeField] public GameObject[] dropPosCube;
    public readonly int width = 10;
    public readonly int height = 24;
    public bool isGameOver = false;


    private Vector3 offset = new Vector3(0.5f, 0, -0.5f);

    // Hold ����
    public bool hasHoldTetromino = false;

    [Header("UI")]
    [SerializeField] private GameObject startText;
    [SerializeField] private GameObject[] gameInfoImages;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject[] resultObject;
    [SerializeField] private Image gameOverImage;
    private int score = 0;
    private int lineCount = 0; // score ����
    private float playTime = 0; // Result
    private int clearLineCount = 0; // Result â�� ������ ��. � �μ̴���


    private void Start()
    {
        input = GameObject.Find("GameManager").GetComponent<InputManager>();
        grid.SetGrid(height, width);
    }

    private void Update()
    {
        if(input.tetromino_Start)
        {
            startText.gameObject.SetActive(false);
            foreach(GameObject gameInfoImage in gameInfoImages)
            {
                gameInfoImage.SetActive(true);
            }
            UpdateInfo();
            UpdateScore();
        }
        //CheckGridArrayDebug();
    }

    private void UpdateInfo()
    {
        playTime += Time.deltaTime;
    }

    public void CheckLine()
    {
        List<int> clearRows = new List<int>(); // �갡 ���� ���ĸ� Ŭ������ ���� ���� ���� ����Ʈ��
        bool isClear = false;
        lineCount = 0;

        for(int j = 0; j < 21; j++) // j==20 -> GameOver
        {
            int rowCount = 0;
            for(int i = 0; i < width; i++)
            {
                if(grid.array[j,i] == 1)
                {
                    rowCount++;
                }

                if(grid.array[20,i] == 1)
                {
                    isGameOver = true;
                }
            }

            if(rowCount == width) // ���� �� ä������ 
            {
                // Clear�� ������ �ѹ��� �� üũ�ϰ� Clear�� Line ���� ��������
                // ���� üũ�ϰ� �ٷ� Clear�ع����� ������ �ٷ� Pull�� �Ǽ� �����̻� �پ����� �� ������ ���Եȴ�.
                Debug.Log($"���� �� ä������ ����.{j}��");
                clearRows.Add(j);
                isClear = true;
                lineCount++;
                clearLineCount++;
            }
        }

        for(int j = 0; j < clearRows.Count; j++)
        {
            ClearLine(clearRows[j]);
        }


        if(isClear)
        {
            //Time.timeScale = 0;
            StartCoroutine(PullLine(clearRows));
        }

        if(isGameOver)
        {
            GameOver();
        }
    }

    private void ClearLine(int column)
    {
        for(int i = 0; i < width; i++)
        {
            grid.array[column, i] = 0;
            Vector3 destroyPoint = new Vector3(i, -0.5f, column) + offset;
            RaycastHit hit;
            if(Physics.Raycast(destroyPoint, Vector3.up * 2f, out hit))
            {
                Destroy(hit.transform.gameObject);
            }
        }
    }

    // Line Clear�ϸ� ���� ������ ���ܿ;��� // �� �� �̻� ���ÿ� �������? �ѹ��� �ִ� 4�ٻ���
    private IEnumerator PullLine(List<int> clearRows)
    {
        yield return new WaitForSeconds(0.05f);

        for(int j = clearRows[0]; j < 20; j++)
        {
            int count = 0;
            for (int row = 0; row < clearRows.Count; row++)
            {
                if (j > clearRows[row])
                {
                    count++;
                }
            }

            for(int i = 0; i < width; i++)
            {
                Vector3 rayPoint = new Vector3(i, -0.5f, j) + offset;
                RaycastHit hit;
                if (Physics.Raycast(rayPoint, Vector3.up * 2f, out hit))
                {
                    // ó�� clear�� ������ �ݺ��� ���ư��ϱ� ���࿡ �¾Ҵٸ� �� ���� clear�� �ȵ� ���̴�.
                    //Debug.DrawRay(rayPoint, Vector3.up * 5f, Color.red, Mathf.Infinity);
                    hit.transform.position += new Vector3(0,0, -count);
                    grid.array[j, i] = 0; // ���� ��ġ�� ����� ���ԵǴϱ� index value 0
                    grid.array[j - count, i] = 1; // ����� ��ġ�� ����� �ְԵǴϱ� index value 1
                }
            }
            
        }
    }

    private void UpdateScore()
    {
        CalculateScore();

        scoreText.text = $"Score : {score}";
    }

    private void CalculateScore()
    {
        if(lineCount > 0 )
        {
            float scoreMulti = 1f;
            switch (lineCount)
            {
                case 1:
                    scoreMulti = 1f;
                    break;
                case 2:
                    scoreMulti = 1.4f;
                    break;
                case 3:
                    scoreMulti = 1.7f;
                    break;
                case 4:
                    scoreMulti = 2f;
                    break;
            }

            score += 10 * (int)(lineCount * scoreMulti);
            lineCount = 0;
        }
        
        if(score > 1000)
        {
            GameOver();
        }

    }

    // ���ӿ���
    private void GameOver()
    {
        gameOverCanvas.SetActive(true);
        GameManager.instance.coin += (int)(score / 10);
        StartCoroutine(ShowResultUI());
    }

    private IEnumerator ShowResultUI()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.8f);

        for(int i = 0; i < resultObject.Length; i++)
        {
            resultObject[i].SetActive(true);

            switch (i)
            {
                case 1:
                    resultObject[i].GetComponent<Text>().text = $"Time  : {(int)playTime/60:00}:{(int)playTime%60:00}";
                    break;
                case 2:
                    resultObject[i].GetComponent<Text>().text = $"Score : {score}";
                    break;
                case 3:
                    resultObject[i].GetComponent<Text>().text = $"Coin  : {(int)(score/10)}";
                    break;
            }

            yield return wfs;
        }
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void SyncGridPos(Vector3 block) => grid.array[(int)block.z, (int)block.x] = 1;

    private void CheckGridArrayDebug()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Debug.Log($"grid[{i},{j}] : {grid.array[i, j]}");
            }
        }
    }

}

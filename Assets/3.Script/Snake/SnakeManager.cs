using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SnakeManager : MonoBehaviour
{
    [SerializeField] private InputManager input;
    [SerializeField] public Grid2D grid;
    [SerializeField] private GameObject target;

    private readonly int height = 22;
    private readonly int width = 22;

    private Vector3 startPos = new Vector3(5, 0, 10);

    public bool isGameover = false;

    [Header("UI")]
    [SerializeField] private GameObject startText;
    [SerializeField] private GameObject snakeCoin;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject[] resultObject;
    public int coin_Count = 0;
    private float playTime = 0; // Result
    private float time_Multiple = 1f;

    private void Awake()
    {
        input = GameObject.Find("GameManager").GetComponent<InputManager>();
        grid.SetGrid(height, width);
        SetGridToWall();
        CreateTarget();
    }

    private void Update()
    {
        if(input.snake_Start)
        {
            startText.gameObject.SetActive(false);
            snakeCoin.SetActive(true);
            UpdateInfo();
        }
    }

    // ±×¸®µå¿¡ Wall ¼³Á¤, grid.array[y,x] == 0:ºóÄ­, 1:SnakeÄ­, 2:TargetÄ­, 3:WallÄ­
    private void SetGridToWall()
    {
        for(int j=0; j<height; j++)
        {
            for(int i=0; i<width; i++)
            {
                if(i == 0 || i == width-1 || j == 0 || j == height-1)
                {
                    grid.array[j, i] = 3;
                }
            }
        }
    }

    // Target ·£´ý »ý¼º <- ÃÊ±â ¹× Snake°¡ TargetÀ» ¸ÔÀ¸¸é ½ÇÇà
    public void CreateTarget()
    {
        List<Vector2> ableNumber = new List<Vector2>();
            
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                if (grid.array[j, i] == 1 || grid.array[j, i] == 3)
                {
                    continue;
                }
                else
                {
                    ableNumber.Add(new Vector2(i, j));
                }
            }
        }

        int rand = Random.Range(0, ableNumber.Count);

        grid.array[(int)ableNumber[rand].y, (int)ableNumber[rand].x] = 2;
        target.transform.position = new Vector3((int)ableNumber[rand].x, 0.5f, (int)ableNumber[rand].y);
    }

    private void UpdateInfo()
    {
        playTime += Time.deltaTime;

        snakeCoin.GetComponentInChildren<Text>().text = $"SnakeCoin : {coin_Count}";
    }

    public void GameOver()
    {
        isGameover = true;
        gameOverCanvas.SetActive(true);
        CalculateTimeMultiple();
        GameManager.instance.coin += (int)(coin_Count * time_Multiple);
        StartCoroutine(ShowResultUI());
    }

    private IEnumerator ShowResultUI()
    {
        WaitForSeconds wfs = new WaitForSeconds(0.8f);

        for (int i = 0; i < resultObject.Length; i++)
        {
            resultObject[i].SetActive(true);

            switch (i)
            {
                case 1:
                    resultObject[i].GetComponent<Text>().text = $"Time  : {(int)playTime / 60:00}:{(int)playTime % 60:00}";
                    break;
                case 2:
                    resultObject[i].GetComponent<Text>().text = $"Snake Coin : {coin_Count}";
                    break;
                case 3:
                    resultObject[i].GetComponent<Text>().text = $"Coin  : {(int)(coin_Count * time_Multiple)}";
                    break;
            }

            yield return wfs;
        }
    }

    private void CalculateTimeMultiple()
    {
        int count = (int)(playTime / 60);
        time_Multiple = 1f + ((1 / 10) * count);
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
  
}

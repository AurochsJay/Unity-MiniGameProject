using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JJumpManager : MonoBehaviour
{
    [SerializeField] private InputManager input;
    // 해당 번호의 라인을 사용했는지. 라인마다 섬들을 한번만 생성
    public bool[] usedLineNumber = new bool[5];

    public bool isGameover = false;
    public int coin_Count = 0;
    public int coinMultiple = 1; // 코인반환하면 X2배

    [Header("UI")]
    [SerializeField] private GameObject startText;
    [SerializeField] private GameObject jjumpCoin;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject[] resultObject;
    [SerializeField] public GameObject coinReturn;
    [SerializeField] public Text coinReturnText;
    private float playTime = 0; // Result

    private void Awake()
    {
        input = GameObject.Find("GameManager").GetComponent<InputManager>();
        InitiateSetting();
    }

    private void Update()
    {
        if(input.JJump_Start)
        {
            startText.gameObject.SetActive(false);
            jjumpCoin.SetActive(true);
            UpdateInfo();
        }

        if(isGameover)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void UpdateInfo()
    {
        playTime += Time.deltaTime;

        jjumpCoin.GetComponentInChildren<Text>().text = $"JJumpCoin : {coin_Count}";
    }

    public void GameOver()
    {
        coinReturn.SetActive(false);
        isGameover = true;
        gameOverCanvas.SetActive(true);
        GameManager.instance.coin += (int)(coin_Count * coinMultiple);
        StartCoroutine(ShowResultUI());
        input.JJump_Start = false;
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
                    resultObject[i].GetComponent<Text>().text = $"JJump Coin : {coin_Count}";
                    break;
                case 3:
                    resultObject[i].GetComponent<Text>().text = $"Coin  : {(int)(coin_Count * coinMultiple)}";
                    break;
            }

            yield return wfs;
        }
    }

    public void GoToLobby()
    {
        GameManager.instance.audio.clip = null;
        GameManager.instance.presentScene = Scene.Lobby;
        SceneManager.LoadScene("Lobby");
    }

    private void InitiateSetting()
    {
        for(int i =0; i<usedLineNumber.Length; i++)
        {
            usedLineNumber[i] = false;
        }
    }

}

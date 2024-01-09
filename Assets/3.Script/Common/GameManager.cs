using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Scene
{
    MainMenu,
    Lobby,
    Tetris,
    Snake,
    JumpUp
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int coin = 0;

    public Scene presentScene;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        presentScene = Scene.Tetris;
    }

    private void Start()
    {
        //presentScene = Scene.Snake;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Scene
{
    MainMenu,
    Lobby,
    Tetris,
    Snake,
    JJump
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int coin = 0;

    public Scene presentScene;

    [SerializeField] public SoundManager soundManager;

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
    }

    private void Start()
    {
        presentScene = Scene.MainMenu;
        soundManager = GetComponent<SoundManager>();
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        PlayBackgroundMusic();
    }

    private void PlayBackgroundMusic()
    {
        if(!audioSource.isPlaying)
        {
            switch (GameManager.instance.presentScene)
            {
                case Scene.Tetris:
                    PlayClip(clips[0]);
                    break;
                case Scene.Snake:
                    PlayClip(clips[1]);
                    break;
                case Scene.JJump:
                    PlayClip(clips[2]);
                    break;
            }
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null)
        {
            // AudioSource를 사용하여 클립을 재생
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}

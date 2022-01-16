using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
    Music On/Off Toggle
 */
public class AudioManager : MonoBehaviour
{
    public Sprite musicOn;
    public Sprite musicOff;
    public GameObject music;
    public AudioClip mainMenuClip;
    public AudioClip startGameClip;
    public AudioClip speedUpClip;
    public AudioClip gameOverClip;
    private AudioSource g_audio;

    private void Awake()
    {
        g_audio = GameObject.Find("AudioController").GetComponent<AudioSource>();
    }

    private void Start()
    {
        g_audio.mute = false;
        music.GetComponent<Image>().sprite = musicOn;
        music.GetComponent<Button>().onClick.AddListener(ToggleMute);
    }

    /*
     MainMenu:
     MainMenu에 해당되는 음악 틀기

     사용:
     SetUp.MainMenu
     */

    public void MainMenu()
    {
        PlayerClip(mainMenuClip);
    }

    /*
     StartGame:
     StartGame에 해당되는 음악 틀기

     사용:
     SetUp.StartGame
     TestPlayerScript
     */

    public void StartGame()
    {
        PlayerClip(startGameClip);
    }

    /*
     SpeedUp:
     SpeedUp에 해당되는 음악 틀기

     사용:
     TestPlayerScript
     */

    public void SpeedUp()
    {
        PlayerClip(speedUpClip);
    }

    /*
     GameOver:
     GameOver에 해당되는 음악 틀기

     사용:
     SetUp.GameOver
     */

    public void GameOver()
    {
        PlayerClip(gameOverClip);
    }


    private void ToggleMute()
    {
        if (g_audio.mute) UnMute();
        else Mute();
    }

    private void Mute()
    {
        g_audio.mute = true;
        music.GetComponent<Image>().sprite = musicOff;
    }

    private void UnMute()
    {
        g_audio.mute = false;
        music.GetComponent<Image>().sprite = musicOn;
    }
    private void PlayerClip(AudioClip clip)
    {
        if (g_audio.clip != clip)
        {
            g_audio.clip = clip;
            g_audio.Play();
        }
    }
}

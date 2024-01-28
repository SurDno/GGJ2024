using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class audioManager : MonoBehaviour
{

    [SerializeField] AudioClip gameMusic;
    

   
    public AudioClip mainMenu;

    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        PlayMusicBasedOnScene();


    }


    public void PlayMusicBasedOnScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        switch (currentScene.name)
        {
            case "Menu":
                PlayMenuMusic();
                break;
            case "Game":
                PlayAmbient();
                break;
            default:
                Debug.LogWarning("Scene not recognized for music playback.");
                break;

        }
    }


   

    public void PlayAmbient()
    {
        audioSource.clip = gameMusic;
        audioSource.Play();
    }

    public void PlayMenuMusic()
    {
        audioSource.clip = mainMenu;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }


}
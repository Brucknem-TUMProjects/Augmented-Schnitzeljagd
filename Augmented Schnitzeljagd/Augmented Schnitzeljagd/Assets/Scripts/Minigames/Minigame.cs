using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Minigame : MonoBehaviour
{

    public bool Win { get; set; }
    public bool Running { get; set; }


    [Header("Minigame Settings")]
    [Range(1, 5)]
    [SerializeField]
    private int difficulty;

    void Start()
    {
        Running = true;
        startGame();
    }

    // override in child class for indivual start setup
    protected virtual void startGame() { }

    // override in child class for indivual end setup
    protected virtual void endGame()
    {
        Running = false;
        StopAllCoroutines();
        if (Win)
        {
            Debug.Log("win");
            GameData.Instance.Me.RouteProgress++;
            try
            {
                GameData.Instance.UpdateMeOnServer();
                SceneManager.LoadScene("MainMenu");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        else
        {
            Debug.Log("lose");
            SceneManager.LoadScene("MainMenu");
        }
    }

    public int getDifficulty()
    {
        return difficulty;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhackAMole : Minigame {

#region Variabels
    private int maxMoles;
    private int nextMole;
    private int molesLeft;

    public Button[] buttons;

    [Header("Time until a mole reappears")]
    private float swapTimeMax = 1.0f;
    private float swapTimeMin = 0.2f;
    private float swapTime = 2;

    private float nextHop;

    public Text t;
#endregion

    protected override void startGame () {

        // difficulty
        maxMoles = 5*getDifficulty();
        swapTimeMax = 6-getDifficulty();
        swapTimeMin = 1.0f/getDifficulty();

        nextHop = Time.time + swapTime;

        for (int i = 0; i < buttons.Length; i++)
            buttons[i].interactable = false;
        Win = false;
        molesLeft = maxMoles;
	}
	
	void Update () {
        if (Win)
            return;
        if (molesLeft <= 0 && Running)
        {
            Win = true;
            endGame();
        }
        if(Time.time > nextHop)
        {
            swapTime = Random.Range(swapTimeMin, swapTimeMax);
            nextHop += swapTime;
            buttons[nextMole].interactable = false;
            nextMole = Random.Range(0, buttons.Length);
            buttons[nextMole].interactable = true;
        }
    }


    void OnGUI()
    {
        t.text = "Whack the moles\nRemaining:\t" + molesLeft;
        if (molesLeft <= 0)
            t.text = "Win";
    }

    public void hitMole()
    {
        molesLeft--;
        buttons[nextMole].interactable = false;
    }
}

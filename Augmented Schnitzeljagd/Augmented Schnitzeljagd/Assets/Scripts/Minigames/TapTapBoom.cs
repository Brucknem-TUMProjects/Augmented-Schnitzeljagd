using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapTapBoom : Minigame {

    public Button[] buttons;
    private int[] field;
    private int traps;
    private int trapsLeft;
    private int targets;
    private int targetsLeft;

    public Text t;

    // Use this for initialization
    protected override void startGame ()
    {
        field = new int[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            field[i] = 0;
            buttons[i].image.color = new Color(1, 1, 1);
            buttons[i].interactable = true;
        }

        // set traps & targets depending on difficulty
        targets = getDifficulty();
        traps = (int) Mathf.Ceil((targets / 2.0f));

        trapsLeft = traps;
        targetsLeft = targets;

        int x;
        for (int j = 0; j < targets; j++)
        {
            x = Random.Range(0, buttons.Length);
            if (field[x] == 0)
                field[x] = 1;
            else
                j--;
        }

        for (int k = 0; k < traps; k++)
        {
            x = Random.Range(0, buttons.Length);
            if (field[x] == 0)
                field[x] = 2;
            else
                k--;
        }
	}
	
    public void pressButton(int x)
    {
        if (Running)
        {
            buttons[x].interactable = false;

            //Debug.Log("Press " + x);
            if (field[x] == 1)
            {
                buttons[x].image.color = new Color(0, 1, 0);
                targetsLeft--;
                if (targetsLeft <= 0)
                {
                    Win = true;
                    endGame();
                }
            }
            else if (field[x] == 2)
            {
                buttons[x].image.color = new Color(1, 0, 0);
                StartCoroutine(trap());
            }
        }
    }

    IEnumerator trap()
    {
        Running = false;
        yield return new WaitForSeconds(.5f);
        startGame();
        Running = true;
    }

    void OnGUI () {
        t.text = "Find all green  fields, avoid the red ones\nGreen\tfields:\t" + targetsLeft + "\nRed\t\tfields:\t" + trapsLeft;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchInterface : MonoBehaviour {
    MainMenu mainMenu;

    public void SetMainMenu(MainMenu mainMenu)
    {
        this.mainMenu = mainMenu;
    }

    public void SelectTab(int tab)
    {
        mainMenu.SelectTab(tab);
    }
}

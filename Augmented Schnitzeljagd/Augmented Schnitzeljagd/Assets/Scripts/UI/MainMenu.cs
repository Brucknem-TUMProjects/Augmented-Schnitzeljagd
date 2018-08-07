using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public RectTransform windowsParent;

    readonly int INITIAL_WINDOW = 2;

    Image[] tabs;
    RectTransform[] windows;
    int selected;

    AsyncOperation[] asyncs;
    bool scenesLoaded;

    //---------------------------------------------------------------------------------------------//
    //---------------------------------------------------------------------------------------------//
    // Use this for initialization
    void Start()
    {
        asyncs = new AsyncOperation[5];
        asyncs[0] = SceneManager.LoadSceneAsync("ProfileTab", LoadSceneMode.Additive);
        asyncs[1] = SceneManager.LoadSceneAsync("FriendsTab", LoadSceneMode.Additive);
        asyncs[2] = SceneManager.LoadSceneAsync("MapTab", LoadSceneMode.Additive);
        asyncs[3] = SceneManager.LoadSceneAsync("ShopTab", LoadSceneMode.Additive);
        asyncs[4] = SceneManager.LoadSceneAsync("Tabs", LoadSceneMode.Additive);
    }

    void SetMenu()
    {
        // Set all Windows
        GameObject[] windowsOfOtherScenes = GameObject.FindGameObjectsWithTag("Window");
        for (int i = 0; i < windowsOfOtherScenes.Length; i++)
        {
            windowsOfOtherScenes[i].transform.parent = windowsParent;
        }
        windows = new RectTransform[windowsOfOtherScenes.Length];
        for (int i = 0; i < windows.Length; i++)
        {
            windows[i] = windowsParent.GetChild(i).GetComponent<RectTransform>();
            windows[i].offsetMax = Vector2.zero;
        }
        // Position of all Windows
        selected = INITIAL_WINDOW;
        for (int i = 0; i < windows.Length; i++)
        {
            windows[i].anchoredPosition = new Vector2((i - INITIAL_WINDOW) * windows[i].rect.width, windows[i].anchoredPosition.y);
        }

        // Set all Tabs
        GameObject tabsOfOtherScenes = GameObject.Find("Tabs");
        tabs = tabsOfOtherScenes.GetComponentsInChildren<Image>();
        tabsOfOtherScenes.GetComponent<SwitchInterface>().SetMainMenu(this);
        UpdateTabColor();


        // Finish Setting
        GameObject[] loadings = GameObject.FindGameObjectsWithTag("Loading");
        foreach (GameObject elem in loadings)
        {
            elem.SetActive(false);
        }
    }

    //---------------------------------------------------------------------------------------------//
    //---------------------------------------------------------------------------------------------//
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

            #region LoadScene
            if (!scenesLoaded)
        {
            bool allLoaded = true;
            foreach (AsyncOperation a in asyncs)
            {
                if (!a.isDone)
                {
                    allLoaded = false;
                }
            }
            if (allLoaded)
            {
                scenesLoaded = true;
                SetMenu();
            }

            return;
        }
        #endregion

        #region SwitchTab
        Vector2 toPos = (selected - INITIAL_WINDOW) * Vector2.left * windowsParent.rect.width;
        Vector2 direction = toPos - windowsParent.anchoredPosition;
        if (direction.magnitude < 2f)
        {
            windowsParent.anchoredPosition = toPos;
        }
        else if (direction.magnitude != 0)
        {
            windowsParent.anchoredPosition = Vector2.Lerp(windowsParent.anchoredPosition,toPos,Time.deltaTime*10);
        }
        #endregion
    }

    void UpdateTabColor()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (i == selected)
            {
                tabs[i].color = Color.gray;
            }
            else
            {
                tabs[i].color = Color.white;
            }
        }
    }

    public void SelectTab(int tab)
    {
        selected = tab;
        UpdateTabColor();
        windows[tab].GetComponent<AbstractTab>().OnSwitchToThisTab();
    }
}

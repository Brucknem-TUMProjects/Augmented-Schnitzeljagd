using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileTab : AbstractTab
{
    [Header("Skins Chooser")]
    public RectTransform skinsGrid;
    public GameObject skinsViewPrefab;
    public Image skinViewer;
    public SpriteHolder spriteHolder;
    public Text playerName;

    RectTransform[] profileTabs;
    int selected;

    [Header("Stats Viewer")]
    public Text[] views;

    // Use this for initialization
    void Start()
    {
        profileTabs = new RectTransform[2];
        profileTabs[0] = transform.GetChild(0).GetComponent<RectTransform>();
        profileTabs[1] = transform.GetChild(1).GetComponent<RectTransform>();

        playerName.text = GameData.Instance.Me.Username;

        profileTabs[1].anchoredPosition = new Vector2(profileTabs[1].anchoredPosition.x, -1 * profileTabs[1].rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        #region Init
        if (profileTabs[1].anchoredPosition != new Vector2(profileTabs[1].anchoredPosition.x, -1 * profileTabs[1].rect.height))
        {
            profileTabs[1].anchoredPosition = new Vector2(profileTabs[1].anchoredPosition.x, -1 * profileTabs[1].rect.height);
        }
        #endregion

        #region SwitchTab
        Vector2 toPos = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, selected * GetComponent<RectTransform>().rect.height);
        Vector2 direction = toPos - GetComponent<RectTransform>().anchoredPosition;
        if (direction.magnitude < 2f)
        {
            GetComponent<RectTransform>().anchoredPosition = toPos;
        }
        else if (direction.magnitude != 0)
        {
            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GetComponent<RectTransform>().anchoredPosition, toPos, Time.deltaTime * 10);
        }
        #endregion
    }

    public void SelectTab(int tab)
    {
        selected = tab;
        OnSwitchToThisTab();
    }

    public override void OnSwitchToThisTab()
    {
        UserStats us = (UserStats)GameData.Instance.RequestUserStats(GameData.Instance.Me.UserID);
        skinViewer.sprite = spriteHolder.GetSprite(GameData.Instance.Me.CurrentSkin);
        if (selected == 0)
        {
            foreach (Transform child in skinsGrid.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (string s in us.Skins)
            {
                GameObject skinView = Instantiate(skinsViewPrefab);
                skinView.transform.parent = skinsGrid;
                skinView.transform.localScale = Vector3.one;
                Button skinButton = skinView.GetComponentInChildren<Button>();
                skinButton.GetComponentInChildren<Text>().text = s;
                skinButton.onClick.AddListener(delegate { OnChooseThisSkin(s); });
            }
        }

        if (selected == 1)
        {
            views[0].text = "Experience: " + us.Experience;
            views[1].text = "Coins: " + us.Coins;
            views[2].text = "Finished Routes: " + us.FinishedRoutes;
            views[3].text = "Created Routes: " + us.CreatedRoutes;
            views[4].text = "Time played: " + us.PlayTime;
        }
    }

    private void OnChooseThisSkin(string skinName)
    {
        foreach (Sprite s in spriteHolder.spritesToLoad)
        {
            if (s.name.Equals(skinName))
            {
                Debug.Log(skinName + " --- " + s.name);
                skinViewer.sprite = s;
                GameData.Instance.Me.CurrentSkin = skinName;
                Debug.Log(GameData.Instance.Me.CurrentSkin);
                GameData.Instance.UpdateMeOnServer();
                return;
            }
        }
    }
}

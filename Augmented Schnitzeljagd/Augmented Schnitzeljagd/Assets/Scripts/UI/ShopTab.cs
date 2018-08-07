using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopTab : AbstractTab
{

    [Header("Skins Chooser")]
    public RectTransform skinsGrid;
    public GameObject buyViewPrefab;

    public SpriteHolder spriteHolder;


    // Use this for initialization
    void Start()
    {
        foreach (Transform child in skinsGrid.transform)
        {
            if (child.tag != "Category")
                GameObject.Destroy(child.gameObject);
        }
        UserStats us = (UserStats)GameData.Instance.RequestUserStats(GameData.Instance.Me.UserID);
        foreach (string s in Strings.SKIN_LIST)
        {
            GameObject buyView = Instantiate(buyViewPrefab);
            buyView.transform.parent = skinsGrid;
            buyView.transform.localScale = Vector3.one;
            buyView.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = s;
            Button skinButton = buyView.GetComponentInChildren<Button>();

            for (int i = 0; i < spriteHolder.spritesToLoad.Length; i++)
            {
                if (spriteHolder.spritesToLoad[i].name.Equals(s))
                {
                    buyView.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = spriteHolder.spritesToLoad[i];
                    skinButton.GetComponentInChildren<Text>().text = spriteHolder.prices[i].ToString() + "$";
                }
            }

            skinButton.onClick.AddListener(delegate { OnBuyThisSkin(s, buyView.transform.GetChild(1).gameObject); });

            
            if (us.Skins.Contains(s))
            {
                buyView.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        skinsGrid.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnSwitchToThisTab()
    {

    }

    private void OnBuyThisSkin(string skinName, GameObject panel)
    {
        UserStats us = (UserStats)GameData.Instance.RequestUserStats(GameData.Instance.Me.UserID);

        bool canBeBought = true ;
        int price = 0;
        for(int i = 0; i < spriteHolder.spritesToLoad.Length; i++)
        {
            if (spriteHolder.spritesToLoad[i].name.Equals(skinName))
            {
                if (us.Coins < spriteHolder.prices[i])
                {
                    canBeBought = false;
                    price = spriteHolder.prices[i];
                }
            }
        }

        if (!canBeBought)
            return;

        panel.SetActive(true);

        us.Skins.Add(skinName);
        us.Skins.Sort();
        us.Coins -= price;
        GameData.Instance.UpdateUserStats(us);
    }
}
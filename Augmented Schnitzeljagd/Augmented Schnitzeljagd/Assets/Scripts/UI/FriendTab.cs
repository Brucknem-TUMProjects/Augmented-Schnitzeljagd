using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendTab : AbstractTab
{
    public RectTransform grid;
    public GameObject friendsView;

    private void Start()
    {
        OnSwitchToThisTab();
    }

    public override void OnSwitchToThisTab()
    {
        Debug.Log("OnSwitchToThisTab");
        foreach (Transform child in grid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (string s in GameData.Instance.Me.FriendsIDs)
        {
            try
            {
                UserStats us = GameData.Instance.RequestUserStats(s);


                GameObject friend = Instantiate(friendsView);
                friend.transform.parent = grid;
                friend.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                Dropdown dd = friend.GetComponent<Dropdown>();

                dd.ClearOptions();

                string name = GameData.Instance.RequestUser(s).Username;
                dd.options.Add(new Dropdown.OptionData() { text = name, });
                dd.options.Add(new Dropdown.OptionData() { text = "Experience: " + us.Experience, });
                dd.options.Add(new Dropdown.OptionData() { text = "Finished Routes: " + us.FinishedRoutes, });
                dd.options.Add(new Dropdown.OptionData() { text = "Created Routes: " + us.CreatedRoutes, });
                dd.options.Add(new Dropdown.OptionData() { text = "Time played: " + us.PlayTime, });
                dd.options.Add(new Dropdown.OptionData() { text = "Delete this friend", });
                dd.onValueChanged.AddListener(delegate { OnDeleteFriend(dd, us.GetProperties()[Strings.USER_ID].GetValue(us, null).ToString()); });

                dd.value = 1;
                dd.value = 0;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    public void OnDeleteFriend(Dropdown dd, string id)
    {
        if (dd.value == dd.options.Count-1)
        {
            GameData.Instance.Me.FriendsIDs.Remove(id);
            GameData.Instance.UpdateMeOnServer();
            OnSwitchToThisTab();
        }
        else
        {
            dd.value = 0;
        }
    }
}
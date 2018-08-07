using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {
    public GameObject text;
	// Use this for initialization
	void Start ()
    {
        GameObject username = Instantiate(text);
        username.GetComponent<Text>().text = GameData.Instance.Me.Username;
        username.transform.parent = this.transform;
        GameObject userId = Instantiate(text);
        userId.GetComponent<Text>().text = GameData.Instance.Me.UserID;
        userId.transform.parent = this.transform;
        GameObject password = Instantiate(text);
        password.GetComponent<Text>().text = GameData.Instance.Me.Password;
        password.transform.parent = this.transform;
        GameObject friends = Instantiate(text);
        friends.GetComponent<Text>().text = "Friends";
        friends.transform.parent = this.transform;
        foreach (string u in GameData.Instance.Me.FriendsIDs)
        {
            GameObject friend = Instantiate(text);
            friend.GetComponent<Text>().text = u;
            friend.transform.parent = this.transform;
        }
    }
}

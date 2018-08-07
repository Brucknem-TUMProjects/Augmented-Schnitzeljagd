using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationPauseManager : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        Debug.Log("Has focus: " + hasFocus);
        if(GameData.Instance.Me != null)
        {
            GameData.Instance.Me.IsLoggedIn = hasFocus;
            try
            {
                GameData.Instance.UpdateMeOnServer();
            }catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log("Paused: " + pauseStatus);

        if (GameData.Instance.Me != null)
        {
            GameData.Instance.Me.IsLoggedIn = !pauseStatus;

            try
            {
                GameData.Instance.UpdateMeOnServer();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}

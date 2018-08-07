using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviour {

    public InputField ipIF, portIF;
    public Button connect;
    public GameObject message;
    public Text[] debugText = new Text[20];

    [Range(1, 10)]
    public int reconnectDelay;

    private string ip = "", port = "";


	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this);
        connect.onClick.AddListener(delegate { OnConnect(); });
	}
	
	// Update is called once per frame
	void Update () {

        message.SetActive(!GameData.Instance.isConnected);
        //debugText[3].text = GameData.Instance.isConnected.ToString();
        //debugText[4].text = GameData.Instance.serverIP.ToString();
        //debugText[5].text = GameData.Instance.port.ToString();
        //debugText[6].text = GameData.Instance.standartIP;
        //debugText[7].text = GameData.Instance.threadsRunning.ToString();

        for(int i = 8; i < debugText.Length; i++)
        {
            //debugText[i].text = (i-8) + ":" + GameData.Instance.texts[i - 8];
        }

        if (!GameData.Instance.isConnected)
        {
            try
            {
                //debugText[0].text = "Connect try";
                GameData.Instance.ConnectServer(ip, port);
            }catch(Exception e){
                //debugText[1].text = e.Message;
                Debug.Log(e.Message);
            }
        }
	}

    public void OnConnect()
    {
        //debugText[2].text = "OnConnect( " + ipIF.text + ", " + portIF.text + ")";
        Debug.Log("OnConnect()");
        ip = ipIF.text;
        port = portIF.text;
    }
}

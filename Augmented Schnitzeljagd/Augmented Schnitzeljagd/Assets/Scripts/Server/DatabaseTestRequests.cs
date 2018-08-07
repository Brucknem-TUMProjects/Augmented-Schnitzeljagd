using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseTestRequests : MonoBehaviour {

    public InputField request, ip, port;
    public Text response;

	// Use this for initialization
	void Start () {
        
	}

    public void OnSend()
    {
        //response.text = GameData.Instance.RequestServer(request.text);
    }

    public void OnConnect()
    {
        GameData.Instance.ConnectServer(ip.text, port.text);

        if (GameData.Instance.SocketConnected)
        {
            response.text = "Connected to server!";
        }
        else
        {
            response.text = "Connection not possible!";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class GameData
{

    private static GameData instance;

    private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    public string standartIP = "192.168.178.49";
    public int port = 1234;
    public IPAddress serverIP = IPAddress.Loopback;
    public int threadsRunning = 0;

    public string[] texts = new string[11];

    public User Me { get; set; }
    public StringList NearbyUsers { get; set; }
    public StringList NearbyRoutes { get; set; }
    public Route CreatingRoute { get; set; }

    public bool isConnected = false;

    private GameData()
    {
        if (instance != null)
            return;
        instance = this;
    }

    public static GameData Instance
    {
        get
        {
            if (instance == null)
                instance = new GameData();
            return instance;
        }
    }

    public void ConnectServer(string ip = "", string port = "")
    {
        Debug.Log("Creating new Thread with ConnectToServerThread()");

        ParameterizedThreadStart start = delegate { ConnectServerThread(ip, port); };
        Thread connectThread = new Thread(start);
        connectThread.Start();
    }


    private void ConnectServerThread(string ip = "", string port = "")
    {
        if (threadsRunning > 10)
            return;

        threadsRunning++;
        Debug.Log("Now in ConnectToServerThread()");
        
        try
        {
            texts[0] = "Reached";
            if (ip.Equals(""))
                serverIP = IPAddress.Parse(standartIP);
            else
                serverIP = IPAddress.Parse(ip);

            texts[1] = "Reached";

            if (port == "")
                port = this.port.ToString();
            texts[2] = "Reached";

            this.port = int.Parse(port);
            texts[3] = "Reached";

            Debug.Log("Connecting on " + serverIP + ":" + port);
            _clientSocket.Connect(serverIP, this.port);
            texts[4] = "Reached";

            Debug.Log("Connecting successfull!");
            isConnected = true;
            texts[5] = "Reached";

        }
        catch (Exception e)
        {
            texts[6] = "Reached";

            _clientSocket.Close();
            texts[7] = "Reached";

            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            texts[8] = "Reached";

            Debug.Log("Connection failed: " + e.Message);
            isConnected = false;
            texts[9] = "Reached";

        }
        texts[10] = "Reached";
        threadsRunning--;
    }

    /// <summary>
    /// <para>Possible server commands are: CreateUser, Login, UpdateUser. </para>
    /// <para>Possible request keywords are: Username, UserID, Password, Position, FriendsIDs.</para>
    /// <para>A server request is in format: &lt;Command&gt;:&lt;Keyword&gt;=value;&lt;Keyword&gt;=value;...</para>
    /// <para>Position and FriendsIDs values must be , seperated. Requests are NOT case-sensitive.</para>
    /// </summary>
    /// <param name="message">The message sent to the server.</param>
    /// <returns>A <see cref="System.String"/> containing the server response.</returns>
    private string RequestServer(System.Object message)
    {
        try
        {
            if (!SocketConnected)
                throw new Exception("Not connected to server!");

            byte[] buffer = Encoding.ASCII.GetBytes(message.ToString());
            _clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[1024];

            int rec = _clientSocket.Receive(receivedBuffer);
            byte[] data = new byte[rec];

            Array.Copy(receivedBuffer, data, rec);

            string response = Encoding.ASCII.GetString(data);
            Debug.Log("Received: " + response);

            isConnected = true;
            return response;
        }
        catch (Exception)
        {
            isConnected = false;
            Debug.Log("The server closed it's connection.");
            return "Error in connection!";
        }
    }

    public bool SocketConnected
    {
        get
        {
            return _clientSocket.Connected;
        }
    }

    public void PingServer()
    {
        isConnected = RequestServer(Strings.PING).Equals(Strings.PING_SUCCESSFUL);
        Debug.Log("Ping: " + isConnected);
        if (isConnected == false)
            throw new SocketException();
    }

    public void UpdateMeOnServer()
    {
        PingServer();
        Debug.Log("Updating me on server: " + Me);
        GameData.Instance.Me = (User)GameData.Instance.RequestServer(Strings.UPDATE_USER + ":" + GameData.Instance.Me);
    }

    public Route RequestRoute(string routeName)
    {
        PingServer();

        string response = GameData.Instance.RequestServer(Strings.REQUEST_ROUTE + ":" + Strings.ROUTENAME + "=" + routeName);
        if (response.Equals(Strings.ROUTE_NOT_FOUND))
            throw new ArgumentException(response);
        return (Route)response;
    }

    public StringList RequestRoutesByStart(Position start, float distance)
    {
        PingServer();
        string response = GameData.Instance.RequestServer(Strings.REQUEST_ROUTE + ":" + Strings.START_POSITION + "=" + start + ";" + Strings.DISTANCE + "=" + distance);
        if (response.Equals(Strings.ROUTE_NOT_FOUND))
            throw new ArgumentException(response);
        return (StringList)response;
    }

    public User RequestUser(string userid = "", string username = "", string password = "")
    {
        PingServer();
        string response = String.Empty;

        if (password != "" && username != "")
            response = GameData.Instance.RequestServer(Strings.REQUEST_USER + ":" + Strings.USERNAME + "=" + username + ";" + Strings.PASSWORD + "=" + password);
        else
            response = GameData.Instance.RequestServer(Strings.REQUEST_USER + ":" + Strings.USER_ID + "=" + userid);

        if (response.Equals(Strings.USER_NOT_FOUND))
            throw new ArgumentException(response);

        return (User)response;

    }

    public StringList RequestNearbyUsers(Position start, float distance)
    {
        PingServer();
        string response = GameData.Instance.RequestServer(Strings.REQUEST_USER + ":" + Strings.START_POSITION + "=" + start + ";" + Strings.DISTANCE + "=" + distance);

        if (response.Equals(Strings.USER_NOT_FOUND))
            throw new ArgumentException(response);

        return (StringList)response;
    }

    public UserStats RequestUserStats(string userid)
    {
        PingServer();
        string response = GameData.Instance.RequestServer(Strings.REQUEST_USER_STATS + ":" + Strings.USER_ID + "=" + userid);

        if (response.Equals(Strings.USER_NOT_FOUND))
            throw new ArgumentException(response);

        return (UserStats)response;
    }

    public void UpdateUserStats(UserStats us)
    {
        PingServer();
        GameData.Instance.RequestServer(Strings.UPDATE_USER_STATS + ":" + us);
    }

    public Route CreateRoute(Route route)
    {
        PingServer();
        return (Route)GameData.Instance.RequestServer(Strings.CREATE_ROUTE + ":" + route);
    }

    public User CreateUser(string userid = "", string username = "", string password = "", User user = null)
    {
        PingServer();
        string response = String.Empty;

        if (password != "" && username != "")
            response = GameData.Instance.RequestServer(Strings.CREATE_USER + ":" + Strings.USERNAME + "=" + username + ";" + Strings.PASSWORD + "=" + password);
        else if (userid != "")
            response = GameData.Instance.RequestServer(Strings.CREATE_USER + ":" + Strings.USER_ID + "=" + userid);
        else
            response = RequestServer(Strings.CREATE_USER + ":" + user);

        if (response.Equals(Strings.USER_EXISTS_ALREADY))
            throw new ArgumentException(response);

        return (User)response;
    }

    public DateTime GetServerTime()
    {
        PingServer();
        string response = RequestServer(Strings.TIME);
        return DateTime.ParseExact(response, "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
    }
}

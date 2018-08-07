using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System.Reflection;
using System;

public class User
{
    public string Username { get; set; }
    public string UserID { get; set; }
    public string Password { get; set; }
    public Position Position { get; set; }
    public IsLoggedIn IsLoggedIn { get; set; }
    public string CurrentRoute { get; set; }
    public int RouteProgress { get; set; }
    public StringList FriendsIDs { get; set; }
    public string CurrentSkin { get; set; }
    public long RouteTime { get; set; }

    private Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();

    private bool hasNameReceived = false;
    private bool hasFriendsReceived = false;

    // Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseUser"/> class.
    /// </summary>
    public User()
    {
        PropertyInfo[] variableNames = typeof(User).GetProperties();
        foreach (PropertyInfo p in variableNames)
        {
            properties.Add(p.Name.ToLower(), p);
        }

        Position = new Position();
        FriendsIDs = new StringList();
        IsLoggedIn = new IsLoggedIn();
        CurrentRoute = "";
        CurrentSkin = "Default";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseUser" /> class.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="position">The position.</param>
    /// <param name="friendsIds">The friends ids.</param>
    public User(string username, string password, string userId = null, Position position = null, IsLoggedIn isLoggedIn = null, string currentRoute=null, int progress=0, StringList friendsIds = null, string currentSkin="Default", long routeTime = 0) : this()
    {
        this.Username = username;
        this.Password = password;
        this.CurrentSkin = currentSkin;
        this.RouteTime = routeTime;

        if (userId == null)
        {
            this.UserID = "";
            //string randomUserId = "";
            //do
            //{
            //    randomUserId = Strings.RandomString(16);
            //} while (!GameData.Instance.RequestServer(Strings.REQUEST_USER + ":" + Strings.USER_ID + "=" + randomUserId).Equals(Strings.USER_NOT_FOUND));
            //this.UserID = randomUserId;
        }
        else
        {
            this.UserID = userId;
        }
        if (friendsIds != null)
            this.FriendsIDs = friendsIds;
        if (position != null)
            this.Position = position;
        if (isLoggedIn != null)
            this.IsLoggedIn = isLoggedIn;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="userStringRepresentation">The user string representation.</param>
    public User(string representation) : this()
    {
        Dictionary<string, object> args = Strings.ParseRequest(representation);
        foreach (string key in args.Keys)
        {
            properties[key.ToLower()].SetValue(this, args[key.ToLower()], null);
        }
    }

    public User(Dictionary<string, object> args) : this()
    {
        if (args.ContainsKey(Strings.USER_ID))
        {
            this.UserID = args[Strings.USER_ID].ToString();
        }
        else
        {
            //string randomUserId = "";
            //do
            //{
            //    randomUserId = Strings.RandomString(16);
            //} while (!GameData.Instance.RequestServer(Strings.REQUEST_USER + ":" + Strings.USER_ID + "=" + randomUserId).Equals(Strings.USER_NOT_FOUND));
            //this.UserID = randomUserId;
        }
        foreach (string key in args.Keys)
        {
            properties[key.ToLower()].SetValue(this, Convert.ChangeType(args[key.ToLower()], properties[key.ToLower()].PropertyType), null);
        }
    }

    public static explicit operator User(string representation)
    {
        return new User(representation);
    }

    public static explicit operator User(Dictionary<string, object> representation)
    {
        return new User(representation);
    }

    public Dictionary<string, PropertyInfo> GetProperties()
    {
        return properties;
    }

    public IEnumerator GenerateFacebookUser()
    {
        Debug.Log("Generate FacebookUser.");
        this.Password = Strings.RandomString(20);
        this.Position = new Position();
        this.UserID = AccessToken.CurrentAccessToken.UserId;
        FB.API("me?fields=name", HttpMethod.GET, NameCallback);
        yield return new WaitUntil(() => hasNameReceived == true);
        FB.API("me/friends", HttpMethod.GET, FriendCallback);
        yield return new WaitUntil(() => hasFriendsReceived == true);
    }

    void NameCallback(IGraphResult result)
    {
        IDictionary<string, object> profile = result.ResultDictionary;
        Username = profile["name"].ToString();
        hasNameReceived = true;
    }

    void FriendCallback(IGraphResult result)
    {
        FriendsIDs = new StringList();
        IDictionary<string, object> data = result.ResultDictionary;
        List<object> friends = (List<object>)data["data"];
        foreach (object obj in friends)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)obj;
            FriendsIDs.Add(dict["id"].ToString());
        }
        hasFriendsReceived = true;
    }


    public override string ToString()
    {
        string s = "";

        foreach (string p in properties.Keys)
        {
            s += p + "=" + properties[p].GetValue(this, null) + ";";
        }

        return s;
    }

    // override object.Equals
    /// <summary>
    /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        return this.UserID.Equals(((User)obj).UserID);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}

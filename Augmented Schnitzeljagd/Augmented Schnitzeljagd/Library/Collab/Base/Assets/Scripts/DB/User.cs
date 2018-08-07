using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System.Linq;
using System.Globalization;

public class User : MonoBehaviour
{

    //Possible request parameters
    private const string NAME = "name";
    private const string USER_ID = "userid";
    private const string PASSWORD = "password";
    private const string POSITION = "position";
    private const string FRIEND_IDS = "friendsids";

    public string Username { get; set; }
    public string UserID { get; set; }
    public string Password { get; set; }
    public Vector2 Position { get; set; }
    public List<string> FriendsIDs { get; set; }

    public static System.Random random = new System.Random();

    private bool hasNameReceived = false;
    private bool hasFriendsReceived = false;

    // Constructor
    public User()
    {
        Debug.Log("Creating new user");
    }

    public User(string username, string password)
    {
        this.Username = username;
        this.Password = password;
        string randomUserId = "";
        do
        {
            randomUserId = RandomString(16);
        } while (MyDataBase.Instance.ContainsUserById(randomUserId));
        this.UserID = randomUserId;
        this.FriendsIDs = new List<string>();
        this.Position = new Vector2(0, 0);
    }

    public User(string username, string userId, string password, List<string> friendsIds, Vector2 position)
    {
        this.Username = username;
        this.UserID = userId;
        this.Password = password;
        this.FriendsIDs = friendsIds;
        this.Position = position;
    }

    public User(string userStringRepresentation)
    {
        Dictionary<string, object> parsed = ParseRequest(userStringRepresentation);
        this.Username = parsed[NAME].ToString();
        this.UserID = parsed[USER_ID].ToString();
        this.Password = parsed[PASSWORD].ToString();
        this.Position = (Vector2)parsed[POSITION];
        this.FriendsIDs = (List<string>)parsed[FRIEND_IDS];
    }

    public IEnumerator GenerateFacebookUser()
    {
        Debug.Log("Generate FacebookUser.");
        this.Password = RandomString(20);
        this.Position = new Vector2(0, 0);
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
        FriendsIDs = new List<string>();
        IDictionary<string, object> data = result.ResultDictionary;
        List<object> friends = (List<object>)data["data"];
        foreach (object obj in friends)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)obj;
            FriendsIDs.Add(dict["id"].ToString());
        }
        hasFriendsReceived = true;
    }

    public static string RandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string s = "";
        for (int i = 0; i < length; i++)
        {
            int x = random.Next(chars.Length);
            s += chars[x];
        }
        return s;
    }


    public override string ToString()
    {
        string s = "Name=" + Username +
            ";UserID=" + UserID +
            ";Password=" + Password +
            ";Position=" + Position.x.ToString("F10", CultureInfo.InvariantCulture) + "," + Position.y.ToString("F10", CultureInfo.InvariantCulture) +
            ";FriendsIDs=";

        if (FriendsIDs.Count > 0)
        {
            foreach (string friend in FriendsIDs)
            {
                s += friend + ",";
            }
            s = s.Substring(0, s.Length - 1);
        }

        return s;
    }

    // override object.Equals
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

    private static Dictionary<string, object> ParseRequest(string parameters)
    {
        Dictionary<string, object> parsedArguments = new Dictionary<string, object>();

        string[] parameterParts = parameters.Split(';');

        foreach (string attribute in parameterParts)
        {
            string keyword = attribute.Split('=')[0].ToLower();
            string[] value = attribute.Split('=')[1].Split(',');

            switch (keyword)
            {
                case POSITION:
                    Vector2 position = new Vector2(float.Parse(value[0]), float.Parse(value[1]));
                    parsedArguments.Add(keyword, position);
                    break;
                case FRIEND_IDS:
                    List<string> friendsIds = new List<string>();
                    foreach (string friend in value)
                    {
                        friendsIds.Add(friend);
                    }
                    parsedArguments.Add(keyword, friendsIds);
                    break;
                default:
                    parsedArguments.Add(keyword, value[0]);
                    break;
            }
        }

        return parsedArguments;
    }
}

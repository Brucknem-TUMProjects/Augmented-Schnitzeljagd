using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class User  {

    public string Username {get; set; }
    public string UserID { get; set; }
    public string Password { get; set; }
    public Position Position { get; set; }
    public List<string> FriendsIDs { get; set; }
    
    public System.Random random = new System.Random();

    // Constructor
    public User()
    {
        Position = new Position();
        FriendsIDs = new List<string>();
    }

    public User(string username, string password, Position position=null, List<string> friendsIds=null)
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
        this.Position = new Position();

        if (friendsIds != null)
            this.FriendsIDs = friendsIds;
        if (position != null)
            this.Position = position;
    }

    public User(string username, string userId, string password, Position position, List<string> friendsIds)
    {
        this.Username = username;
        this.UserID = userId;
        this.Password = password;
        this.FriendsIDs = friendsIds;
        this.Position = position;
    }
    
    private string RandomString(int length)
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
            ";Position=" + Position.Longitude + "," + Position.Latitude +
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
}

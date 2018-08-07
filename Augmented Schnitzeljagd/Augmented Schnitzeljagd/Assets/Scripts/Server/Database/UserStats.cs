using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UserStats
{
    public string UserID { get; set; }
    public long Experience { get; set; }
    public long Coins { get; set; }
    public long FinishedRoutes { get; set; }
    public long CreatedRoutes { get; set; }
    public long PlayTime { get; set; }
    public StringList Skins { get; set; }

    private Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();

    private UserStats()
    {
        PropertyInfo[] variableNames = typeof(UserStats).GetProperties();
        foreach (PropertyInfo p in variableNames)
        {
            properties.Add(p.Name.ToLower(), p);
        }
    }

    public UserStats(string userID, long experience = 0, long coins = 0, long finishedRoutes = 0, long createdRoutes = 0, long playTime = 0, StringList skins=null) : this()
    {
        this.UserID = userID;
        this.Experience = experience;
        this.Coins = coins;
        this.FinishedRoutes = finishedRoutes;
        this.CreatedRoutes = createdRoutes;
        this.PlayTime = playTime;
        if (skins != null)
            this.Skins = skins;
        else
            this.Skins = new StringList() { "Default", };
    }

    public UserStats(string representation) : this()
    {
        Debug.Log(representation);
        Dictionary<string, object> args = Strings.ParseRequest(representation);
        this.UserID = args[Strings.USER_ID].ToString();

        foreach (string key in args.Keys)
        {
            if (key.Equals(Strings.SKINS))
                properties[key.ToLower()].SetValue(this, (StringList)(args[key.ToLower()].ToString()), null);
            else if (!key.Equals(Strings.USER_ID))
                properties[key.ToLower()].SetValue(this, long.Parse(args[key.ToLower()].ToString()), null);
            
        }
    }

    public UserStats(Dictionary<string, object> args) : this()
    {
        foreach (string key in args.Keys)
        {
            properties[key.ToLower()].SetValue(this, args[key.ToLower()], null);
        }
    }

    public static explicit operator UserStats(string representation)
    {
        return new UserStats(representation);
    }

    public static explicit operator UserStats(Dictionary<string, object> representation)
    {
        return new UserStats(representation);
    }

    public Dictionary<string, PropertyInfo> GetProperties()
    {
        return properties;
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
}
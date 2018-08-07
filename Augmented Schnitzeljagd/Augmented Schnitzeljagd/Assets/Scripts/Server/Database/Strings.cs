using System;
using System.Collections.Generic;
using System.Globalization;

public static class Strings
{
    //Succes/fail messages
    public const string USER_CREATION_FAILED = "Creation failed.";
    public const string USER_UPDATE_FAILED = "Update failed.";
    public const string USER_NOT_FOUND = "User not found.";
    public const string INVALID_REQUEST = "Invalid request.";
    public const string ROUTE_NOT_FOUND = "Route not found";
    public const string USER_EXISTS_ALREADY = "User already in database";

    public const string CONNECTION_FAILED = "Connection failed!";
    public const string CONNECTION_SUCCESS = "Connection successful!";

    public const string PING_SUCCESSFUL = "Ping successful.";

    //Possible request commands
    public const string CREATE_USER = "createuser";
    public const string REQUEST_USER = "requestuser";
    public const string UPDATE_USER = "updateuser";

    public const string CREATE_ROUTE = "createroute";
    public const string REQUEST_ROUTE = "requestroute";
    public const string UPDATE_ROUTE = "updateroute";

    public const string REQUEST_USER_STATS = "requestuserstats";
    public const string UPDATE_USER_STATS = "updateuserstats";

    public const string PING = "PING";
    public const string TIME = "servertime";

    //Possible request parameters
    public const string USERNAME = "username";
    public const string USER_ID = "userid";
    public const string PASSWORD = "password";
    public const string POSITION = "position";
    public const string IS_LOGGED_IN = "isloggedin";
    public const string FRIEND_IDS = "friendsids";
    public const string CURRENT_ROUTE = "currentroute";
    public const string ROUTE_PROGRESS = "routeprogress";
    public const string CURRENT_SKIN = "currentskin";
    public const string ROUTE_TIME = "routetime";

    public const string EXPERIENCE = "experience";
    public const string COINS = "coins";
    public const string FINISHED_ROUTES = "finishedroutes";
    public const string CREATED_ROUTES = "createdroutes";
    public const string PLAYTIME = "playtime";
    public const string SKINS = "skins";

    public const string ROUTENAME = "routename";
    public const string COORDINATES = "coordinates";
    public const string START_POSITION = "start";
    public const string DISTANCE = "distance";

    public static readonly List<string> SKIN_LIST = new List<string>() { "Agent", "Boyscout", "Censored", "Default", "Gandalf", "Horseface", "Kanye", "Kelso", "Mozart", "Pilgram", "Pirate", "Suit", "Tourist", "Tsubasa", "Zombie", };

    public static System.Random random = new System.Random();

    public static string RandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string s = "";
        for (int i = 0; i < length; i++)
        {
            int x = random.Next(chars.Length);
            s += chars[x];
        }
        return s;
    }

    /// <summary>
    /// Parses the request.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>A <see cref="System.Collections.Generic.Dictionary{string, object}" /> of the parsed request. Possible Keys are <see cref="USERNAME" />, <see cref="USER_ID" />,<see cref="PASSWORD" />,<see cref="POSITION" />,<see cref="FRIEND_IDS" />...</returns>
    public static Dictionary<string, object> ParseRequest(string parameters)
    {
        Dictionary<string, object> parsedArguments = new Dictionary<string, object>();

        string[] parameterParts = parameters.Split(';');

        foreach (string attribute in parameterParts)
        {
            string keyword = "";
            string value = "";
            try
            {
                keyword = attribute.Split('=')[0].ToLower();
                value = attribute.Split('=')[1].Trim();
            }
            catch (Exception)
            { //Console.WriteLine(keyword + " not parsable."); 
                continue;
            }

            switch (keyword)
            {
                case Strings.POSITION:
                    parsedArguments.Add(keyword, (Position)value);
                    break;
                case Strings.START_POSITION:
                    parsedArguments.Add(keyword, (Position)value);
                    break;
                case Strings.FRIEND_IDS:
                    parsedArguments.Add(keyword, (StringList)value);
                    break;
                case Strings.IS_LOGGED_IN:
                    parsedArguments.Add(keyword, (IsLoggedIn)value);
                    break;
                case Strings.COORDINATES:
                    parsedArguments.Add(keyword, (CoordinateList)value);
                    break;
                case Strings.ROUTE_PROGRESS:
                    parsedArguments.Add(keyword, int.Parse(value));
                    break;
                case Strings.ROUTE_TIME:
                    parsedArguments.Add(keyword, long.Parse(value));
                    break;
                case Strings.SKINS:
                    parsedArguments.Add(keyword, (StringList)value);
                    break;
                default:
                    parsedArguments.Add(keyword, value);
                    break;
            }
        }

        return parsedArguments;
    }

    public static void printDict(Dictionary<string, object> dict)
    {
        foreach (string key in dict.Keys)
            Console.WriteLine("Key: " + key + " --- " + "Value: " + dict[key]);
    }


}
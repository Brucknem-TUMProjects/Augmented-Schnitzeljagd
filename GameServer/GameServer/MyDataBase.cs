using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.IO;
using System;
using System.Drawing;

public class MyDataBase {

    private const string USER_NOT_FOUND = "User not found";
    private const string ROUTE_NOT_FOUND = "Route not found";

    // Lists stored in Data Base
    private List<Route> routes;
    private List<User> users;

    // Paths to XML files
    private static string routePath = Path.Combine("", "Routes.xml");
    private static string usersPath = Path.Combine("", "Users.xml");

    private static MyDataBase instance;

    private MyDataBase()
    {
        if (instance != null)
            return;
        routes = new List<Route>();
        users = new List<User>();
        instance = this;
    }

    public static MyDataBase Instance
    {
        get
        {
            if (instance == null)
                instance = new MyDataBase();
            return instance;
        }
    }


    // ----------------------------------------------------------------------------------------------------------------------------------------------
    //                                                              METHODS
    // ----------------------------------------------------------------------------------------------------------------------------------------------
    // -----------------------------------------------------------------------------------------------------------------
    //                                                 Add Routes & Users
    // ----------------------------------------------------------------------------------------------------------------- 

    public void AddRoute(string routeName, List<Coordinate> coordinates)
    {
        Route newRoute = new Route(routeName, coordinates);
        routes.Add(newRoute);
        SaveRoutesToXML();
    }

    public void AddFacebookUser(User user)
    {
        if(!users.Contains(user))
            users.Add(user);
        SaveUsersToXML();
    }

    public void AddCustomUser(User user)
    {
        foreach(User u in users)
        {
            if (user.Username.Equals(u.Username))
                throw new ArgumentException("User already in database");
        }

        users.Add(user);
        SaveUsersToXML();
    }



    // -----------------------------------------------------------------------------------------------------------------
    //                                                 Get Routes & Users
    // ----------------------------------------------------------------------------------------------------------------- 
    
    public Route GetRouteByName(string routeName)
    {
        foreach(Route r in routes)
        {
            if (r.getRouteName().Equals(routeName))
                return r;
        }
        throw new ArgumentException(ROUTE_NOT_FOUND);
    }

    public List<Route> GetRoutesByStartingLocation(float longitude, float latitude, float maxDistance)
    {
        List<Route> routesWithNearbyStartingLocation = new List<Route>();

        foreach(Route r in routes)
        {
            float x = r.getCoordinates()[0].Longitude - longitude;
            float y = r.getCoordinates()[0].Latitude - latitude;
            if (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)) < maxDistance)
            {
                routesWithNearbyStartingLocation.Add(r);
            }
        }

        return routesWithNearbyStartingLocation;
    }

    public User GetUserByID(string id)
    {
        foreach(User u in users)
        {
            if (u.UserID.Equals(id))
                return u;
        }
        throw new ArgumentException(USER_NOT_FOUND);
    }

    public List<User> GetNearbyUsers(User me, float maxDistance)
    {
        List<User> nearbyUsers = new List<User>();

        foreach(User u in users)
        {
            float x = u.Position.Longitude - me.Position.Longitude;
            float y = u.Position.Latitude - me.Position.Latitude;
            if (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)) < maxDistance)
            {
                nearbyUsers.Add(u);
            }
        }

        return nearbyUsers;
    }


    // -----------------------------------------------------------------------------------------------------------------
    //                                                 Load Lists
    // ----------------------------------------------------------------------------------------------------------------- 

    /* Load lists with Methods below
     * if anything changes in XML File Structure, adapt methods properly
     * so that everythin is read correctly
     */
    public void LoadRouteList()
    {
        XmlDocument xmlDoc = new XmlDocument();

        // Load xml file
        try
        {
            xmlDoc.Load(routePath);
        }
        catch (Exception)
        {
            string[] initXML = { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<Routes>", "</Routes>" };
            File.WriteAllLines(routePath, initXML);
            Console.WriteLine("Error 404: Path: " + routePath + " could not be found.not found. \n Routelist could not be loaded. DataBase has wrong Path stored!");
            Console.WriteLine("Created new routes database.");
            return;
        }

        XmlNodeList routeList = xmlDoc.GetElementsByTagName("Route");
        foreach(XmlNode routeInfo in routeList)
        {
            XmlNodeList routeAttributes = routeInfo.ChildNodes;
            Route route = new Route();
            foreach(XmlNode routeAttribute in routeAttributes)
            {
                switch (routeAttribute.Name)
                {
                    case "Name":
                        route.setRouteName(routeAttribute.InnerText);
                        break;
                    case "Coordinates":
                        XmlNodeList coordinates = routeAttribute.ChildNodes;
                        List<Coordinate> coords = new List<Coordinate>();
                        foreach(XmlNode coordinateAttribute in coordinates)
                        {
                            Coordinate coordinate = new Coordinate();
                            coordinate.Longitude = float.Parse(coordinateAttribute.Attributes["Longitude"].Value);
                            coordinate.Latitude = float.Parse(coordinateAttribute.Attributes["Latitude"].Value);
                            coordinate.GameType = Int32.Parse(coordinateAttribute.Attributes["GameType"].Value);
                            coords.Add(coordinate);
                        }
                        route.setCoordinates(coords);
                        break;
                }
            }
            routes.Add(route);
        }
    }

    public void LoadUserList()
    {
        XmlDocument xmlDoc = new XmlDocument();

        // Load xml file
        try
        {
            xmlDoc.Load(usersPath);
        }
        catch (Exception)
        {
            string[] initXML = { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<Users>", "</Users>" };
            File.WriteAllLines(usersPath, initXML);
            Console.WriteLine("Error 404: Path: " + usersPath + " could not be found.not found. \n Userlist could not be loaded. DataBase has wrong Path stored!");
            Console.WriteLine("Created new users database.");
            return;
        }

        XmlNodeList userlist = xmlDoc.GetElementsByTagName("User");
        foreach (XmlNode userInfo in userlist)
        {
            User user = new User();
            user.Username = userInfo.Attributes["Username"].Value;
            user.UserID = userInfo.Attributes["UserID"].Value;
            user.Password = userInfo.Attributes["Password"].Value;

            XmlNodeList userAttributes = userInfo.ChildNodes;
            foreach (XmlNode userAttribute in userAttributes)
            {
                switch (userAttribute.Name)
                {
                    case "Position":
                        user.Position.Longitude = float.Parse(userAttribute.Attributes["Longitude"].Value);
                        user.Position.Latitude = float.Parse(userAttribute.Attributes["Latitude"].Value);
                        break;
                    case "FriendsIDs":
                        XmlNodeList friendsIds = userAttribute.ChildNodes;
                        List<string> friends = new List<string>();
                        foreach (XmlNode friendsIdsAttribute in friendsIds)
                        {
                            friends.Add(friendsIdsAttribute.InnerText);
                        }
                        user.FriendsIDs = friends;
                        break;
                }
            }
            users.Add(user);
        }
        foreach(User u in users)
        {
            Console.WriteLine(u);
        }
    }



    // -----------------------------------------------------------------------------------------------------------------
    //                                                 Save Lists
    // ----------------------------------------------------------------------------------------------------------------- 

    public void SaveRoutesToXML()
    {
        XmlDocument xmlDoc = new XmlDocument();
        string[] initXML = { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<Routes>", "</Routes>" };
        File.WriteAllLines(routePath, initXML);

        xmlDoc.Load(routePath);
        XmlElement elemRoot = xmlDoc.DocumentElement;

        foreach(Route r in routes)
        {
            XmlElement elemNew = xmlDoc.CreateElement("Route");
            XmlElement name = xmlDoc.CreateElement("Name");
            name.InnerText = r.getRouteName();

            XmlElement coordinates = xmlDoc.CreateElement("Coordinates");
            foreach(Coordinate c in r.getCoordinates())
            {
                XmlElement coordinate = xmlDoc.CreateElement("Coordinate");
                coordinate.SetAttribute("Longitude", c.Longitude.ToString());
                coordinate.SetAttribute("Latitude", c.Latitude.ToString());
                coordinate.SetAttribute("GameType", c.GameType.ToString());
                coordinates.AppendChild(coordinate);
            }
            elemNew.AppendChild(name);
            elemNew.AppendChild(coordinates);
            elemRoot.AppendChild(elemNew);
        }
        xmlDoc.Save(routePath);
    }

    public void SaveUsersToXML()
    {
        XmlDocument xmlDoc = new XmlDocument();
        string[] initXML = { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<Users>", "</Users>" };
        File.WriteAllLines(usersPath, initXML);

        xmlDoc.Load(usersPath);
        XmlElement elemRoot = xmlDoc.DocumentElement;

        foreach(User u in users)
        {
            XmlElement elemNew = xmlDoc.CreateElement("User");
            XmlElement position = xmlDoc.CreateElement("Position");
            XmlElement friendsIds = xmlDoc.CreateElement("FriendsIDs");

            elemNew.SetAttribute("Username", u.Username);
            elemNew.SetAttribute("UserID", u.UserID);
            elemNew.SetAttribute("Password", u.Password);
            position.SetAttribute("Longitude", u.Position.Longitude.ToString());
            position.SetAttribute("Latitude", u.Position.Latitude.ToString());

            foreach (string friend in u.FriendsIDs)
            {
                XmlElement f = xmlDoc.CreateElement("FriendID");
                f.InnerText = friend;
                friendsIds.AppendChild(f);
            }
            
            elemNew.AppendChild(position);
            elemNew.AppendChild(friendsIds);

            elemRoot.AppendChild(elemNew);
        }
        xmlDoc.Save(usersPath);
    }


    // -----------------------------------------------------------------------------------------------------------------
    //                                                 User Logging
    // ----------------------------------------------------------------------------------------------------------------- 

    public User CheckForUserWithPassword(string username, string password)
    {
        foreach(User u in users)
        {
            Console.WriteLine(username + " - " + u.Username);
            Console.WriteLine(password + " - " + u.Password);
            if (u.Username.Equals(username) && u.Password.Equals(password))
                return u;
        }

        throw new ArgumentException(USER_NOT_FOUND);
    }

    public bool ContainsUserByName(string username)
    {
        foreach(User u in users)
        {
            if (u.Username.Equals(username))
                return true;
        }
        return false;
    }

    public bool ContainsUserById(string id)
    {
        foreach (User u in users)
        {
            if (u.UserID.Equals(id))
                return true;
        }
        return false;
    }
}
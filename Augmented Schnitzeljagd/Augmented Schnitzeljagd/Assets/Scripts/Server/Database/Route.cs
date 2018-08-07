using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Route  
{
    public string RouteName { get; set; }
    public CoordinateList Coordinates { get; set; }

    private Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Route"/> class.
    /// </summary>
    public Route()
    {
        PropertyInfo[] variableNames = typeof(Route).GetProperties();
        foreach (PropertyInfo p in variableNames)
        {
            Console.WriteLine(p.Name.ToLower());
            properties.Add(p.Name.ToLower(), p);
        }
        RouteName = "";
        Coordinates = new CoordinateList();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Route"/> class.
    /// </summary>
    /// <param name="routeName">Name of the route.</param>
    /// <param name="coordinates">The coordinates.</param>
    public Route(string routeName, CoordinateList coordinates) : this()
    {
        this.RouteName = routeName;
        this.Coordinates = coordinates;
    }

    public Route(string representation) : this()
    {
        Dictionary<string, object> args = Strings.ParseRequest(representation);
        foreach (string key in args.Keys)
        {
            properties[key.ToLower()].SetValue(this, Convert.ChangeType(args[key.ToLower()], properties[key.ToLower()].PropertyType), null);
        }
    }

    public Route(Dictionary<string,object> args) : this()
    {
        foreach (string key in args.Keys)
        {
            Console.WriteLine(key + " - " + args[key]);
            properties[key.ToLower()].SetValue(this, args[key.ToLower()], null);
        }
    }

    public Dictionary<string, PropertyInfo> GetProperties()
    {
        return properties;
    }

    public static explicit operator Route(string representation)
    {
        return new Route(representation);
    }

    public static explicit operator Route(Dictionary<string,object> representation)
    {
        return new Route(representation);
    }

    public override string ToString()
    {
        return Strings.ROUTENAME + "=" + RouteName + ";" + Strings.COORDINATES + "=" + Coordinates;
    }
}

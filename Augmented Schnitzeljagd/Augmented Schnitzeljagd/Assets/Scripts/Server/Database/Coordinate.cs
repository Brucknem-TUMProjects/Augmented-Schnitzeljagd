using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Coordinate
{
    public float Longitude { get; set; }
    public float Latitude { get; set; }
    public int GameType { get; set; }
    public int Difficulty { get; set; }
    public string Hint { get; set; }
    public float C1 { get; set; }
    public float C2 { get; set; }
    public float C3 { get; set; }

    public Coordinate()
    {
        Longitude = 0;
        Latitude = 0;
        GameType = 0;
        Difficulty = 0;
        C1 = C2 = C3 = 0;
        Hint = "";
    }

    public Coordinate(string representation) : this()
    {
        string[] values = representation.Split('|');
        Latitude = float.Parse(values[0], CultureInfo.InvariantCulture);
        Longitude = float.Parse(values[1], CultureInfo.InvariantCulture);
        GameType = int.Parse(values[2]);
        Difficulty = int.Parse(values[3]);
        Hint = values[4];
        C1 = float.Parse(values[5], CultureInfo.InvariantCulture);
        C2 = float.Parse(values[6], CultureInfo.InvariantCulture);
        C3 = float.Parse(values[7], CultureInfo.InvariantCulture);
        Debug.Log("Coordinate: " + C1 + "," + C2 + "," + C3);
    }

    public static explicit operator Coordinate(string representation)
    {
        return new Coordinate(representation);
    }

    public override string ToString()
    {
        string s = Latitude.ToString("F10", CultureInfo.InvariantCulture) + '|' + Longitude.ToString("F10", CultureInfo.InvariantCulture) + '|' + GameType + '|' + Difficulty + '|' + Hint + '|' + C1.ToString("0.00000", CultureInfo.InvariantCulture) + '|' + C2.ToString("0.00000", CultureInfo.InvariantCulture) + '|' + C3.ToString("0.00000", CultureInfo.InvariantCulture);

        return s;
    }
}

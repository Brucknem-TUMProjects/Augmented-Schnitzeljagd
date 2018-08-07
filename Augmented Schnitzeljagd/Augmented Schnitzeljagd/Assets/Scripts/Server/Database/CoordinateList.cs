using System.Collections;
using System.Collections.Generic;

public class CoordinateList : List<Coordinate>
{
    public CoordinateList()
    {

    }

    public CoordinateList(string representation) : this()
    {
        string[] values = representation.Split(',');
        foreach (string value in values)
        {
            this.Add(new Coordinate(value));
        }
    }

    public static explicit operator CoordinateList(string representation)
    {
        return new CoordinateList(representation);
    }

    public override string ToString()
    {
        string s = "";

        if (this.Count > 0)
        {
            foreach (Coordinate coordinate in this)
            {
                s += coordinate + ",";
            }
            s = s.Substring(0, s.Length - 1);
        }

        return s;
    }
}

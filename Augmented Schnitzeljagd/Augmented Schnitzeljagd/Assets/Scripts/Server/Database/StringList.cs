using System.Collections;
using System.Collections.Generic;

public class StringList : List<string>
{
    public StringList()
    {
        
    }

    public StringList(string representation) : this()
    {
        string[] values = representation.Split(',');
        foreach(string value in values)
        {
            this.Add(value);
        }
    }

    public static explicit operator StringList(string representation)
    {
        return new StringList(representation);
    }

    public static StringList Empty
    {
        get
        {
            return new StringList();
        }
    }

    public override string ToString()
    {
        string s = "";

        if (this.Count > 0)
        {
            foreach (string friend in this)
            {
                s += friend + ",";
            }
            s = s.Substring(0, s.Length - 1);
        }

        return s;
    }
}

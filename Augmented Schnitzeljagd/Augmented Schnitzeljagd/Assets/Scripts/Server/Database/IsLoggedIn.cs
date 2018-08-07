
using System;

public class IsLoggedIn
{
    public bool Current { get; set; }

    public IsLoggedIn()
    {
        this.Current = false;
    }

    public IsLoggedIn(bool current)
    {
        this.Current = current;
    }

    public IsLoggedIn(string representation)
    {
        Current = representation.ToLower().Equals("true");
    }

    public static implicit operator bool(IsLoggedIn isLoggedIn)
    {
        return isLoggedIn != null && isLoggedIn.Current;
    }

    public static implicit operator IsLoggedIn(bool isLoggedIn)
    {
        return new IsLoggedIn(isLoggedIn);
    }

    public static explicit operator IsLoggedIn(string representation)
    {
        return new IsLoggedIn(representation);
    }

    public override string ToString()
    {
        return Current.ToString();
    }
}
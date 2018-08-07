
using System.Globalization;

public class Position
{
    public float Longitude { get; set; }
    public float Latitude { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Position"/> class.
    /// </summary>
    public Position() : this(0, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Position"/> class.
    /// </summary>
    /// <param name="longitude">The longitude.</param>
    /// <param name="latitude">The latitude.</param>
    public Position(float latitude, float longitude)
    {
        Longitude = longitude;
        Latitude = latitude;
    }

    public static implicit operator Position(string representation)
    {
        return new Position(representation);
    }

    public Position(string representation)
    {
        string[] values = representation.Split(',');
        Longitude = float.Parse(values[1], CultureInfo.InvariantCulture);
        Latitude = float.Parse(values[0], CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return Latitude.ToString("F10", CultureInfo.InvariantCulture) + "," + Longitude.ToString("F10", CultureInfo.InvariantCulture);
    }
}

public class Position
{
    public float Longitude { get; set; }
    public float Latitude { get; set; }

    public Position()
    {
        Longitude = Latitude = 0;
    }

    public Position(float longitude, float latitude)
    {
        Longitude = longitude;
        Latitude = latitude;
    }

    public override string ToString()
    {
        return Longitude + "," + Latitude;
    }
}
using System.Collections;
using System.Collections.Generic;

public class Route  
{
	private string routeName;
    private List<Coordinate> coordinates = new List<Coordinate>();


    public Route()
    {

    }

    public Route(string routeName, List<Coordinate> coordinates)
    {
        this.routeName = routeName;
        this.coordinates = coordinates;
    }

    // Get Methods

    public string getRouteName()
    {
        return routeName;
    }

    public List<Coordinate> getCoordinates()
    {
        return coordinates;
    }


    // Set Methods

    public void setRouteName(string routeName)
    {
        this.routeName = routeName;
    }

    public void setCoordinates(List<Coordinate> coordinates)
    {
        this.coordinates = coordinates;
    }
}

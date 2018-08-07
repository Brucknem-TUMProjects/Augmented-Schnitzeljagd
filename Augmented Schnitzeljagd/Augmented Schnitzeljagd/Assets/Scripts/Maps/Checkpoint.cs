using UnityEngine;
using System.Collections;

//Checkpoint
//
//Bugged out Need to fix TODO
public class Checkpoint : MonoBehaviour
{
    //Geo Position Variables
    GoogleStaticMap mainMap;
    public float lat_d = 48.26258f, lon_d = 11.66788f;
    private GeoPoint pos;

    //Rotationvariables
    public float rotationSpeed = 100.0f;
    public Vector3 rotationVector = Vector3.up;

    void Update()
    {
        //Rotation
        transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime, Space.World);
    }

    void Awake()
    {
        //Set the Checkpoints coords
        pos = new GeoPoint();
        pos.setLatLon_deg(lat_d, lon_d);
    }

    //
    //Changed this from
    //transform.position = new Vector3(tempPosition.x, transform.position.y, tempPosition.y);
    public void setPositionOnMap()
    {
        Vector2 tempPosition = GameManager.Instance.getMainMapMap().getPositionOnMap(this.pos);
        transform.position = new Vector2(tempPosition.x, transform.position.y);
    }

    public void setPositionOnMap(GeoPoint pos)
    {
        this.pos = pos;
        setPositionOnMap();
    }

}

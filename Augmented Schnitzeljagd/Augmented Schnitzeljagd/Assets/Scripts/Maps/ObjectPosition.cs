using UnityEngine;
using System.Collections;

//ObjectPosition
//
//
public class ObjectPosition : MonoBehaviour {
	GoogleStaticMap mainMap;

    //lat, lon
    public Coordinate start;
    public string routeName;

    public SpriteHolder spriteHolder;

	private GeoPoint pos;

	void Start (){
	}

    public void SetGeoPoint()
    {
        pos = new GeoPoint();
        pos.setLatLon_deg(start.Latitude, start.Longitude);
    }

    //Set postion
	public void setPositionOnMap () {
		Vector2 tempPosition = GameManager.Instance.getMainMapMap ().getPositionOnMap (this.pos);
        Debug.Log(tempPosition);
		transform.position = new Vector3 (tempPosition.x, transform.position.y, tempPosition.y);
	}

	public void setPositionOnMap (GeoPoint pos) {
		this.pos = pos;
		setPositionOnMap ();
	}

    public void SetSprite()
    {
        User user = (User)GameData.Instance.RequestUser(userid: routeName);
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = spriteHolder.GetSprite(user.CurrentSkin);
    }

}

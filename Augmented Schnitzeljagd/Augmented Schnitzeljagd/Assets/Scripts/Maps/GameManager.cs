using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//GameManager
//
//
public class GameManager : Singleton<GameManager> {

	[HideInInspector]
	public bool locationServicesIsRunning = false;

	public GameObject mainMap;
	public GameObject newMap;

	public GameObject player;
	public GeoPoint playerGeoPosition;
	public PlayerLocationService player_loc;

	public enum PlayerStatus { TiedToDevice, FreeFromDevice }

	private PlayerStatus _playerStatus;
	public PlayerStatus playerStatus
	{
		get { return _playerStatus; }
		set { _playerStatus = value; }
	}

	void Awake (){

		Time.timeScale = 1;
		playerStatus = PlayerStatus.TiedToDevice;

        //Disable second map
		player_loc = player.GetComponent<PlayerLocationService>();
		newMap.GetComponent<MeshRenderer>().enabled = false;
		newMap.SetActive (false);

	}

	public GoogleStaticMap getMainMapMap () {
		return mainMap.GetComponent<GoogleStaticMap> ();
	}

	public GoogleStaticMap getNewMapMap () {
		return newMap.GetComponent<GoogleStaticMap> ();
	}

	IEnumerator Start () {
        
        //Init
		getMainMapMap ().initialize ();
		yield return StartCoroutine (player_loc._StartLocationService ());
		StartCoroutine (player_loc.RunLocationService ());

		locationServicesIsRunning = player_loc.locServiceIsRunning;
		Debug.Log ("Player loc from GameManager: " + player_loc.loc);
		getMainMapMap ().centerMercator = getMainMapMap ().tileCenterMercator (player_loc.loc);
		getMainMapMap ().DrawMap ();

		mainMap.transform.localScale = Vector3.Scale (
			new Vector3 (getMainMapMap ().mapRectangle.getWidthMeters (), getMainMapMap ().mapRectangle.getHeightMeters (), 1.0f),
			new Vector3 (getMainMapMap ().realWorldtoUnityWorldScale.x, getMainMapMap ().realWorldtoUnityWorldScale.y, 1.0f));

        //Set Position of Player
        Coordinate c = new Coordinate();
        c.Latitude = GameData.Instance.Me.Position.Latitude;
        c.Longitude = GameData.Instance.Me.Position.Longitude;
        player.GetComponent<ObjectPosition>().start = c;
        player.GetComponent<ObjectPosition>().SetGeoPoint();
        player.GetComponent<ObjectPosition> ().setPositionOnMap (player_loc.loc);

        // Breaking for debugging
        yield break;

        //Set Position of GameObjects
		//GameObject[] objectsOnMap = GameObject.FindGameObjectsWithTag ("ObjectOnMap");

  //      List<Route> routes = MyDataBase.Instance.GetRoutesByStartingLocation(new Vector2(player_loc.loc.lat_d, player_loc.loc.lon_d), 100);

  //      //Need to fix TODO
  //      foreach (GameObject obj in objectsOnMap)
  //      {
  //          obj.SetActive(false);
  //      }
  //      foreach (GameObject obj in objectsOnMap) {
  //          if (routes.Count == 0)
  //          {
  //              break;
  //          }
  //          obj.GetComponent<ObjectPosition>().setLatLon_deg(routes[0].getCoordinates()[0].Latitude, routes[0].getCoordinates()[0].Longitude);
  //          routes.Remove(routes[0]);
		//	obj.GetComponent<ObjectPosition> ().setPositionOnMap ();
  //          obj.SetActive(true);
		//}
    }

    //Update Map
    void Update () {
		if(!locationServicesIsRunning){

			//TODO: Show location service is not enabled error. 
			return;
		}

		// playerGeoPosition = getMainMapMap ().getPositionOnMap(new Vector2(player.transform.position.x, player.transform.position.z));
		playerGeoPosition = new GeoPoint();
		// GeoPoint playerGeoPosition = getMainMapMap ().getPositionOnMap(new Vector2(player.transform.position.x, player.transform.position.z));
		if (playerStatus == PlayerStatus.TiedToDevice) {
			playerGeoPosition = player_loc.loc;
			player.GetComponent<ObjectPosition> ().setPositionOnMap (playerGeoPosition);
		} else if (playerStatus == PlayerStatus.FreeFromDevice){
			playerGeoPosition = getMainMapMap ().getPositionOnMap(new Vector2(player.transform.position.x, player.transform.position.z));
		}


		var tileCenterMercator = getMainMapMap ().tileCenterMercator (playerGeoPosition);
		if(!getMainMapMap ().centerMercator.isEqual(tileCenterMercator)) {

			newMap.SetActive(true);
			getNewMapMap ().initialize ();
			getNewMapMap ().centerMercator = tileCenterMercator;

			getNewMapMap ().DrawMap ();

			getNewMapMap ().transform.localScale = Vector3.Scale(
				new Vector3 (getNewMapMap ().mapRectangle.getWidthMeters (), getNewMapMap ().mapRectangle.getHeightMeters (), 1.0f),
				new Vector3(getNewMapMap ().realWorldtoUnityWorldScale.x, getNewMapMap ().realWorldtoUnityWorldScale.y, 1.0f));	

			Vector2 tempPosition = GameManager.Instance.getMainMapMap ().getPositionOnMap (getNewMapMap ().centerLatLon);
			newMap.transform.position = new Vector3 (tempPosition.x, 0, tempPosition.y);

			GameObject temp = newMap;
			newMap = mainMap;
			mainMap = temp;

		}
		if(getMainMapMap().isDrawn && mainMap.GetComponent<MeshRenderer>().enabled == false){
			mainMap.GetComponent<MeshRenderer>().enabled = true;
			newMap.GetComponent<MeshRenderer>().enabled = false;
			newMap.SetActive(false);
		}
	}

	public Vector3? ScreenPointToMapPosition(Vector2 point){
		var ray = Camera.main.ScreenPointToRay(point);
		//RaycastHit hit;
		// create a plane at 0,0,0 whose normal points to +Y:
		Plane hPlane = new Plane(Vector3.up, Vector3.zero);
		float distance = 0; 
		if (!hPlane.Raycast (ray, out distance)) {
			// get the hit point:
			return null;
		}
		Vector3 location = ray.GetPoint (distance);
		return location;
	}

}

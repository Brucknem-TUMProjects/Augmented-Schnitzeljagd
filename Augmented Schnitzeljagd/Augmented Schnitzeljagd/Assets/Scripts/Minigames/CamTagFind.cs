using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTagFind : MonoBehaviour {

    CamTag c;

	// Use this for initialization
	void Start () {
        c = GetComponentInChildren<CamTag>();
        //c.Win = true;
        try
        {
            Route route = (Route)GameData.Instance.RequestRoute(GameData.Instance.Me.CurrentRoute);
            Coordinate coord = route.Coordinates[GameData.Instance.Me.RouteProgress + 1];
            c.PlaceTagAt(new Vector3(coord.C1, coord.C2, coord.C3));
        }catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}

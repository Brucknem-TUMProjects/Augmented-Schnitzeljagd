using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CamTag : Minigame {

    public GameObject camTag;
    public GameObject confButt;

    private WebCamTexture deviceCam = null;
    private Image mScreen;


    private GameObject mainCam;
    private GameObject mCamHolder;
    private Quaternion rot;

    private bool gyroEnabled;

    public bool BlockGyro { get; set; }


    protected override void startGame()
    {
        //Win = false;

        // set up in game cam
        mainCam = FindObjectOfType<Camera>().gameObject;
        mCamHolder = new GameObject("Camera Holder");
        mCamHolder.transform.position = mainCam.transform.position;
        mainCam.transform.SetParent(mCamHolder.transform);

        gyroEnabled = EnableGyro();

        // lock display rotation 
        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.Portrait;

        // activate cam
        deviceCam = new WebCamTexture(Screen.width, Screen.height);
        mScreen = GetComponent<Image>();
        mScreen.defaultMaterial.mainTexture = deviceCam;
        deviceCam.Play();
        
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (gyroEnabled)
            mainCam.transform.localRotation = Input.gyro.attitude * rot;
        //GyroModifyCamera();
    }
    
    public void PlaceTag()
    {
        if (!confButt.activeSelf)
            confButt.SetActive(true);
        camTag.SetActive(true);
        camTag.transform.position = mainCam.transform.localPosition + mainCam.transform.forward;
    }

    public void ConfirmTag()
    {
        Win = false;
        Coordinate curr = GameData.Instance.CreatingRoute.Coordinates[GameData.Instance.CreatingRoute.Coordinates.Count - 1];
        curr.C1 = camTag.transform.position.x;
        curr.C2 = camTag.transform.position.y;
        curr.C3 = camTag.transform.position.z;

        GameData.Instance.CreatingRoute.Coordinates[GameData.Instance.CreatingRoute.Coordinates.Count - 1] = curr ;
        Debug.Log("Confirm tag placement");
        endGame();
    }

    public void FindTag()
    {
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, 5))
        {
            Win = true;
            Debug.Log("Tag found");
            endGame();
        }
    }

    public void PlaceTagAt(Vector3 pos)
    {
        if(!camTag.activeSelf)
            camTag.SetActive(true);
        camTag.transform.position = pos;
    }

    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;

            mCamHolder.transform.rotation = Quaternion.Euler(90, 90, 0);
            rot = new Quaternion(0, 0, 1, 0);

            return true;
        }
        return false;
    }
    
    // old functions for gyroscope setup
    //private void GyroModifyCamera()
    //{
    //    if (!blockGyro)
    //        mainCam.transform.localRotation = GyroToUnity(Input.gyro.attitude);
    //}

    //private static Quaternion GyroToUnity(Quaternion q)
    //{
    //    return new Quaternion(q.x, q.y, -q.z, -q.w);
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Net.Sockets;

public class MapTab : AbstractTab
{
    public Camera cam;
    public SpriteHolder spriteHolder;

    public Transform idleTransform;
    public Transform createTransform;
    public Transform playTransform;
    public Transform otherUserTransform;

    public GameObject startPoint;
    public GameObject checkpoint;
    public GameObject otherUser;

    public GameObject[] points;

    List<GameObject> startPointList;
    GameObject nextCheckpoint;
    List<GameObject> otherUsers;

    // active route
    User other;
    Route route;

    // idle
    GameObject refreshButton;
    GameObject createRouteButton;

    // create route
    Transform createRouteMenu;
    GameObject abortCreateButton;
    GameObject abortCreateMenu;
    GameObject addCheckpointButton;
    Transform addCheckpointMenu;
    GameObject finishCreateButton;
    Transform confirmFinishCreateMenu;
    Transform finishCreateMenu;

    // play route
    Transform startPlayMenu;
    GameObject showHintButton;
    Transform showHintMenu;
    GameObject giveUpButton;
    Transform giveUpMenu;
    Transform checkpointMenu;
    Transform finishMenu;

    // other user
    Transform addFriendMenu;
    Transform confirmAddFriendMenu;

    // Use this for initialization
    void Awake()
    {
        #region Idle Components
        refreshButton = idleTransform.Find("RefreshButton").gameObject;
        createRouteButton = idleTransform.Find("CreateRouteButton").gameObject;
        #endregion

        #region Create Components
        createRouteMenu = createTransform.Find("CreateRouteMenu");
        abortCreateButton = createTransform.Find("AbortCreateButton").gameObject;
        abortCreateMenu = createTransform.Find("AbortCreateMenu").gameObject;
        addCheckpointButton = createTransform.Find("AddCheckpointButton").gameObject;
        addCheckpointMenu = createTransform.Find("AddCheckpointMenu");
        finishCreateButton = createTransform.Find("FinishCreateButton").gameObject;
        confirmFinishCreateMenu = createTransform.Find("ConfirmFinishCreateMenu");
        finishCreateMenu = createTransform.Find("FinishCreateMenu");
        #endregion

        #region Play Components
        startPlayMenu = playTransform.Find("StartPlayMenu");
        showHintButton = playTransform.Find("ShowHintButton").gameObject;
        showHintMenu = playTransform.Find("ShowHintMenu");
        giveUpButton = playTransform.Find("GiveUpButton").gameObject;
        giveUpMenu = playTransform.Find("GiveUpMenu");
        checkpointMenu = playTransform.Find("CheckpointMenu");
        finishMenu = playTransform.Find("FinishMenu");
        #endregion

        #region Other User Components
        addFriendMenu = otherUserTransform.Find("AddFriendMenu");
        confirmAddFriendMenu = otherUserTransform.Find("ConfirmAddFriendMenu");
        #endregion

        startPointList = new List<GameObject>();
        otherUsers = new List<GameObject>();
    }

    void Start()
    {
        if (GameData.Instance.CreatingRoute != null)
        {
            EnableCreateTransform(true);
            EnableCreateButtons(true);
        }
        else if (GameData.Instance.Me.CurrentRoute == "")
        {
            EnableIdleTransform(true);
        }
        else
        {
            EnablePlayTransform(true);

            try
            {
                route = GameData.Instance.RequestRoute(routeName: GameData.Instance.Me.CurrentRoute);

                if (GameData.Instance.Me.RouteProgress < route.Coordinates.Count - 1)
                {
                    showHintMenu.gameObject.SetActive(true);
                    showHintMenu.Find("Text").GetComponent<Text>().text = route.Coordinates[GameData.Instance.Me.RouteProgress + 1].Hint;
                }
                else
                {
                    long newTime = (long)(GameData.Instance.GetServerTime().Ticks / Mathf.Pow(10, 7));
                    long seconds = (long)((newTime - GameData.Instance.Me.RouteTime) * Mathf.Pow(10, 7));
                    TimeSpan timeSpan = new TimeSpan(seconds);
                    finishMenu.Find("YourTime").GetComponent<Text>().text = timeSpan.Hours + ":"
                        + ((timeSpan.Minutes < 10) ? "0" + timeSpan.Minutes : "" + timeSpan.Minutes) + ":"
                        + ((timeSpan.Seconds < 10) ? "0" + timeSpan.Seconds : "" + timeSpan.Seconds);
                    finishMenu.gameObject.SetActive(true);

                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        OnSwitchToThisTab();
    }

    // Update is called once per frame
    void Update()
    {
        if (createRouteMenu.gameObject.activeInHierarchy)
        {
            createRouteMenu.Find("Coordinates").GetComponent<Text>().text = "Latitude: " + GameData.Instance.Me.Position.Latitude
                + "\nLongitude: " + GameData.Instance.Me.Position.Longitude;
        }
        if (addCheckpointMenu.gameObject.activeInHierarchy)
        {
            addCheckpointMenu.Find("Coordinates").GetComponent<Text>().text = "Latitude: " + GameData.Instance.Me.Position.Latitude
                + "\nLongitude: " + GameData.Instance.Me.Position.Longitude;
        }

        // Click on Object
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            SendRay(true);
        if (Input.GetButtonDown("Fire1"))
            SendRay(false);

        if (nextCheckpoint != null)
        {
            if (Vector3.Distance(cam.transform.parent.position, nextCheckpoint.transform.position) > 5f)
                nextCheckpoint.gameObject.SetActive(false);
            else
                nextCheckpoint.gameObject.SetActive(true);
        }
    }

    void SendRay(bool touch)
    {
        if (!MenuOpened() && (!EventSystem.current.IsPointerOverGameObject(0) || !EventSystem.current.IsPointerOverGameObject()))
        {
            Ray ray = (touch) ? cam.ScreenPointToRay(Input.GetTouch(0).position) : cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 100);

            if (Physics.Raycast(ray, out hit, 100))
            {
                // StartPoint
                if (hit.transform.gameObject.layer == 10)
                {
                    try
                    {
                        string routeName = hit.transform.GetComponent<ObjectPosition>().routeName;
                        route = GameData.Instance.RequestRoute(routeName: routeName);

                        EnablePlayTransform(true);
                        startPlayMenu.gameObject.SetActive(true);

                        startPlayMenu.Find("HeadText").GetComponent<Text>().text = route.RouteName;

                        float rateValue = 3.5f;
                        Image[] rating = startPlayMenu.Find("Rating").Find("Fill").GetComponentsInChildren<Image>();
                        for (int i = 0; i < rating.Length; i++)
                        {
                            if (rateValue >= 1)
                            {
                                rating[i].fillAmount = 1;
                                rateValue--;
                            }
                            else
                            {
                                rating[i].fillAmount = rateValue;
                                rateValue = 0;
                            }
                        }
                        startPlayMenu.Find("Text").GetComponent<Text>().text = route.Coordinates[0].Hint;
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }

                // Checkpoint
                if (hit.transform.gameObject.layer == 11)
                {
                    showHintButton.SetActive(false);
                    checkpointMenu.gameObject.SetActive(true);
                }

                // other user
                if (hit.transform.gameObject.layer == 12)
                {
                    try
                    {
                        string id = hit.transform.parent.GetComponent<ObjectPosition>().routeName;
                        other = GameData.Instance.RequestUser(userid: id);
                        idleTransform.gameObject.SetActive(false);
                        addFriendMenu.gameObject.SetActive(true);
                        addFriendMenu.Find("HeadText").GetComponent<Text>().text = other.Username;
                        addFriendMenu.Find("Sprite").GetComponent<Image>().sprite = spriteHolder.GetSprite(other.CurrentSkin);
                        addFriendMenu.Find("Sprite").GetComponent<Image>().SetNativeSize();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }

        }
    }

    bool MenuOpened()
    {
        return createRouteMenu.gameObject.activeInHierarchy
            || abortCreateMenu.activeInHierarchy
            || addCheckpointMenu.gameObject.activeInHierarchy
            || finishCreateMenu.gameObject.activeInHierarchy
            || confirmFinishCreateMenu.gameObject.activeInHierarchy
            || startPlayMenu.gameObject.activeInHierarchy
            || showHintMenu.gameObject.activeInHierarchy
            || giveUpMenu.gameObject.activeInHierarchy
            || finishMenu.gameObject.activeInHierarchy
            || addFriendMenu.gameObject.activeInHierarchy
            || confirmAddFriendMenu.gameObject.activeInHierarchy;
    }

    //---------------------------------------------------------------------------------------------//
    public void RefreshButton()
    {
        //OtherUsers
        foreach (GameObject obj in otherUsers)
            Destroy(obj);
        otherUsers.Clear();

        try
        {
            StringList userList = GameData.Instance.RequestNearbyUsers(start: GameData.Instance.Me.Position, distance: 100);

            foreach (string u in userList)
            {
                if (!u.Equals(GameData.Instance.Me.UserID))
                {
                    User user = (User)GameData.Instance.RequestUser(userid: u);
                    GameObject obj = (GameObject)Instantiate(otherUser);
                    Coordinate coord = new Coordinate()
                    {
                        Latitude = user.Position.Latitude,
                        Longitude = user.Position.Longitude
                    };
                    obj.GetComponent<ObjectPosition>().start = coord;
                    obj.GetComponent<ObjectPosition>().routeName = user.UserID;
                    obj.GetComponent<ObjectPosition>().SetSprite();
                    obj.GetComponent<ObjectPosition>().SetGeoPoint();
                    obj.GetComponent<ObjectPosition>().setPositionOnMap();
                    otherUsers.Add(obj);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        // Routes
        DestroyStartPoints();

        //Debug.Log(GameData.Instance.RequestServer(GameData.Instance.Me));

        try
        {
            StringList routeList = GameData.Instance.RequestRoutesByStart(GameData.Instance.Me.Position, 10);

            foreach (string r in routeList)
            {
                Route rTemp = (Route)GameData.Instance.RequestRoute(routeName: r);
                Debug.Log(rTemp);
                GameObject obj = (GameObject)Instantiate(startPoint);
                obj.GetComponent<ObjectPosition>().start = rTemp.Coordinates[0];
                obj.GetComponent<ObjectPosition>().routeName = rTemp.RouteName;
                obj.GetComponent<ObjectPosition>().SetGeoPoint();
                obj.GetComponent<ObjectPosition>().setPositionOnMap();
                startPointList.Add(obj);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void DestroyStartPoints()
    {
        foreach (GameObject obj in startPointList)
            Destroy(obj);
        startPointList.Clear();
    }

    void EnableIdleTransform(bool enable)
    {
        idleTransform.gameObject.SetActive(enable);
        if (enable)
        {
            //GetNearbyRoutesButton();
            EnableCreateTransform(false);
            EnablePlayTransform(false);
        }
        else
        {
            DestroyStartPoints();
        }

    }
    void EnableCreateTransform(bool enable)
    {
        createTransform.gameObject.SetActive(enable);
        if (enable)
        {
            EnableIdleTransform(false);
            EnablePlayTransform(false);
        }
    }
    void EnablePlayTransform(bool enable)
    {
        playTransform.gameObject.SetActive(enable);
        if (enable)
        {
            EnableIdleTransform(false);
            EnableCreateTransform(false);
        }
    }

    #region Create Route
    // Create Start
    public void CreateRouteButton()
    {
        EnableCreateTransform(true);

        InputField nameField = createRouteMenu.Find("NameField").GetComponent<InputField>();
        Text nameText = createRouteMenu.Find("NameText").GetComponent<Text>();
        InputField descriptionField = createRouteMenu.Find("DescriptionField").GetComponent<InputField>();

        nameText.color = Color.black;
        nameText.text = "Give this route a name!";
        nameField.text = "";
        descriptionField.text = "";

        createRouteMenu.gameObject.SetActive(true);
    }

    public void CreateRoute()
    {
        InputField nameField = createRouteMenu.Find("NameField").GetComponent<InputField>();
        Text nameText = createRouteMenu.Find("NameText").GetComponent<Text>();

        bool routeExistsAlready = true;

        try
        {
            GameData.Instance.RequestRoute(routeName: nameField.text);
        }
        catch (Exception e)
        {
            routeExistsAlready = false;
            Debug.Log(e.Message);
        }

        if (nameField.text == "" || routeExistsAlready)
        {
            nameText.text = "You can't use this name.";
            nameText.color = Color.red;
        }
        else
        {
            // create new Route
            GameData.Instance.CreatingRoute = new Route()
            {
                // set Name
                RouteName = nameField.text
            };

            // initialize coord list
            Coordinate coord = new Coordinate()
            {
                Latitude = GameData.Instance.Me.Position.Latitude,
                Longitude = GameData.Instance.Me.Position.Longitude,
                GameType = 0
            };
            InputField descriptionField = createRouteMenu.Find("DescriptionField").GetComponent<InputField>();
            coord.Hint = descriptionField.text;
            GameData.Instance.CreatingRoute.Coordinates.Add(coord);

            // UI
            EnableCreateButtons(true);
            createRouteMenu.gameObject.SetActive(false);
        }
    }

    public void CancelCreate()
    {
        createRouteMenu.gameObject.SetActive(false);
        EnableIdleTransform(true);
    }

    // Abort Creation
    public void AbortCreateButton(bool show)
    {
        abortCreateMenu.SetActive(show);
        if (show)
        {
            EnableCreateTransform(true);
        }
        else
        {
            EnableIdleTransform(true);
            RefreshButton();
        }

    }

    public void AbortCreation()
    {
        GameData.Instance.CreatingRoute = null;
        abortCreateMenu.SetActive(false);
        EnableIdleTransform(true);
    }

    void EnableCreateButtons(bool enable)
    {
        addCheckpointButton.SetActive(enable);
        abortCreateButton.SetActive(enable);
        if (GameData.Instance.CreatingRoute.Coordinates.Count > 1)
            finishCreateButton.SetActive(enable);
    }

    // Add Checkpoint
    public void AddCheckpointButton()
    {
        EnableCreateButtons(false);

        Text hintText = addCheckpointMenu.Find("HintText").GetComponent<Text>();
        InputField hintField = addCheckpointMenu.Find("HintField").GetComponent<InputField>();
        Slider difficultySlider = addCheckpointMenu.Find("DifficultySlider").GetComponent<Slider>();

        hintText.color = Color.black;
        hintField.text = "";
        difficultySlider.value = 1;

        addCheckpointMenu.gameObject.SetActive(true);
    }

    public void AddCheckpoint()
    {
        Text hintText = addCheckpointMenu.Find("HintText").GetComponent<Text>();
        InputField hintField = addCheckpointMenu.Find("HintField").GetComponent<InputField>();

        if (hintField.text == "")
        {
            hintText.color = Color.red;
        }
        else
        {
            // add new coordinate
            Coordinate coord = new Coordinate()
            {
                Latitude = GameData.Instance.Me.Position.Latitude,
                Longitude = GameData.Instance.Me.Position.Longitude,
                GameType = GetGameType(),
                Hint = hintField.text
            };
            Slider difficultySlider = addCheckpointMenu.Find("DifficultySlider").GetComponent<Slider>();
            coord.Difficulty = (int)difficultySlider.value;
            GameData.Instance.CreatingRoute.Coordinates.Add(coord);

            if (coord.GameType == 1)
                SetCameraTag();

            // UI
            addCheckpointMenu.gameObject.SetActive(false);
            EnableCreateButtons(true);
        }
    }

    public void CancelAddCheckpoint()
    {
        addCheckpointMenu.gameObject.SetActive(false);
        EnableCreateButtons(true);
    }

    int GetGameType()
    {
        Toggle[] gameTypes = addCheckpointMenu.GetComponentsInChildren<Toggle>();

        int type = 0;
        for (int i = 0; i < gameTypes.Length; i++)
        {
            if (gameTypes[i].isOn)
            {
                type = i + 1;
                break;
            }
        }
        return type;
    }

    void SetCameraTag()
    {
        SceneManager.LoadScene("CamTag");
    }

    // Finish Create
    public void EnableConfirmFinishMenu(bool enable)
    {
        EnableCreateButtons(!enable);
        confirmFinishCreateMenu.gameObject.SetActive(enable);
    }

    public void ConfirmFinishButton()
    {
        try
        {
            GameData.Instance.CreateRoute(GameData.Instance.CreatingRoute);
            GameData.Instance.CreatingRoute = null;
            confirmFinishCreateMenu.gameObject.SetActive(false);
            finishCreateMenu.gameObject.SetActive(true);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void EndCreateRoute()
    {
        finishCreateMenu.gameObject.SetActive(false);

        EnableIdleTransform(true);
    }
    #endregion

    #region Play Route
    public void StartPlay()
    {
        DestroyStartPoints();

        Debug.Log(GameData.Instance.Me.CurrentRoute + " --- " + route.RouteName);
        GameData.Instance.Me.CurrentRoute = route.RouteName;
        Debug.Log(GameData.Instance.Me.CurrentRoute);
        GameData.Instance.Me.RouteProgress = 0;
        GameData.Instance.Me.RouteTime = (long)(GameData.Instance.GetServerTime().Ticks / Mathf.Pow(10, 7));
        try
        {
            GameData.Instance.UpdateMeOnServer();
            startPlayMenu.gameObject.SetActive(false);
            showHintMenu.gameObject.SetActive(true);
            showHintMenu.Find("Text").GetComponent<Text>().text = route.Coordinates[1].Hint;
            SetNextCheckpoint();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }

    public void AbortStartPlay()
    {
        startPlayMenu.gameObject.SetActive(false);
        EnableIdleTransform(true);
        RefreshButton();
    }

    public void ShowHint(bool show)
    {
        showHintMenu.gameObject.SetActive(show);
        showHintButton.SetActive(!show);
        showHintMenu.Find("Text").GetComponent<Text>().text = route.Coordinates[GameData.Instance.Me.RouteProgress + 1].Hint;
        giveUpButton.SetActive(!show);
        if (!show && nextCheckpoint == null)
        {
            cam.transform.parent.GetChild(2).gameObject.SetActive(true);
            SetNextCheckpoint();
        }
    }

    void SetNextCheckpoint()
    {
        nextCheckpoint = (GameObject)Instantiate(checkpoint);
        nextCheckpoint.GetComponent<ObjectPosition>().start = route.Coordinates[GameData.Instance.Me.RouteProgress + 1];
        nextCheckpoint.GetComponent<ObjectPosition>().SetGeoPoint();
        nextCheckpoint.GetComponent<ObjectPosition>().setPositionOnMap();
    }

    public void GiveUpButton(bool show)
    {
        giveUpButton.SetActive(!show);
        giveUpMenu.gameObject.SetActive(show);
        showHintButton.SetActive(!show);
    }

    public void GiveUp()
    {
        cam.transform.parent.GetChild(2).gameObject.SetActive(false);
        Destroy(nextCheckpoint);
        nextCheckpoint = null;
        giveUpMenu.gameObject.SetActive(false);
        EnableIdleTransform(true);
        RefreshButton();

        GameData.Instance.Me.CurrentRoute = "";
        GameData.Instance.Me.RouteProgress = 0;
        GameData.Instance.Me.RouteTime = 0;
        try
        {
            GameData.Instance.UpdateMeOnServer();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void PlayHintButton()
    {
        //Strings.UpdateMeOnServer();
        switch (route.Coordinates[GameData.Instance.Me.RouteProgress + 1].GameType)
        {
            case 1:
                SceneManager.LoadScene("FindCamTag");
                break;
            case 2:
                SceneManager.LoadScene("WhackAMole");
                break;
            case 3:
                SceneManager.LoadScene("TapTapBoom");
                break;
        }
    }

    public void RateRoute(int rateValue)
    {
        Transform stars = finishMenu.Find("Rating").Find("Fill");
        for (int i = 0; i < stars.childCount; i++)
        {
            stars.GetChild(i).gameObject.SetActive(i <= rateValue);
        }
    }

    public void FinishPlay()
    {
        finishMenu.gameObject.SetActive(false);
        EnableIdleTransform(true);
        RefreshButton();

        GameData.Instance.Me.CurrentRoute = "";
        GameData.Instance.Me.RouteProgress = 0;
        try
        {
            GameData.Instance.UpdateMeOnServer();
            UserStats userStats = GameData.Instance.RequestUserStats(GameData.Instance.Me.UserID);
            userStats.Coins += 20;
            GameData.Instance.UpdateUserStats(userStats);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    #endregion

    #region Other User
    public void AddFriend()
    {
        // check if already friend
        bool friend = false;
        StringList friendList = GameData.Instance.Me.FriendsIDs;
        foreach (string f in friendList)
        {
            if (f.Equals(other.UserID))
            {
                friend = true;
                break;
            }
        }

        if (!friend)
        {
            GameData.Instance.Me.FriendsIDs.Add(other.UserID);
            try
            {
                GameData.Instance.UpdateMeOnServer();
                confirmAddFriendMenu.Find("Text").GetComponent<Text>().text = "This user is now in your friendlist.";
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        else
            confirmAddFriendMenu.Find("Text").GetComponent<Text>().text = "This user has already been added to your friendlist.";

        confirmAddFriendMenu.Find("HeadText").GetComponent<Text>().text = other.Username;
        confirmAddFriendMenu.gameObject.SetActive(true);
        addFriendMenu.gameObject.SetActive(false);
    }

    public void AbortAddFriend()
    {
        addFriendMenu.gameObject.SetActive(false);
        idleTransform.gameObject.SetActive(true);
    }

    public void EndAddFriend()
    {
        confirmAddFriendMenu.gameObject.SetActive(false);
        idleTransform.gameObject.SetActive(true);
    }
    #endregion

    public override void OnSwitchToThisTab()
    {
        cam.transform.parent.GetChild(1).GetComponent<SpriteRenderer>().sprite = spriteHolder.GetSprite(GameData.Instance.Me.CurrentSkin);
        Debug.Log("Sprite set");
    }
}

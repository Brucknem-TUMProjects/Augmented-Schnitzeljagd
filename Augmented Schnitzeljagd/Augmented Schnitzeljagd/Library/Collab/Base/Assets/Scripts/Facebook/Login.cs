using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public GameObject LoginTypeChooser;
    public GameObject LoginWithUsernameScreen;
    public GameObject CreateUserScreen;
    // public GameObject ConnectToServerScreen;

    public InputField ip, port;
    public Button connect, facebook, custom;
    public GameObject serverSettings;

    private string sceneToLoad = "MainMenu";

    private void Start()
    {
        OnConnect();

        //if (GameData.Instance.IsConnected)
        //{
        //  //  ConnectToServerScreen.GetComponentInChildren<Text>().text = Strings.CONNECTION_SUCCESS;
        //  //  ConnectToServerScreen.SetActive(false);
        //    ShowLoginTypeChooser();
        //}
        //else
        //{
        //    ConnectToServerScreen.GetComponentInChildren<Text>().text = Strings.CONNECTION_FAILED;
        //    return;
        //}

        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback);
        }
        //ShowLoginTypeChooser();
    }

    private void Update()
    {
        serverSettings.SetActive(!GameData.Instance.IsConnected);
    }

    public void OnConnect()
    {
        GameData.Instance.ConnectServer(ip.text, port.text);

        if (GameData.Instance.IsConnected)
        {
            facebook.GetComponent<Button>().interactable = true;
            custom.GetComponent<Button>().interactable = true;
        }
        else
        {
            facebook.GetComponent<Button>().interactable = false;
            custom.GetComponent<Button>().interactable = false;
        }
    }

    public void LoginWithFacebook()
    {
        if (!FB.IsLoggedIn)
        {
            Debug.Log("Logging in.");
            FB.LogInWithReadPermissions(new List<string> { "public_profile", "user_friends" }, LoginCallback);
        }
        else
        {
            StartCoroutine(CreateFacebookUser());
            Debug.Log("Already Logged in");
        }
    }

    /// <summary>
    /// Logins the with username.
    /// </summary>
    public void LoginWithUsername()
    {
        string username = "", password = "";
        InputField[] inputFields = LoginWithUsernameScreen.GetComponentsInChildren<InputField>();
        foreach (InputField i in inputFields)
        {
            switch (i.name)
            {
                case "Username":
                    username = i.text;
                    break;
                case "Password":
                    password = i.text;
                    break;
            }
        }

        string response = GameData.Instance.RequestServer(Strings.REQUEST_USER + ":" + Strings.USERNAME + "=" + username + ";" + Strings.PASSWORD + "=" + password);
        LoginWithUsernameScreen.GetComponentsInChildren<Text>()[0].GetComponent<Text>().text = response;

        Debug.Log(response);
        Debug.Log("Now logging in");
        if (!response.Equals(Strings.USER_NOT_FOUND))
        {
            GameData.Instance.Me = (User)response;
            LoadMainMenu();
        }
    }

    public void CreateUser()
    {
        string username = "", password = "", passwordConfirm = "";
        InputField[] inputFields = CreateUserScreen.GetComponentsInChildren<InputField>();
        foreach (InputField i in inputFields)
        {
            switch (i.name)
            {
                case "Username":
                    username = i.text;
                    break;
                case "Password":
                    password = i.text;
                    break;
                case "PasswordConfirm":
                    passwordConfirm = i.text;
                    break;
            }
        }

        Text errorMessage = CreateUserScreen.GetComponentsInChildren<Text>()[0].GetComponent<Text>();
        
        if (!password.Equals(passwordConfirm))
        {
            errorMessage.text = "Passwords have to be equal";
            return;
        }

        if(password.Length < 4 || password.Length > 10)
        {
            errorMessage.text = "Password length must be between 4 and 10";
            return;
        }

        string response = GameData.Instance.RequestServer(Strings.CREATE_USER + ":" + Strings.USERNAME + "=" + username + ";" + Strings.PASSWORD + "=" + password);
        if (!response.Equals(Strings.USER_NOT_FOUND))
        {
            ShowLoginTypeChooser();
        }
        else
        {
            errorMessage.text = response;
        }

    }

    void InitCallback()
    {
        Debug.Log("Facebook has been initialized");
    }

    void LoginCallback(ILoginResult result)
    {
        Debug.Log("LoginCallback: " + result.RawResult);
        if (result.Error == null && !result.Cancelled)
        {
            Debug.Log("Facebook has logged in");
            StartCoroutine(CreateFacebookUser());
        }
        else
        {
            ShowLoginTypeChooser();
            Debug.Log("Error during login: " + result.Error);
        }
    }

    IEnumerator CreateFacebookUser()
    {
        Debug.Log("Creating new facebook user.");
        GameData.Instance.Me = new User();
        yield return null;

        yield return GameData.Instance.Me.GenerateFacebookUser();

        string response = GameData.Instance.RequestServer(Strings.REQUEST_USER + ":" + Strings.USER_ID + "=" + GameData.Instance.Me.UserID);
        
        if(response.Equals(Strings.USER_NOT_FOUND))
        {
            response = GameData.Instance.RequestServer(Strings.CREATE_USER + ":" + GameData.Instance.Me);
        }

        GameData.Instance.Me = new User(response);

        LoadMainMenu();
        
    }

    private void LoadMainMenu()
    {
        GameData.Instance.Me.IsLoggedIn = true;
        Strings.UpdateMeOnServer();
        SceneManager.LoadScene(sceneToLoad);
    }

    public void ShowLoginTypeChooser()
    {
        LoginTypeChooser.SetActive(true);
        LoginWithUsernameScreen.SetActive(false);
        CreateUserScreen.SetActive(false);
    }

    public void ShowLoginWithUsernameScreen()
    {
        LoginTypeChooser.SetActive(false);
        LoginWithUsernameScreen.SetActive(true);
        CreateUserScreen.SetActive(false);
    }

    public void ShowCreateUserScreen()
    {
        LoginTypeChooser.SetActive(false);
        LoginWithUsernameScreen.SetActive(false);
        CreateUserScreen.SetActive(true);
    }

    public void Cancel()
    {
        ShowLoginTypeChooser();
    }
}
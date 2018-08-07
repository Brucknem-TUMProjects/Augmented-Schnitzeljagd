using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Net.Sockets;

public class Login : MonoBehaviour
{
    public GameObject LoginTypeChooser;
    public GameObject LoginWithUsernameScreen;
    public GameObject CreateUserScreen;
    // public GameObject ConnectToServerScreen;

    
    public Button facebook, custom;
    public GameObject serverSettings;

    private string sceneToLoad = "MainMenu";

    private void Start()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        serverSettings.SetActive(!GameData.Instance.SocketConnected);
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

        try
        {
            GameData.Instance.Me = GameData.Instance.RequestUser(username: username, password: password);
            LoadMainMenu();
        }
        catch (Exception e)
        {
            LoginWithUsernameScreen.GetComponentsInChildren<Text>()[0].GetComponent<Text>().text = e.Message;
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

        try
        {
            GameData.Instance.CreateUser(username: username, password: password);
            ShowLoginTypeChooser();
        }catch(Exception e)
        {
            errorMessage.text = e.Message;
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

        try
        {
            GameData.Instance.Me = GameData.Instance.RequestUser(userid: GameData.Instance.Me.UserID);
            LoadMainMenu();
        }
        catch (SocketException e)
        {
            Debug.Log(e.Message);
        }catch(Exception e)
        {
            Debug.Log(e.Message);

            try
            {
                GameData.Instance.Me = GameData.Instance.CreateUser(user:GameData.Instance.Me);
                LoadMainMenu();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }

    private void LoadMainMenu()
    {
        GameData.Instance.Me.IsLoggedIn = true;
        GameData.Instance.UpdateMeOnServer();
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
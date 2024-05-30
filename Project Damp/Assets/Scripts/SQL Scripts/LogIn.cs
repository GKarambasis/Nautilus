using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LogIn : MonoBehaviour
{
    [Header("Minimum Input Length for Username and Password")]
    public int usernameInputLength = 4;
    public int passwordInputLength = 8;

    //Input Fields
    [Header("GameObject Assignation")]
    public TMP_InputField nameField;
    public TMP_InputField passwordField;

    public Button submitButton;

    public GameObject startScreen;
    public GameObject accountScreen;
    public GameObject lobbyScreen;
    public GameObject loadingScreen; //Loading Screen
    public GameObject loginErrorScreen;
    public GameObject usernameErrorScreen;
    public GameObject timeoutErrorScreen;

    [Header("Account Display")]
    public TextMeshProUGUI playerDisplay;
    public TextMeshProUGUI upgradeDisplay1;
    public TextMeshProUGUI upgradeDisplay2;
    public TextMeshProUGUI upgradeDisplay3;
    public TextMeshProUGUI scrapDisplay;

    //Call The Couroutine to Log In (Method Used by Unity Button)
    public void CallLogin()
    {
        StartCoroutine(LoginPlayer());
    }

    //Updating the Account Screen with the correct Username
    public void UpdateAccount()
    {
        if (PlayerData.LoggedIn)
        {
            playerDisplay.text = "Player: " + PlayerData.username;
            upgradeDisplay1.text = "Upgrade 1: " + PlayerData.upgrade1;
            upgradeDisplay2.text = "Upgrade 2: " + PlayerData.upgrade2;
            upgradeDisplay3.text = "Upgrade 3: " + PlayerData.upgrade3;
            scrapDisplay.text = "Scrap: " + PlayerData.scrap;

        }
        else
        {
            playerDisplay.text = "Not Logged In";
            return;
        }

    }

    //Close the login window if it is active
    public void CompleteLogin()
    {
        if (startScreen.activeInHierarchy)
        {
            startScreen.SetActive(false);
            lobbyScreen.SetActive(true);
            UpdateAccount(); //See UpdateAccount() Method
            NetworkManager.PhotonConnect();
        }
        else if (accountScreen.activeInHierarchy)
        {
            accountScreen.SetActive(false);
            startScreen.SetActive(true);
            PlayerData.LogOut();
            UpdateAccount();
            NetworkManager.PhotonDisconnect();
        }
    }

    //Coroutine that gets the login.php and does the following:
    //Check that the login is valid
    //Return player upgrades, scrap and nickname
    IEnumerator LoginPlayer() 
    {
        loadingScreen.SetActive(true);
        WWWForm form = new WWWForm();
        //important for php
        form.AddField("name", nameField.text);
        form.AddField("password", passwordField.text);

        UnityWebRequest www = UnityWebRequest.Post("https://projectdampdb.000webhostapp.com/login.php", form);

        // Send the request
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Web Connection Failed: " + www.error);

            loadingScreen.SetActive(false);

            if (www.error == "Request timeout")
            {
                timeoutErrorScreen.SetActive(true);
            }
        }
        else
        {
            string responseText = www.downloadHandler.text;
            if (responseText[0] == '0')
            {
                string[] data = responseText.Split('\t');
                PlayerData.username = nameField.text;
                //Read Values returned from the PHP Login Query and Save them in PlayerData C# Script
                PlayerData.upgrade1 = int.Parse(data[1]);
                PlayerData.upgrade2 = int.Parse(data[2]);
                PlayerData.upgrade3 = int.Parse(data[3]);
                PlayerData.scrap = int.Parse(data[4]);

                //Close the login window
                CompleteLogin();
            }
            else
            {
                Debug.Log("User Login Failed. Error #" + responseText);

                if (responseText == "6: Incorrect password")
                {
                    if (loginErrorScreen != null && !loginErrorScreen.activeInHierarchy)
                    {
                        loginErrorScreen.SetActive(true);
                    }
                }
                else if (responseText == "5: Either no user with name, or more than one")
                {
                    if (usernameErrorScreen != null && !usernameErrorScreen.activeInHierarchy)
                    {
                        usernameErrorScreen.SetActive(true);
                    }
                }

                loadingScreen.SetActive(false);
            }
        }
    }

    //If the inputs are above a certain character length, allow submission
    public void VerifyInputs()
    {
        submitButton.interactable = (nameField.text.Length >= usernameInputLength && passwordField.text.Length >= passwordInputLength);
    }
}

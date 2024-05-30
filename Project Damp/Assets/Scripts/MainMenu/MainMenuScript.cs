using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] GameObject[] menuScreens;
    [SerializeField] GameObject errorScreen;
    [SerializeField] GameObject startingScreen;

    private void Start()
    {
        if (PlayerData.LoggedIn)
        {
            startingScreen.SetActive(false);
            menuScreens[0].SetActive(true);
            LogIn loginScript = FindFirstObjectByType<LogIn>();
            loginScript.UpdateAccount();
        }
    }


    public void MenuButton(int index)
    {
        if (menuScreens[index] == null)
        {
            Debug.Log("Error in finding Menu");
        }
        else
        {
            //Error Message if player tries to open menus or start the game without logging in
            if (startingScreen.activeInHierarchy)
            {
                errorScreen.SetActive(true);
                return;
            }

            //Close down any opened menu tabs when opening another
            for (int i = 0; i < menuScreens.Length; i++)
            {
                if (menuScreens[i].activeInHierarchy)
                {
                    menuScreens[i].SetActive(false);
                }
            }
            
            //Show Menu
            menuScreens[index].gameObject.SetActive(true);
        }        
    }

    public void MenuBackButton(int index)
    {
        if (menuScreens[index] == null)
        {
            Debug.Log("Error in finding Menu");
        }
        else
        {

            menuScreens[index].gameObject.SetActive(false);

        }
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}

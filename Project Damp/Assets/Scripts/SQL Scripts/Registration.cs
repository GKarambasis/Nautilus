using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class Registration : MonoBehaviour
{
    [Header("Minimum Input Length for Username and Password")]
    public int usernameInputLength = 4;
    public int passwordInputLength = 8;

    //Input Fields
    [Header("GameObject Assignation")]
    public TMP_InputField nameField;
    public TMP_InputField passwordField;

    public Button submitButton;

    public GameObject window1;
    public GameObject window2;

    public void CompleteRegistration()
    {
        if (window1.activeInHierarchy)
        {
            window1.SetActive(false);
            window2.SetActive(true);
        }
        else if (window2.activeInHierarchy)
        {
            window2.SetActive(false);
            window1.SetActive(true);
        }
    }

    //Method Called by Button
    public void CallRegister()
    {
        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        //important for php
        form.AddField("name", nameField.text);
        form.AddField("password", passwordField.text);

        UnityWebRequest www = UnityWebRequest.Post("https://projectdampdb.000webhostapp.com/registernew.php", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Web Connection Failed: " + www.error);
        }
        else
        {
            string responseText = www.downloadHandler.text;
            if (responseText == "0")
            {
                Debug.Log("User created Successfully.");
                CompleteRegistration();
            }
            else
            {
                Debug.Log("User creation failed. Error#" + responseText);
            }
        }
    }

    public void VerifyInputs()
    {
        //If the inputs are above a certain character length, allow submission
        submitButton.interactable = (nameField.text.Length >= usernameInputLength && passwordField.text.Length >= passwordInputLength);
    }

}

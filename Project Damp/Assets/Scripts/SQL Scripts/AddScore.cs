using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class AddScore : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    public void CallSaveData() //Called by the Save Button
    {
        StartCoroutine(SavePlayerData());
    }

    IEnumerator SavePlayerData()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        WWWForm form = new WWWForm();
        form.AddField("name", PlayerData.username);
        form.AddField("upgrade1", PlayerData.upgrade1);
        form.AddField("upgrade2", PlayerData.upgrade2);
        form.AddField("upgrade3", PlayerData.upgrade3);
        form.AddField("scrap", PlayerData.scrap);

        UnityWebRequest www = UnityWebRequest.Post("https://projectdampdb.000webhostapp.com/savedata.php", form);

        // Send the request
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Web Connection Failed: " + www.error);
            if (loadingScreen != null)
            {
                loadingScreen.SetActive(false);
            }
        }
        else
        {
            string responseText = www.downloadHandler.text;
            if (responseText[0] == '0')
            {
                Debug.Log("Data Saved");
                if (loadingScreen != null)
                {
                    loadingScreen.SetActive(false);
                }
            }
            else
            {
                Debug.Log("User Data Save Failed. Error #" + responseText);
                if (loadingScreen != null)
                {
                    loadingScreen.SetActive(false);
                }
            }
        }


    }

    //Debug Button Methods
    public void AddScrap50()
    {
        PlayerData.scrap += 50;
    }
    public void AddUpgrade1()
    {
        PlayerData.upgrade1 += 1;
    }
    public void AddUpgrade2()
    {
        PlayerData.upgrade2 += 1;
    }
    public void AddUpgrade3()
    {
        PlayerData.upgrade3 += 1;
    }
}

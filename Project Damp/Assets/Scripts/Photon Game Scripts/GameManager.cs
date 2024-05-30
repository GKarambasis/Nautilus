using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Xml;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int moveSpeed = 10;
    public int rspeed = 10;

    public GameObject submarinePrefab; //prefab to be instantiated
    public Transform[] spawnPoints;
    [Header("HUD Assignation")]
    public GameObject loadingScreen; //loading screen while data is called
    public GameObject gameOverScreen, victoryScreen;
    [Header("Runtime Variables (No need to Assign)")]
    public GameObject teamSubmarine;
    public GameObject localPlayer, teamPlayer;

    private Rigidbody2D rb;
    private Animator animator;

    private Camera myCamera;
    private AddScore addScore;

    [SerializeField]
    Dictionary<int, List<string>> groupedPlayers = new Dictionary<int, List<string>>(); // Create a new dictionary to store players grouped by team number


    // Start is called before the first frame update
    void Start()
    {
        addScore = gameObject.GetComponent<AddScore>();
        
        //get the bulletpool 
        if (PhotonNetwork.IsMasterClient)
        {
            BulletPool.Instance.InitializePool();
        }

        myCamera = Camera.main;

        if (!loadingScreen.activeInHierarchy)
        {
            loadingScreen.SetActive(true);
        }

        CreateSubmarines();

    }

    //In Order to Create the Submarines, we first make a list of Player IDs (ActorNumbers) and their Teams
    //after that we convert this dictionary into a new dictionary where the key is the team and the value is the list of player IDs in the team
    public void CreateSubmarines()
    {
        Dictionary<string, int> playerTeamDict = new Dictionary<string, int>(); //dictionary to get all the player nicknames and team number

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            playerTeamDict.Add(PhotonNetwork.PlayerList[i].ActorNumber.ToString(), (int)PhotonNetwork.PlayerList[i].CustomProperties["TEAM"]); //Get the custom properties of every player indicating their team and name

            //Debug.Log(playerTeamDict[PhotonNetwork.NickName]);
        }



        // Iterate through the original dictionary
        foreach (KeyValuePair<string, int> kvp in playerTeamDict)
        {
            // Check if the value already exists as a key in the groupedPlayers dictionary
            if (groupedPlayers.ContainsKey(kvp.Value))
            {
                // If the key exists, add the current key to the list of strings for that value
                groupedPlayers[kvp.Value].Add(kvp.Key);
            }
            else
            {
                // If the key doesn't exist, create a new list with the current key and add it to the groupedStrings dictionary
                List<string> newList = new List<string>();
                newList.Add(kvp.Key);
                groupedPlayers.Add(kvp.Value, newList);
            }
        }

        // Iterate through the groupedStrings dictionary and print the strings grouped by their values
        foreach (KeyValuePair<int, List<string>> kvp in groupedPlayers)
        {
            Debug.Log("Strings with value " + kvp.Key + ":");
            foreach (string str in kvp.Value)
            {
                Debug.Log(str);
            }
        }

        StartCoroutine(InstantiateSubmarine());

    }

    //this coroutine instantiates the submarines and assigns each player character variable 
    IEnumerator InstantiateSubmarine()
    {
        yield return new WaitForSeconds(1);
        int playerTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["TEAM"];

        int playerUpgrade = FetchPlayerUpgrade();


        foreach (KeyValuePair<int, List<string>> kvp in groupedPlayers) //find the kvp of the local player
        {
            if (kvp.Key == playerTeam) //make sure that the team of the player is equal to the kvp.key team
            {
                if (kvp.Value[0] == PhotonNetwork.LocalPlayer.ActorNumber.ToString()) //make sure that the player is the first in the list and have the submarine be instantiated by him
                {
                    Debug.Log("this one is a team leader, instantiating submarine");
                    teamSubmarine = PhotonNetwork.Instantiate(submarinePrefab.name, spawnPoints[playerTeam].position, Quaternion.identity); //instantiate the submarine

                    teamSubmarine.GetComponent<ShipController>().ChangeSubmarineTag(playerTeam);

                    //Load the upgrade if the player has one
                    if (playerUpgrade != -1)
                    {
                        teamSubmarine.GetComponent<Inventory>().LoadItem(playerUpgrade);
                    }

                    localPlayer = teamSubmarine.transform.GetChild(0).gameObject; //make variables of the 2 players in the submarine
                    teamPlayer = teamSubmarine.transform.GetChild(1).gameObject;

                    rb = localPlayer.GetComponent<Rigidbody2D>();
                    animator = localPlayer.GetComponent<Animator>();

                    //teamPlayer.GetPhotonView().TransferOwnership(int.Parse(kvp.Value[1]));
                    Debug.Log("1st Player successfully loaded data");
                    yield return new WaitForSeconds(3);

                    myCamera.transform.position = new Vector3(localPlayer.transform.position.x, localPlayer.transform.position.y, myCamera.transform.position.z);
                    myCamera.orthographicSize = 5;
                    myCamera.transform.parent = localPlayer.transform;

                    loadingScreen.SetActive(false);
                }
                else //Second Player Code
                {
                    yield return new WaitForSeconds(3); // wait 3 seconds to ensure that the sub will be instantiated
                    teamSubmarine = GameObject.FindWithTag("Sub" + playerTeam.ToString()); //find the submarine

                    localPlayer = teamSubmarine.transform.GetChild(1).gameObject; //make variables of the 2 players in the submarine
                    teamPlayer = teamSubmarine.transform.GetChild(0).gameObject;


                    localPlayer.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer); //assign the photon view of player 2 to player 2
                    Debug.Log("2nd Player Owner: " + localPlayer.GetPhotonView().Owner);

                    rb = localPlayer.GetComponent<Rigidbody2D>();
                    animator = localPlayer.GetComponent<Animator>();

                    yield return new WaitForSeconds(1);

                    if (playerUpgrade != -1)
                    {
                        teamSubmarine.GetComponent<Inventory>().LoadItem(playerUpgrade);
                    }

                    myCamera.transform.position = new Vector3(localPlayer.transform.position.x, localPlayer.transform.position.y, myCamera.transform.position.z);
                    myCamera.orthographicSize = 5;
                    myCamera.transform.parent = localPlayer.transform;

                    loadingScreen.SetActive(false);
                }
            }
            else
            {
                Debug.Log("didn't find the thing");
            }
        }

    }

    private int FetchPlayerUpgrade()
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("UPGRADE"))
        {
            return -1;
        }

        return (int)PhotonNetwork.LocalPlayer.CustomProperties["UPGRADE"];

    }
    public void VictoryScreen()
    {
        if (victoryScreen != null)
        {
            SaveUpgrades();
            victoryScreen.SetActive(true);
        }
        else
        {
            Debug.LogWarning("You have not assigned the victory screen");
        }
    }
    public void SaveUpgrades()
    {
        Inventory inventoryScript = teamSubmarine.GetComponent<Inventory>();

        for (int i = 0; i < inventoryScript.inventorySlotItems.Length; i++)
        {
            if (inventoryScript.inventorySlotItems[i] != null)
            {
                switch(inventoryScript.inventorySlotItems[i].GetComponent<InventoryItem>().itemID) 
                {
                    case 2:
                        PlayerData.scrap += inventoryScript.inventorySlotItems[i].GetComponent<InventoryItem>().Value;
                        break;
                    case 3:
                        PlayerData.upgrade1 += 1;
                        break;
                    case 4:
                        PlayerData.upgrade2 += 1;
                        break;
                    case 5:
                        PlayerData.upgrade3 += 1;
                        break;
                    default:
                        Debug.Log("This Upgrade can't be saved");
                        break;
                }
            }
        }

        addScore.CallSaveData();
    }

    public void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        Debug.Log("GameOver Screen is Now Active");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        foreach (PhotonView photonView in PhotonNetwork.PhotonViewCollection) //cycle through all photonviews
        {
            if (photonView.Owner == PhotonNetwork.LocalPlayer) //cycle through the photonview that belongs to this player
            {
                if (photonView.gameObject.tag == "Player") //if the owned photon view's gameobject has tag player
                {
                    if (photonView.gameObject != localPlayer) //and it is not the localplayer
                    {
                        PhotonNetwork.Destroy(photonView); //destroy the other player's avatar
                    }
                }

            }
        }
    }
    
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        SceneManager.LoadScene(0);
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
    }

    // Called when the master client is switched
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // Transfer PhotonViews to the new master client
        TransferPhotonViews(newMasterClient, PhotonNetwork.MasterClient);

        //get the bulletpool 
        if (PhotonNetwork.IsMasterClient)
        {
            BulletPool.Instance.InitializePool();
        }

    }

    // Transfer PhotonViews from the disconnected master client to the new master client
    private void TransferPhotonViews(Player newMasterClient, Player oldMaster)
    {
        foreach (PhotonView photonView in PhotonNetwork.PhotonViewCollection)
        {
            if (photonView.Owner == null || photonView.Owner == oldMaster)
            {
                photonView.TransferOwnership(newMasterClient);
                Debug.Log("Transfered " + photonView.gameObject.name + " from " + oldMaster.NickName + " to " + photonView.Owner.NickName);

            }
        }
    }

}

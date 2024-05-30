using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Game Version")]
    [SerializeField]
    static string gameVersion = "0.9";

    public GameObject loadingScreen; //Loading Screen GameObject to enable when Loading Data

    [SerializeField]
    GameObject mainMenuCanvas; //Main Menu Canvas to Enable when player is in Main Menu

    public GameObject roomCanvas; //Different Canvas for When Player joins a lobby
    public TextMeshProUGUI roomNameText;


    private void Awake()
    {
        //sets scene to automatic
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    public static void PhotonConnect()
    {
        //Are we connected?
        if (!PhotonNetwork.IsConnected)
        {
            //Track Game Version
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = gameVersion;
            //Establish Connection
            PhotonNetwork.ConnectUsingSettings();
            //Give Nickname
            PhotonNetwork.NickName = PlayerData.username;
            Debug.Log("Logged in as: " +  PhotonNetwork.NickName);
        }
        else
        {
            Debug.Log("Already Connected as" + PhotonNetwork.NickName);
        }
    }
    public static void PhotonDisconnect()
    {
        //Are we connected?
        if (PhotonNetwork.IsConnected)
        {
            //Disconnect
            PhotonNetwork.Disconnect();
            Debug.Log("Successful Disconnection");
        }
        else
        {
            Debug.Log("Not Connected");
        }
    }


    //disconnect and provide server info
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + cause.ToString() + "Server Address: " + PhotonNetwork.ServerAddress);
        
    }
    
    //say that we connected and say what region + join the lobby
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        Debug.Log("Connection made to " + PhotonNetwork.CloudRegion + " Server.");
        Debug.Log("My NIckname is " + PhotonNetwork.LocalPlayer.NickName, this);

        if (loadingScreen.activeInHierarchy)
        {
            loadingScreen.SetActive(false); //Deactivates Loading Screen when Connection to Master Server is accomplished
        }
        
        if (!PhotonNetwork.InLobby)
        {
            //after connecting to server join lobby
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
    }

    //When A player joins a Room, We want to switch between canvases
    public override void OnJoinedRoom()
    {

        mainMenuCanvas.SetActive(false); 
        roomCanvas.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }

    //When leaving a room Switch Canvases Again
    public override void OnLeftRoom()
    {
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
        }
        if (roomCanvas != null)
        {
            roomCanvas.SetActive(false);

        }
    }


}

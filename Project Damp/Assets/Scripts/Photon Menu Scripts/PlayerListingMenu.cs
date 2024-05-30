using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [Header("Content Transform")]
    [SerializeField]
    private Transform _content;
    
    [Header("Player Listing Variables")]
    [SerializeField]
    private PlayerListing _playerListing;
    private List<PlayerListing> _listings = new List<PlayerListing>();
    [SerializeField]
    private PlayerTeamManager teamManager;


    [Header("AddScore Script")]
    AddScore addScore;

    [Header("Loading Screens")]
    [SerializeField] GameObject playerErrorScreen;
    [SerializeField] GameObject playerNotMasterScreen;
    [SerializeField] GameObject loadingScreen;

    private void Awake()
    {
        addScore = GetComponent<AddScore>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        GetCurrentRoomPlayers();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        for(int i = 0; i < _listings.Count; i++)
        {
            Destroy(_listings[i].gameObject);
        }

        _listings.Clear();

        //Wipe the List
        for (int i = 0; i < teamManager.teamsize.Length; i++)
        {
            teamManager.teamsize[i] = 0;
        }
    }

    private void GetCurrentRoomPlayers()
    {
       if (!PhotonNetwork.IsConnected)
       {
            Debug.Log("You are not Connected");
            return;
       }
       if(PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
       {
            Debug.Log("Cannot Find Current Room or Current Room Players");
            return;
       }
       foreach(KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players)
       {
            AddPlayerListing(playerInfo.Value);
       }
    }

    private void AddPlayerListing(Player player)
    {
        int index = _listings.FindIndex(x => x.Player == player);
        if (index != -1)
        {
            _listings[index].SetPlayerInfo(player);
        }
        else
        {
            PlayerListing listing = Instantiate(_playerListing, _content);
            if (listing != null)
            {
                listing.SetPlayerInfo(player);
                _listings.Add(listing);
            }
        }
    }

    //When a Player Joins the Room add their name to the player listing
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerListing(newPlayer);
    }

    //When a player leaves the room remove their name from the player listing
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //ensures that Players are deleted upon leaving
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    //Start Game Button, Does not Let Anyone But the Master Client to Start the Game
    public void OnClick_StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartGame());
        }
        else
        {
            Debug.Log("You are not the Master Client");
            //pop-up error Screen
            if (playerNotMasterScreen != null && !playerNotMasterScreen.activeInHierarchy) { playerNotMasterScreen.SetActive(true); }
        }
    }

    IEnumerator StartGame()
    {
        KickPlayers();
        
        yield return new WaitForSeconds(2);

        photonView.RPC("SavePlayerData", RpcTarget.All);

        yield return new WaitForSeconds(2);

        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
    }

    public void KickPlayers()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        //for every team look for the one that has 1 player
        for (int i = 0; i < teamManager.teamsize.Length; i++)
        {
            //if the size of the team is 1 and the player belongs in that team
            if (teamManager.teamsize[i] == 1)
            {
                //if the player belongs in that team
                if ((int)PhotonNetwork.LocalPlayer.CustomProperties["TEAM"] == i)
                {
                    if (loadingScreen != null)
                    {
                        loadingScreen.SetActive(false);
                    }

                    OnClick_LeaveRoom();
                    teamManager.OnClick_LeaveRoomAndTeam();

                    //pop-up error Screen
                    if(playerErrorScreen!= null && !playerErrorScreen.activeInHierarchy) { playerErrorScreen.SetActive(true); }

                    return;
                }
            }
        }

        photonView.RPC("ReceiveKickPlayers", RpcTarget.Others);
    }

    [PunRPC]
    void ReceiveKickPlayers()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        //for every team look for the one that has 1 player
        for (int i = 0; i < teamManager.teamsize.Length; i++)
        {
            //if the size of the team is 1 and the player belongs in that team
            if (teamManager.teamsize[i] == 1)
            {
                //if the player belongs in that team
                if ((int)PhotonNetwork.LocalPlayer.CustomProperties["TEAM"] == i)
                {
                    if (loadingScreen != null)
                    {
                        loadingScreen.SetActive(false);
                    }

                    OnClick_LeaveRoom();
                    teamManager.OnClick_LeaveRoomAndTeam();

                    //pop-up error Screen
                    if (playerErrorScreen != null && !playerErrorScreen.activeInHierarchy) { playerErrorScreen.SetActive(true); }

                    return;
                }
            }
        }
    }

    [PunRPC]
    void SavePlayerData()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("UPGRADE"))
        {
            int upgrade = (int)PhotonNetwork.LocalPlayer.CustomProperties["UPGRADE"];
            switch (upgrade)
            {
                case 0:
                    PlayerData.upgrade1 -= 1; //Remove the Upgrade
                    addScore.CallSaveData(); //Update the List
                    break;
                case 1:
                    PlayerData.upgrade2 -= 1; //Remove the Upgrade
                    addScore.CallSaveData(); //Update the List
                    break;
                case 2:
                    PlayerData.upgrade3 -= 1; //Remove the Upgrade
                    addScore.CallSaveData(); //Update the List
                    break;
                default:
                    Debug.LogWarning("Didn't find the correct upgrade to remove");
                    break;
            }
        }
    }

    //Leave Room Button
    public void OnClick_LeaveRoom()
    {
        //Temporary, Should Remove if I use the HashTable More
        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
        PhotonNetwork.LeaveRoom(true);

    }
}

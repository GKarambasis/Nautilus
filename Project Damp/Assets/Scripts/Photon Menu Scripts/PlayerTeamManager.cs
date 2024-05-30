using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using System;
using System.Reflection;
using TMPro;
using Photon.Realtime;


public class PlayerTeamManager : MonoBehaviourPunCallbacks
{
    private ExitGames.Client.Photon.Hashtable _playerProperties = new ExitGames.Client.Photon.Hashtable();

    [SerializeField]
    public int[] teamsize = new int[4];

    [Header("All Available Room Listings")]
    [SerializeField] RoomListing[] roomListings;

    [SerializeField]
    private TextMeshProUGUI[] teamTexts;
    //check that the team doesn't have more than two players
    //add player to team
    //increase the amount of players in team
    //if player leaves, remove the number of members in the team 


    private void Update()
    {
        for (int i = 0; i < teamTexts.Length; i++)
        {
            teamTexts[i].text = teamsize[i].ToString() + " / 2";
        }

    }

    //Player clicks on button to join team (Buttons are labeled with an integer based on the team number)
    public void OnClick_JoinTeam(int teamNumber)
    {
        //If the team has less than two players
        if (teamsize[teamNumber] < 2)
        {
            //Enable Buttons
            roomListings[teamNumber].EnableButtons();
            
            //remove player from their previous team, if they are in one
            removeTeam();

            //update the number of players in the team the player is joining
            int teamsizeUpdate = teamsize[teamNumber] + 1;

            //call the Update Method to sync it with other players
            UpdateArray(teamNumber, teamsizeUpdate);

            //Set the player properties team
            SetTeam(teamNumber);
        }
        else
        {
            Debug.Log("Team is Full");
        }
        
    }

    public void OnClick_AddUpgrade(int upgrade) //add an upgrade upon clicking the upgrade button
    {
        _playerProperties["UPGRADE"] = upgrade; //store the upgrade on a hash table
        PhotonNetwork.SetPlayerCustomProperties(_playerProperties); //sync the hashtable over photon

        roomListings[(int)_playerProperties["TEAM"]].DisableButtons(); //disable all other buttons
        Debug.Log("Upgrade " + upgrade.ToString() + " Successfully Added!");
    }


    private void removeTeam()
    {
        if (_playerProperties.ContainsKey("TEAM"))
        {
            int oldTeam = (int)_playerProperties["TEAM"];

            roomListings[oldTeam].DisableButtons();

            int oldTeamSize = teamsize[oldTeam] - 1;

            UpdateArray(oldTeam, oldTeamSize);
        }
    }

    private void SetTeam(int team)
    {
        _playerProperties["TEAM"] = team;
        PlayerData.team = team;
        PhotonNetwork.SetPlayerCustomProperties(_playerProperties);
        
        //PhotonNetwork.LocalPlayer.CustomProperties = _playerProperties;
    }


    void Start()
    {
        // If this is the local player, initialize the array
        for (int i = 0; i < teamsize.Length; i++)
        {
            teamsize[i] = 0;
        }
    }

    
    // Update the array
    //when a player joins a team write a function that pings to everyone that they joined the team
    public void UpdateArray(int index, int value)
    {
        // Update the local array
        teamsize[index] = value;

        // Send array updates to other players via Photon RPC
        photonView.RPC("ReceiveArrayUpdates", RpcTarget.Others, index, value);
    }


    [PunRPC]
    void ReceiveArrayUpdates(int index, int value)
    {
        // Update the local array with received data
        teamsize[index] = value;
    }

    //On player entering the room Update already existing players in the room from the master client
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < teamsize.Length; i++)
            {
                UpdateArray(i, teamsize[i]);
            }
        }
    }

    //On Leave Room Remove Player from their team and clear their properties
    public void OnClick_LeaveRoomAndTeam()
    {
        removeTeam();
        _playerProperties.Clear();
    }

}

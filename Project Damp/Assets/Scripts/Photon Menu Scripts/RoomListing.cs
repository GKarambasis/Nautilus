using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

public class RoomListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI _roomNameText, _roomSizeText;

    [SerializeField]
    public RoomInfo RoomInfo { get; private set; }

    public Button[] upgradeButtons = new Button[3];
    //Enable the buttons when a player joins the team
    public void EnableButtons()
    {
        if (PlayerData.upgrade1 > 0)
        {
            upgradeButtons[0].interactable = true;
        }
        if(PlayerData.upgrade2 > 0)
        {
            upgradeButtons[1].interactable = true;
        }
        if (PlayerData.upgrade3 > 0)
        {
            upgradeButtons[2].interactable = true;
        }
    }
    //Disable the button when a player leaves a team
    public void DisableButtons()
    {
        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (upgradeButtons[i].interactable)
            {
                upgradeButtons[i].interactable = false;
            }
        }
    }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _roomNameText.text = roomInfo.Name;
        _roomSizeText.text = roomInfo.PlayerCount.ToString()+ " / " + roomInfo.MaxPlayers.ToString();
    }

    public void OnClick_JoinRoom()
    {
        PhotonNetwork.JoinRoom(_roomNameText.text);
        
    }
}

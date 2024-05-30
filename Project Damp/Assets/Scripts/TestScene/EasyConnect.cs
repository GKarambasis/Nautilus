using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EasyConnect : MonoBehaviourPunCallbacks
{
    public PhotonView submarinePrefab;
    public GameObject currentSubmarine;
    private GameObject currentPlayer;
    private GameObject secondPlayer;

    public float speed = 1f;
    public float rspeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient) 
        { 
            Debug.Log("Joined a room. and Created A submarine");
            currentSubmarine = PhotonNetwork.Instantiate(submarinePrefab.name, Vector3.zero, Quaternion.identity);
            //submarines.Add(currentSubmarine);
            currentPlayer = currentSubmarine.transform.GetChild(0).gameObject;
            secondPlayer = currentSubmarine.transform.GetChild(1).gameObject;
        }
        else
        {
            currentSubmarine = GameObject.Find("Sub2(Clone)");
            currentPlayer = currentSubmarine.transform.GetChild(1).gameObject;
        }

    }

    public void Update()
    {
        if (currentPlayer != null)
        {
                
            MovePlayer(currentPlayer);
            
        }
        if (currentSubmarine!= null)
        {
            MoveShip(currentSubmarine);
        }
    }

    public void MovePlayer(GameObject player)
    {
      
            if (Input.GetKey(KeyCode.A))
            {
                player.transform.Translate(-speed * Time.deltaTime, 0f, 0f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                player.transform.Translate(speed * Time.deltaTime, 0f, 0f);
            }

            if (Input.GetKey(KeyCode.W))
            {
                player.transform.Translate(0f, speed * Time.deltaTime, 0f);
            }

            if (Input.GetKey(KeyCode.S))
            {
                player.transform.Translate(0f, -speed * Time.deltaTime, 0f);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                player.transform.Rotate(0f, 0f, -rspeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.E))
            {
                player.transform.Rotate(0f, 0f, rspeed * Time.deltaTime);
            }
        
      
        
    }
    public void MoveShip(GameObject ship)
    {

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            ship.transform.Translate(-speed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            ship.transform.Translate(speed * Time.deltaTime, 0f, 0f);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            ship.transform.Translate(0f, speed * Time.deltaTime, 0f);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            ship.transform.Translate(0f, -speed * Time.deltaTime, 0f);
        }

        if (Input.GetKey(KeyCode.P))
        {
            ship.transform.Rotate(0f, 0f, -rspeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.O))
        {
            ship.transform.Rotate(0f, 0f, rspeed * Time.deltaTime);
        }



    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        TransferOwnershipToPlayer(newPlayer, secondPlayer.GetPhotonView());
    }

    // This function is called when the GameObject should change ownership
    public void TransferOwnershipToPlayer(Player player, PhotonView photonView)
    {
        // Check if the PhotonView component exists
        if (photonView != null)
        {
            // Get the PhotonView component
            PhotonView view = photonView;

            // Check if the PhotonView is owned by the specified player already
            if (view.Owner != player)
            {
                // Transfer ownership to the specified player
                view.TransferOwnership(player);
                Debug.Log("Ownership transferred to Player " + player);
            }
            else
            {
                Debug.Log("Object is already owned by Player " + player);
            }
        }
        else
        {
            Debug.LogError("PhotonView component is missing.");
        }
    }

}

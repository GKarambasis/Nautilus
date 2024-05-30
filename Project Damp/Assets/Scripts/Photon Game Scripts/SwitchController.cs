using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class SwitchController : MonoBehaviour
{
    private string captain;
    private bool playerIsInHelm = false;
    Player player1;
    GameObject square;
    private int counter = 0;
    [SerializeField] GameObject ship;
    
    private Camera myCamera;
    private GameObject promptText; //prompt to take control upon the player getting next to the console

    [SerializeField]
    float shipCameraSize = 20; 
    [SerializeField]
    float playerCameraSize = GameData.playerCameraSize;

    private void Start()
    {
        myCamera = Camera.main;
        promptText = transform.GetChild(0).gameObject;
    }



    // Update is called once per frame
    void Update()
    {
        if (playerIsInHelm) //if the player is controlling the ship
        {
            if (Input.GetKeyDown(KeyCode.F) && counter == 0) //if the counter is 0 and the player presses space
            {
                counter++;
                ship.GetPhotonView().TransferOwnership(player1);
                Debug.Log(player1);
                Debug.Log(ship.GetPhotonView().Owner);
                myCamera.transform.position = new Vector3(ship.transform.position.x, ship.transform.position.y, myCamera.transform.position.z);
                myCamera.orthographicSize = shipCameraSize;
                myCamera.transform.parent = ship.transform;
            }
            else if (counter == 1 && ship.GetPhotonView().IsMine) //if the counter is 1 and the player owns the photon view
            {
                square.GetComponent<PlayerController>().enabled = false;
                ship.GetComponent<ShipController>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("what the fuck are you doing you absolute motherfucker");
                    counter--;
                    ship.GetComponent<ShipController>().enabled = false;
                    square.GetComponent<PlayerController>().enabled = true;
                    myCamera.transform.position = new Vector3(square.transform.position.x, square.transform.position.y, myCamera.transform.position.z);
                    myCamera.orthographicSize= playerCameraSize;
                    myCamera.transform.parent = square.transform;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player1 == null && collision.tag == "Player")
        {
            square = collision.gameObject;
            Debug.Log(player1);
            player1 = square.GetPhotonView().Owner;
            Debug.Log(player1);

            Debug.Log("hello there");

            if (collision.gameObject.GetPhotonView().IsMine)
            {
                playerIsInHelm = true;
                promptText.SetActive(true);//enable text prompt
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (player1 == collision.gameObject.GetPhotonView().Owner) //Should probably make owner of ship photon view Null
            {
                player1 = null;
                playerIsInHelm = false;

            
                if (collision.gameObject.GetPhotonView().IsMine)
                {
                    promptText.SetActive(false);//disable text prompt
                }
            }
        }
    }


}

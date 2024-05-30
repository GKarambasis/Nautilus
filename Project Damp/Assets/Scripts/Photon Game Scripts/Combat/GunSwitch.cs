using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwitch : MonoBehaviour
{
    private string captain;
    private bool playerIsInHelm = false;
    Player player1;
    GameObject square;
    private int counter = 0;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject playerGunSprite;

    private Camera myCamera;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private float gunCameraSize = 20;
    [SerializeField] private float playerCameraSize = GameData.playerCameraSize;

    // Start is called before the first frame update
    void Start()
    {
        myCamera = Camera.main;
        
        if(cameraPosition == null)
        {
            cameraPosition = gameObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerIsInHelm)
        {
            if (Input.GetKeyDown(KeyCode.F) && counter == 0)
            {
                counter++;
                gun.GetPhotonView().TransferOwnership(player1);

                //Change Camera View
                myCamera.transform.position = new Vector3(cameraPosition.transform.position.x, cameraPosition.transform.position.y, myCamera.transform.position.z);
                myCamera.orthographicSize = gunCameraSize;
                myCamera.transform.parent = cameraPosition.transform;
            }
            else if (counter == 1 && gun.GetPhotonView().IsMine)
            {
                if (gun.GetComponent<GunControl>().enabled == false)
                {
                    Debug.Log("Gday Mate");
                    //Hide Player Sprite and show player on gun sprite
                    square.GetComponent<SpriteRenderer>().enabled = false;
                    playerGunSprite.GetComponent<SpriteRenderer>().enabled = true;

                    //Switch Controls
                    square.GetComponent<PlayerController>().enabled = false;
                    Debug.Log(square.GetComponent<PlayerController>().enabled);
                    gun.GetComponent<GunControl>().enabled = true;
                }
                if (Input.GetKeyDown(KeyCode.F))
                {
                    counter--;
                    //Show Player Sprite again
                    square.GetComponent<SpriteRenderer>().enabled = true;
                    playerGunSprite.GetComponent<SpriteRenderer>().enabled = false;

                    //Switch Controls
                    gun.GetComponent<GunControl>().enabled = false;
                    Debug.Log(square);
                    square.GetComponent<PlayerController>().enabled = true;

                    //Change Camera View
                    myCamera.transform.position = new Vector3(square.transform.position.x, square.transform.position.y, myCamera.transform.position.z);
                    myCamera.orthographicSize = playerCameraSize;
                    myCamera.transform.parent = square.transform;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player1 == null && collision.tag == "Player")
        {
            Debug.Log("you have entered the helm");
            square = collision.gameObject;
            player1 = square.GetPhotonView().Owner;

            if (collision.gameObject.GetPhotonView().IsMine)
            {
                playerIsInHelm = true;
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (player1 == collision.gameObject.GetPhotonView().Owner)
            {
                player1 = null;
                playerIsInHelm = false;
            }
        }
    }
}

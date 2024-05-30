using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShipController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private float speed = 30f;
    [SerializeField]
    private float rspeed = 20f;

    //Change submarine tag to the team tag and sync it with all players
    public void ChangeSubmarineTag(int team)
    {
        photonView.RPC("ReceiveChangeSubmarineTag", RpcTarget.All, team);
    }
    [PunRPC]
    void ReceiveChangeSubmarineTag(int team)
    {
        gameObject.tag = "Sub" + team.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        MoveShip(transform.gameObject);
    }

    public void MoveShip(GameObject ship)
    {
        if (ship.GetPhotonView().IsMine)
        {

            if (Input.GetKey(KeyCode.A))
            {
                ship.transform.Translate(-speed * Time.deltaTime, 0f, 0f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                ship.transform.Translate(speed * Time.deltaTime, 0f, 0f);
            }
            if (Input.GetKey(KeyCode.W))
            {
                ship.transform.Translate(0f, speed * Time.deltaTime, 0f);

            }
            if (Input.GetKey(KeyCode.S))
            {
                ship.transform.Translate(0f, -speed * Time.deltaTime, 0f);
            }
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Mouse0))
            {
                ship.transform.Rotate(0f, 0f, -rspeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Mouse1))
            {
                ship.transform.Rotate(0f, 0f, rspeed * Time.deltaTime);
            }

        }
    }
}


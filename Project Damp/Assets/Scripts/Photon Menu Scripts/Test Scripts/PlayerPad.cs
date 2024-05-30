using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerPad : MonoBehaviourPunCallbacks
{

    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            InputMovement();
        }
    }

    void InputMovement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + Vector3.up * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position - Vector3.up * speed * Time.deltaTime);
        }
    }
}

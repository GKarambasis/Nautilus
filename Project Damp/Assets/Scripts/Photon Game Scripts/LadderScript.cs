using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    [SerializeField] Transform topTransform, bottomTransform;
    bool IsOverlapping;
    private Collider2D playerCollision;

    [SerializeField] bool isUp;

    private void Update()
    {
        if (IsOverlapping)
        {
            //Debug.Log("You're in!");
            // Check if the collision is in a range around TopTransform
            if (isUp)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerCollision.transform.position = new Vector3(bottomTransform.position.x, bottomTransform.position.y, playerCollision.transform.position.z);
                    isUp = false;
                    Debug.Log("Jumped down");
                }
            }
            // Check if the collision is in a range around BottomTransform
            else if (!isUp)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerCollision.transform.position = new Vector3(topTransform.position.x, topTransform.position.y, playerCollision.transform.position.z);
                    isUp = true;
                    Debug.Log("Jumped up");
                }
            }
            else
            {
                Debug.Log("Error");
            }
        }    
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.gameObject.GetPhotonView().IsMine)
        {
            IsOverlapping = true;
            playerCollision = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.gameObject.GetPhotonView().IsMine)
        {
            IsOverlapping = false;
        }
    }


}
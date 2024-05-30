using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 7f;

    private Player localPlayer; //useless so far

    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] GameManager gameManager;

    private bool collidingLeft = false;
    private bool collidingRight = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        rb = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();

        localPlayer = gameObject.GetPhotonView().Owner;

    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager != null) //check that gamemanager is not null
        {
            if (gameManager.localPlayer != null) //check that local player variable in the gamemanager script is not null
            {
                if (gameObject.GetPhotonView().IsMine) //make sure that the photon view is owned by the local player
                {
                    MovePlayer(transform.GameObject());
                }
            }
        }
    }

    public void MovePlayer(GameObject player)
    {

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        //Debug.Log(horizontalInput);

        // Set the velocity of the Rigidbody2D to move the player horizontally
        //rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        //NEW MOVEMENT ALLOWS FOR TRANSLATION REGARDLESS OF THE PLAYER ROTATION
        // Calculate movement direction based on local axis
        Vector3 movement = new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0f, 0f);



        // Set the "Speed" parameter in the Animator controller to control the animation
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        if (horizontalInput > 0 && !animator.GetBool("isFacingRight") || horizontalInput < 0 && animator.GetBool("isFacingRight"))
        {
            animator.SetBool("isFacingRight", !animator.GetBool("isFacingRight"));
        }

        if((horizontalInput >= 0f && collidingRight) || (horizontalInput < 0f && collidingLeft))
        {
            return;
        }

        // Move the player based on local axis
        transform.Translate(movement);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "wallR")
        {
            collidingRight = true;
        }
        if(collision.gameObject.tag == "wallL")
        {
            collidingLeft = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "wallR")
        {
            collidingRight = false;
        }
        if (collision.gameObject.tag == "wallL")
        {
            collidingLeft = false;
        }
    }

}

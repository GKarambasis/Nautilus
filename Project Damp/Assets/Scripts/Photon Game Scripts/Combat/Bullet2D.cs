using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet2D : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(1, 50)]
    [SerializeField] private float speed = 10f;
    private PhotonView photonView;
    private Rigidbody2D rb;
    public ParticleSystem explosionVFX;
    private bool hasCollided = false;
    private SpriteRenderer bulletSprite;


    private void Awake()
    {
        bulletSprite = GetComponent<SpriteRenderer>();
        photonView = gameObject.GetPhotonView();
        if (photonView.IsMine)
        {
            photonView.RPC("SetInactive", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void SetInactive()
    {
        gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        hasCollided = false;
        bulletSprite.enabled = true;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void FixedUpdate()
    {
        if (!hasCollided) //ensure that it hasn't collided
        {
            rb.velocity = transform.right * speed;
        }
        else 
        {
            rb.velocity = new Vector2(0, 0);
        }
    }



    //[PunRPC]
    //public void RequestReturnBulletFromMaster(int bulletViewID)
    //{
    //    // Only the Master Client should handle bullet return requests
    //    if (!PhotonNetwork.IsMasterClient) return;

    //    // Find the bullet using the received ViewID
    //    PhotonView bulletView = PhotonView.Find(bulletViewID);
    //    if (bulletView != null)
    //    {
    //        GameObject bullet = bulletView.gameObject;

    //        // Return the bullet to the bullet pool
    //        BulletPool.Instance.ReturnBullet(bullet);

    //        // Synchronize the bullet's state with all clients
    //        bulletView.RPC("SetInactive", RpcTarget.AllBuffered);
    //    }
    //}

    public void OnCollisionEnter2D(Collision2D collision)
    {
        DisableProjectile();
        
        //Debug.Log("I collided with: " + collision.gameObject.name);
    }

    public void DisableProjectile()
    {
        if (!hasCollided)
        {
            StartCoroutine(DisableProjectileCO());
        }
    }

    IEnumerator DisableProjectileCO()
    {
        hasCollided = true;
        //Debug.Log("Disabling Projectile");
        bulletSprite.enabled = false;
        if(explosionVFX != null)
        {
            explosionVFX.Play();
            Debug.Log("I explode");
        }
        else { Debug.LogWarning("No Particle Assigned to Bullet"); }
        yield return new WaitForSeconds(1);
        if (photonView.IsMine)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
        gameObject.SetActive(false);
    }

}

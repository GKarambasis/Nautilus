using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class GunControl : MonoBehaviour
{
    [Header("Rotation Speed")]
    public float rspeed = 16f;

    [Header("Bullet Lifetime")]
    [SerializeField] private float bulletLife = 3f;
    [Header("Clamp Settings")]
    [SerializeField]
    float minRotation = -90f; // Minimum allowed rotation in degrees
    [SerializeField]
    float maxRotation = 90f; // Maximum allowed rotation in degrees
    private PhotonView photonView;

    [SerializeField] ParticleSystem gunBarrelVFX;
    [SerializeField] GameObject ship;
    private Ammunition ammoScript;
    [SerializeField] GameObject char1, char2;
    private Player player1, player2;

    [Header("Firing Modes and Cooldown")]
    [SerializeField] int mode = 1;
    [SerializeField] float cooldownTime = 3f;
    private bool cooldown = false;


    [SerializeField] private Transform firingPoint;

    [Header("Cooldown HUD Element")]
    private GameManager gameManager;
    private Slider cooldownSlider;

    private void Start()
    {
        ammoScript = ship.GetComponent<Ammunition>();
        this.enabled = false;
        

    }

    private void Awake()
    {
        photonView = gameObject.GetPhotonView();
        //Get the GameManager Script
        gameManager = FindFirstObjectByType<GameManager>();
        //Find the Cooldown element from the hierarchy
        if (GameObject.Find("Cooldown_Element") != null)
        {
            cooldownSlider = GameObject.Find("Cooldown_Element").GetComponent<Slider>();
        }
        else
        {
            Debug.LogWarning("Cannot find Cooldown Element GameObject");
        }

    }

    //When this method gets triggered, the StartCooldown begins, and it updates the slider for the Gun Cooldown HUD Element.
    public void UpdateAmmoSlider()
    {
        if (cooldownSlider != null)
        {
            cooldownSlider.maxValue = cooldownTime;
            cooldownSlider.value = cooldownTime;
            cooldown = true;
        }
    }

    //Gun Cooldown
    private void StartCoolDown()
    {
        if(cooldown == true)
        {
            cooldownSlider.value -= Time.deltaTime;
            if (cooldownSlider.value == cooldownSlider.minValue)
            {
                cooldown = false; 
            }
        }
    }

    void Update()
    {
        
        Movegun(transform.gameObject);
        ClampRotation();

        StartCoolDown();
        
        if (mode != 2)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                Debug.Log("Shot Weapon");
                if(ammoScript.GetAmmo() > 0 && !cooldown)
                {
                    UpdateAmmoSlider();
                
                    RequestBullet();
                
                    ammoScript.Fired();//reduce ammo

                    if (char1 != null && char2 != null)
                    {
                        player1 = char1.GetPhotonView().Owner;
                        player2 = char2.GetPhotonView().Owner;
                        Player otherPlayer = (PhotonNetwork.LocalPlayer == player1) ? player2 : player1;
                        photonView.RPC("InvokeFired", otherPlayer);// to synchronize with other player
                    }
                }
                else
                {
                    Debug.Log("you have no bullets, please reload");
                }
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                Debug.Log("Shot Shotgun");
                if (ammoScript.GetAmmo() > 0 && !cooldown)
                {
                    UpdateAmmoSlider();

                    RequestBullet();

                    ammoScript.Fired();
                    
                    if(char1 != null && char2 != null)
                    {
                        player1 = char1.GetPhotonView().Owner;
                        player2 = char2.GetPhotonView().Owner;
                        Player otherPlayer = (PhotonNetwork.LocalPlayer == player1) ? player2 : player1;
                        photonView.RPC("InvokeFired", otherPlayer);// to synchronize with other player
                    }
                }
                else
                {
                    Debug.Log("you have no bullets, please reload");
                }
            }
        }
    }

    public void Mode1()
    {
        cooldownTime = 3;
        mode = 1;
        Debug.Log("mode " + mode + " has been activate");
    }
    public void Mode2()
    {
        cooldownTime = 1;
        mode = 2;
        Debug.Log("mode " + mode + " has been activate");
    }
    public void Mode3()
    {
        cooldownTime = 3;
        mode = 3;
        Debug.Log("mode " + mode + " has been activate");
    }
    [PunRPC]
    public void InvokeFired()
    {
        ammoScript.Fired();
    }

    public void Movegun(GameObject gun)
    {
        if (gun.GetPhotonView().IsMine)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                gun.transform.Rotate(0f, 0f, rspeed * Time.deltaTime);
                Debug.Log(transform.rotation.z);
            }


            if (Input.GetKey(KeyCode.E))
            {
                gun.transform.Rotate(0f, 0f, -rspeed * Time.deltaTime);
                Debug.Log(transform.rotation.z);

            }
        }
    }

    //Method to restrict turret rotation
    void ClampRotation()
    {
        // Get the current local rotation
        Quaternion currentLocalRotation = transform.localRotation;

        // Convert the quaternion to euler angles
        Vector3 currentLocalEulerAngles = currentLocalRotation.eulerAngles;

        // Clamp the rotation around the Z-axis
        currentLocalEulerAngles.z = Mathf.Clamp(currentLocalEulerAngles.z, minRotation, maxRotation);

        // Convert the euler angles back to quaternion
        Quaternion clampedLocalRotation = Quaternion.Euler(currentLocalEulerAngles);

        // Apply the clamped local rotation
        transform.localRotation = clampedLocalRotation;
    }

    public void RequestBullet()
    {
        if (mode == 1 || mode == 2)
        {   // Send a message to the Master Client to request a bullet
            photonView.RPC("RequestBulletFromMaster", RpcTarget.MasterClient, firingPoint.position, firingPoint.rotation);
        }

        else if (mode == 3)
        {   //Send a message to the Master Client to request enough bullets for a shotgun spread
            photonView.RPC("RequestShotgunBulletsFromMaster", RpcTarget.MasterClient, firingPoint.position, firingPoint.rotation);
        }
    }

    //Request a bullet from the bullet pool
    [PunRPC]
    public void RequestBulletFromMaster(Vector3 position, Quaternion rotation)
    {
        // Only the Master Client should handle bullet requests
        if (!PhotonNetwork.IsMasterClient) return;

        // Get a bullet from the bullet pool
        GameObject bullet = BulletPool.Instance.GetBullet();

        // If a bullet was successfully retrieved from the pool
        if (bullet != null)
        {
            // Set the bullet's position and rotation
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;

            // Activate the bullet
            bullet.SetActive(true);
            
            //Play Audio and Spawn VFX
            if (gameObject.GetComponent<AudioSource>() != null)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }
            if(gunBarrelVFX != null)
            {
                gunBarrelVFX.Play(withChildren: true);
            }

            //Start Coroutine to disable bullet after X seconds
            StartCoroutine(ActivateRPCAfterDelay(bullet));
            
            // Synchronize the bullet's state with all clients
            photonView.RPC("SyncBulletState", RpcTarget.Others, bullet.GetPhotonView().ViewID, bullet.transform.position, bullet.transform.rotation);
        }
    }
    
    //Request 3 bullets from the bullet pool
    [PunRPC]
    public void RequestShotgunBulletsFromMaster(Vector3 position, Quaternion rotation)
    {
        // Only the Master Client should handle bullet requests
        if (!PhotonNetwork.IsMasterClient) return;

        // Get a bullet from the bullet pool
        GameObject bullet1 = BulletPool.Instance.GetBullet();
        GameObject bullet2 = BulletPool.Instance.GetBullet();
        GameObject bullet3 = BulletPool.Instance.GetBullet();

        // If a bullet was successfully retrieved from the pool
        if (bullet1 != null && bullet2 != null && bullet3 != null)
        {
            // Set the bullet's position and rotation
            bullet1.transform.position = position;
            bullet2.transform.position = position;
            bullet3.transform.position = position;
            bullet1.transform.rotation = rotation;
            bullet2.transform.rotation = rotation * Quaternion.Euler(0, 0, 45);
            bullet3.transform.rotation = rotation * Quaternion.Euler(0, 0, -45);

            // Activate the bullets
            bullet1.SetActive(true);
            bullet2.SetActive(true);
            bullet3.SetActive(true);

            //Play Audio and Spawn VFX
            if (gameObject.GetComponent<AudioSource>() != null)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }
            if (gunBarrelVFX != null)
            {
                gunBarrelVFX.Play(withChildren: true);
            }

            //Start Coroutine to disable bullet after X seconds
            StartCoroutine(ActivateRPCAfterDelay(bullet1));
            StartCoroutine(ActivateRPCAfterDelay(bullet2));
            StartCoroutine(ActivateRPCAfterDelay(bullet3));

            // Synchronize the bullet's state with all clients
            photonView.RPC("SyncBulletState", RpcTarget.Others, bullet1.GetPhotonView().ViewID, bullet1.transform.position, bullet1.transform.rotation);
            photonView.RPC("SyncBulletState", RpcTarget.Others, bullet2.GetPhotonView().ViewID, bullet2.transform.position, bullet2.transform.rotation);
            photonView.RPC("SyncBulletState", RpcTarget.Others, bullet3.GetPhotonView().ViewID, bullet3.transform.position, bullet3.transform.rotation);
        }
    }

    //Deactivates the bullet after bulletlife seconds
    IEnumerator ActivateRPCAfterDelay(GameObject bullet)
    {
        yield return new WaitForSeconds(bulletLife);
        RequestReturnBulletFromMasterFire(bullet.GetPhotonView().ViewID);
    }
    
    //Syncs the bullet state with all the other players
    [PunRPC]
    public void SyncBulletState(int bulletViewID, Vector3 position, Quaternion rotation)
    {

        PhotonView bulletView = PhotonView.Find(bulletViewID);

        // If a bullet was successfully retrieved from the pool
        if (bulletView != null)
        {
            GameObject bullet = bulletView.gameObject;
            // Set the bullet's position and rotation
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;

            // Activate the bullet
            bullet.SetActive(true);

            if(gameObject.GetComponent<AudioSource>() != null)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }
            if (gunBarrelVFX != null)
            {
                gunBarrelVFX.Play(withChildren: true);
            }
        }
    }

    
    public void RequestReturnBulletFromMasterFire(int bulletViewID)
    {
        // Find the bullet using the received ViewID
        GameObject bullet = PhotonView.Find(bulletViewID).gameObject;
        if (bullet != null && bullet.activeInHierarchy)
        {
            bullet.GetComponent<Bullet2D>().DisableProjectile();

            // Synchronize the bullet's state with all clients
            photonView.RPC("SyncBulletReturnStateFire", RpcTarget.Others, bulletViewID);
        }
    }

    [PunRPC]
    public void SyncBulletReturnStateFire(int bulletViewID)
    {
        // Find the bullet using the received ViewID
        GameObject bullet = PhotonView.Find(bulletViewID).gameObject;
        if (bullet != null)
        {

            bullet.GetComponent<Bullet2D>().DisableProjectile();

        }
    }
    
}

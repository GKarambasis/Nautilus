using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipHealth : MonoBehaviourPunCallbacks
{
    private GameManager gameManager;
    [SerializeField] private int health = 100, damage = 20, maxHealth = 100, healAmount = 20;
    [SerializeField] Slider healthSlider;
    [SerializeField] ParticleSystem deathExploisionVFX;
    [SerializeField] Extract extraction;
    private AudioSource subAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        extraction = FindFirstObjectByType<Extract>();
        gameManager = FindFirstObjectByType<GameManager>();

        subAudioSource = gameObject.GetComponent<AudioSource>();

        if (GameObject.Find("Health_Element") != null)
        {
            healthSlider = GameObject.Find("Health_Element").GetComponent<Slider>();
        }
        else
        {
            Debug.LogWarning("Cannot find Health Element GameObject");
        }
    }

    public void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            if(gameManager.teamSubmarine == gameObject) //Ensure that the local player is getting the update only for their own submarine
            {
                healthSlider.value = health;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            health -= damage;
            Debug.Log("health is at" + health);
            UpdateHealthBar();

            if (health <= 0)
            {
                DeactivateShip();
            }

        }
    }

    public void DeactivateShip()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(DeactivateShipCoroutine());
        }
    }

    [PunRPC]
    public void RpcGameOver()
    {
        if (gameManager != null)
        {
            gameManager.GameOverScreen();
        }
        else
        {
            Debug.Log("You're a fucking moron");
        }
    }

    public void InvokeWin()
    {
        photonView.RPC("Win", RpcTarget.All);
    }

    [PunRPC]
    public void Win()
    {
        if(extraction.getShip() == gameManager.teamSubmarine)
        {
            if(extraction.getShip().GetPhotonView().IsMine) 
            { 
                VictoryShip();
            }
            else if (gameManager.teamPlayer == null)
            {
                VictoryShip();
            }
        }
        else
        {
            gameManager.teamSubmarine.GetComponent<ShipHealth>().DeactivateShip(); //get the deactivation code for the local player running this
        }
    }
    public void VictoryShip()
    {
        Player owner = gameManager.localPlayer.GetPhotonView().Owner;
        Player other = gameManager.teamPlayer.GetPhotonView().Owner;

        gameManager.VictoryScreen();
        if (other != null)
        {
            photonView.RPC("InvokeVictoryScreen", other);
        }
        Debug.Log("Here goes nothing");
    }

    IEnumerator DeactivateShipCoroutine()
    {
        
        //Disable the ship's collider
        if(gameObject.GetComponent<PolygonCollider2D>() != null)
        {
            gameObject.GetComponent<PolygonCollider2D>().enabled = false;   
        }
        
        yield return new WaitForSeconds(0.1f);

        //play explosion for all players
        photonView.RPC("PlayExplosionParticle", RpcTarget.All);
        
        Inventory inventoryScript = gameObject.GetComponent<Inventory>();
        for(int i = 0; i < inventoryScript.inventorySlotItems.Length; i++) //For every item in the inventory remove it at instantiate it
        {
            if (inventoryScript.inventorySlotItems[i] != null) //if the item is not null
            {
                PhotonNetwork.Instantiate(inventoryScript.inventorySlotItems[i].name, gameObject.transform.position, Quaternion.identity); //spawns the item behind

                inventoryScript.RemoveItem(i); //removes the item in the slot
            }
        }

        yield return new WaitForSeconds(deathExploisionVFX.main.duration + 0.3f);

        gameManager.GameOverScreen();
        if(gameManager.localPlayer != null && gameManager.teamPlayer != null)
        {
            Player owner = gameManager.localPlayer.GetPhotonView().Owner;
            Player other = gameManager.teamPlayer.GetPhotonView().Owner;
            photonView.RPC("RpcGameOver", other);
        }

        PhotonNetwork.Destroy(gameObject);

        Debug.Log("Here goes nothing");
        //StartCoroutine(DeactivateShipCoroutine(other));
    }
    [PunRPC]
    public void InvokeVictoryScreen()
    {
        gameManager.VictoryScreen();
    }

    [PunRPC]
    public void PlayExplosionParticle()
    {
        if(deathExploisionVFX != null)
        {
            deathExploisionVFX.Play(withChildren: true);
            subAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("Death Explosion particle not assigned to ship");
        }
    }

    public void Heal()
    {
        if(health <= maxHealth-healAmount)
        {
            health += healAmount;
        }
        else
        {
            health = maxHealth;
        }
        UpdateHealthBar();
        Debug.Log("health is at" + health + " " + PhotonNetwork.LocalPlayer);
    }
}

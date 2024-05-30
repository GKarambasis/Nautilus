using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ammunition : MonoBehaviourPunCallbacks
{
    [SerializeField] int ammo = 10, maxAmmo = 100;

    [SerializeField] Slider ammoSlider;

    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

        if (GameObject.Find("Ammo_Element") != null)
        {
            ammoSlider = GameObject.Find("Ammo_Element").GetComponent<Slider>();
        }
        else
        {
            Debug.LogWarning("Cannot find Ammo Element GameObject");
        }
    }

    public void UpdateAmmoSlider()
    {
        if (ammoSlider != null)
        {
            if (gameManager.teamSubmarine == gameObject) //Ensure that the local player is getting the update only for their own submarine
            {
                ammoSlider.value = ammo;
            }
        }
    }


    public int GetAmmo()
    {
        return ammo;
    }
    public void SetAmmo(int newAmmo)
    {
        this.ammo = newAmmo;
    }

    public void Fired()
    {
        ammo--;
        UpdateAmmoSlider();
        Debug.Log("ammo is at " + this.ammo);
    }

    public void Reload()
    {
        this.ammo = maxAmmo;
        UpdateAmmoSlider();
        Debug.Log("Ammo is at " +  this.ammo); 
    }
    [PunRPC]
    public void InvokeReload()
    {
        Reload();
    }
}

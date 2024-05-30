using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ExtractionBeacon : MonoBehaviourPunCallbacks
{
    private Inventory inventoryScript;
    private TargetIndicator indicatorScript;
    bool hasExtractionBeacon;
    bool activePointer = false;
    private bool isRunning = false;

    private GameObject localPlayer1, localPlayer2;
    public GameObject getPlayer1()
    {
        return localPlayer1;
    }

    public GameObject getPlayer2() 
    { 
        return localPlayer2;
    }

    private void Start()
    {
        inventoryScript = GetComponent<Inventory>();
        indicatorScript = FindFirstObjectByType<TargetIndicator>();
        localPlayer1 = gameObject.transform.GetChild(0).gameObject;
        localPlayer2 = gameObject.transform.GetChild(1).gameObject;

        StartCoroutine(RunFunctionRepeatedly());

    }



    IEnumerator RunFunctionRepeatedly()
    {
        while (true) // Runs indefinitely
        {
            if (!isRunning)
            {
                isRunning = true;
                // Call Beacon Check
                BeaconCheck();
                yield return new WaitForSeconds(2f); // Adjust the delay as needed
                isRunning = false;
            }
            yield return null;
        }
    }

    public void BeaconCheck()
    {
        hasExtractionBeacon = false;
        
        for (int i = 0; i < inventoryScript.inventorySlotItems.Length; i++) 
        {
            if (inventoryScript.inventorySlotItems[i] != null)
            {
                if (inventoryScript.inventorySlotItems[i].GetComponent<InventoryItem>().Name == "ExtractionBeacon")
                {
                    //Ensure that none of the players have logged off. If either has, or both of them, modify the checks
                    if(localPlayer1 != null && localPlayer2 != null)
                    {
                        //Ensure that neither of the photonviews of the ship belong to me
                        if (!(localPlayer1.GetPhotonView().IsMine || localPlayer2.GetPhotonView().IsMine)) 
                        {
                            hasExtractionBeacon = true; 
                        }
                    }
                    else if (localPlayer1 == null)
                    {
                        if (!localPlayer1.GetPhotonView().IsMine)
                        {
                            hasExtractionBeacon = true;
                        }
                    }
                    else if (localPlayer2 == null)
                    {
                        if (!localPlayer2.GetPhotonView().IsMine)
                        {
                            hasExtractionBeacon = true;
                        }
                    }
                    else
                    {
                        hasExtractionBeacon = true;
                    }
                }
            }
        }
        //After the for loop if the extraction beacon is found on the ship, set the pointer to it active.
        if(hasExtractionBeacon)
        {
            indicatorScript.target = gameObject.transform;
            activePointer = true;
        }
        else if (!hasExtractionBeacon && activePointer)
        {
            indicatorScript.target = null;
            activePointer = false;
        }

    }

}

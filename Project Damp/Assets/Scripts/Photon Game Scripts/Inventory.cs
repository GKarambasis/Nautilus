using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using Unity.VisualScripting;
using System.Drawing;

public class Inventory : MonoBehaviourPunCallbacks
{

    [Header("Object Reference to the Inventory Slots")]
    public GameObject[] inventorySlots = new GameObject[5];
    [Header("Current Items In Each Inventory Slot")]
    public GameObject[] inventorySlotItems = new GameObject[5];//items currently in inventory
    [Header("Possible Inventory Items")]
    public GameObject[] inventoryItems;

    [Header("All weapon Upgrades that can be loaded from Menu")]
    [SerializeField]
    InventoryItem[] weaponsArray;
    [SerializeField] GunControl gunControlTop;
    [SerializeField] GunControl gunControlBottom;

    [Header("Other Variables")]
    public Transform ejectionLocation;
    [SerializeField] PhotonView ship, char1, char2;
    private Player player1, player2;
    public bool hasEntered = false;

    [Header("Extraction Variables")]
    public bool isInExtractionZone = false;
    [SerializeField] Extract extraction;

    private void Start()
    {
        extraction = FindFirstObjectByType<Extract>();
    }

    public void DestroyItem(int viewID)
    {
        photonView.RPC("InvokeDestroyItem", RpcTarget.All, viewID);

        hasEntered = false;
    }

    [PunRPC]
    public void InvokeDestroyItem(int viewID)
    {
        PhotonView targetPhotonView = PhotonNetwork.GetPhotonView(viewID); //get the photon view through the id

        if(targetPhotonView != null)
        {
            if (targetPhotonView.IsMine)
            {
                PhotonNetwork.Destroy(targetPhotonView.gameObject); //find the owner of the photon view and destroy it
            }
        }
        else
        {
            Debug.LogWarning("could not find the correct item PhotonView");
        }
    }

    public void LoadItem(int upgrade)
    {
        AddLoadedItem(weaponsArray[upgrade].Name);
    }

    //Method Called by InventoryItem Script
    public void AddLoadedItem(string item)
    {
        for (int i = 0; i < inventorySlotItems.Length; i++) //cycle through inventory slots and find an empty one
        {
            if (inventorySlotItems[i] == null)
            {
                for (int j = 0; j < inventoryItems.Length; j++) //cycle through inventory items
                {
                    if (inventoryItems[j].GetComponent<InventoryItem>().Name == item) //if the item picked up equals the item in the list add it to the slot
                    {
                        Debug.Log("Adding Item");
                        photonView.RPC("InvokeAddItem", RpcTarget.AllBuffered, i, j);

                        //Change the Slot Image and update it
                        photonView.RPC("InventorySlotUpgrade", RpcTarget.AllBuffered, i);


                        break;
                    }
                    else
                    {
                        Debug.Log("Error, Can't find the appropriate item to Add");
                    }
                }
                break;
            }
            else
            {
                Debug.LogError("Inventory Full, please Discard item");
                hasEntered = false;
            }
        }

    }

    //Method Called by InventoryItem Script
    public void AddItem(string item, int viewID)
    {
        for(int i = 0; i < inventorySlotItems.Length; i++) //cycle through inventory slots and find an empty one
        {
            if (inventorySlotItems[i] == null)
            {
                for(int j = 0; j < inventoryItems.Length; j++) //cycle through inventory items
                {
                    if (inventoryItems[j].GetComponent<InventoryItem>().Name == item) //if the item picked up equals the item in the list add it to the slot
                    {
                        Debug.Log("Adding Item");
                        photonView.RPC("InvokeAddItem", RpcTarget.AllBuffered, i, j);

                        //Change the Slot Image and update it
                        photonView.RPC("InventorySlotUpgrade", RpcTarget.AllBuffered, i);
                        
                        //Run an RPC to Destroy the Item
                        DestroyItem(viewID);


                        break;
                    }
                    else
                    {
                        Debug.Log("Error, Can't find the appropriate item to Add");
                        hasEntered = false;
                    }
                }
                break;
            }
            else
            {
                Debug.Log("Inventory Full, please Discard item");
                hasEntered = false;
            }
        }
        
    }
    //Method called on start, to add any item brought by a player from the Main Menu
    [PunRPC]
    public void InventorySlotUpgrade(int i)
    {
        inventorySlots[i].GetComponent<InventorySlot>().slotUpdate();
    }

    [PunRPC]
    public void InvokeAddItem(int i, int j)
    {
        Debug.Log("Item Added");
        inventorySlotItems[i] = inventoryItems[j];
    }

    public void RemoveItem(int slot)
    {
        photonView.RPC("InvokeRemoveItem", RpcTarget.All, slot); //invoke the InvokeRemoveItem method on all players
    }

    [PunRPC]
    public void InvokeRemoveItem(int slot)
    {
        if (photonView.IsMine) //If the photon view of the ship is mine run this
        {
            PhotonNetwork.Instantiate(inventorySlotItems[slot].gameObject.name, ejectionLocation.position, Quaternion.identity);
        }
        inventorySlotItems[slot] = null;
        inventorySlots[slot].GetComponent<InventorySlot>().slotUpdate();
    }

    public void UseItem(int slot)
    {
        photonView.RPC("InvokeUseItem", RpcTarget.AllBuffered, slot);
    }

    [PunRPC]
    public void InvokeUseItem(int slot)
    {
        player1 = char1.Owner;
        player2 = char2.Owner;
        switch (inventorySlotItems[slot].GetComponent<InventoryItem>().itemID)
        {
            case 0://health
                ship.GetComponent<ShipHealth>().Heal();
                inventorySlotItems[slot] = null;
                inventorySlots[slot].GetComponent<InventorySlot>().slotUpdate();
                break;
            case 1://ammo
                ship.GetComponent<Ammunition>().Reload();
                inventorySlotItems[slot] = null;
                inventorySlots[slot].GetComponent<InventorySlot>().slotUpdate();
                break; 
            case 2://scrap
                Debug.Log("Scrap Cannot be Used");
                break;
            case 3://weapon 1
                gunControlTop.Mode1();
                gunControlBottom.Mode1();
                inventorySlotItems[slot] = null;
                inventorySlots[slot].GetComponent<InventorySlot>().slotUpdate();
                break;
            case 4://weapon 2
                gunControlTop.Mode2();
                gunControlBottom.Mode2();
                inventorySlotItems[slot] = null;
                inventorySlots[slot].GetComponent<InventorySlot>().slotUpdate();
                break;
            case 5://weapon 3
                gunControlTop.Mode3();
                gunControlBottom.Mode3();
                inventorySlotItems[slot] = null;
                inventorySlots[slot].GetComponent<InventorySlot>().slotUpdate();
                break;
            case 6://Extraction
                if(isInExtractionZone) //check if they are in area
                {
                    if (extraction.GetPart() == 0)
                    {
                        extraction.beginExtraction(gameObject); //set begun to true
                    }
                    else
                    {
                        Debug.LogWarning("Extraction Already Activated");
                    }
                }
                else
                {
                    Debug.Log("Can't use Extraction Item outside the Extraction zone!");
                }
                break;
            default:
                Debug.LogWarning("Error: No Item Found");
                inventorySlotItems[slot] = null;
                inventorySlots[slot].GetComponent<InventorySlot>().slotUpdate();
                break;


        }
        
    }

}

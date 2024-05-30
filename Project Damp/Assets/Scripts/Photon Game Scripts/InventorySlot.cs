using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InventorySlot : MonoBehaviourPunCallbacks
{

    [Header("Slot Parameters")]
    public int slotID;
    public int slotItem = -1;
    [SerializeField] public SpriteRenderer slotSprite;
    [SerializeField] GameObject promptText;

    [Header("Submarine Inventory Script")]
    [SerializeField] Inventory inventoryScript;

    bool playerInSlot = false;

    private void Update()
    {
       if (playerInSlot)
       {
            if (Input.GetKeyDown(KeyCode.E)) //Remove Item
            {
                inventoryScript.RemoveItem(slotID);
            }
        
            else if(Input.GetKeyDown(KeyCode.Space)) //Use Item
            {
                Debug.Log("i have pressed the button");
                inventoryScript.UseItem(slotID);
            }
       }
    }

    public void slotUpdate()
    {
        if (inventoryScript.inventorySlotItems[slotID] != null) //if the inventory slot has an item, show it to all players
        {
            slotSprite.gameObject.SetActive(true);
            
            Debug.Log("Slot Updated, Item Added");
            slotItem = inventoryScript.inventorySlotItems[slotID].GetComponent<InventoryItem>().itemID; //set the item id of the slot to the item id of the item

            slotSprite.sprite = inventoryScript.inventorySlotItems[slotID].GetComponent<InventoryItem>().itemImage; //Set the sprite of the sprite renderer to the correct object
            
        }
        else //If the slot is empty delete
        {
            slotSprite.gameObject.SetActive(false);

            slotItem = -1;

            Debug.Log("Slot Updated, Item Removed");
        }
    }
    

    private void OnTriggerEnter2D(Collider2D collision) // when a player collides with this object
    {
        if (collision.tag == "Player" && collision.gameObject.GetPhotonView().IsMine && slotItem != -1)
        {
            promptText.SetActive(true); //set the prompt text active
            Debug.Log("player has entered the area");

            playerInSlot = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && collision.gameObject.GetPhotonView().IsMine)
        {
            promptText.SetActive(false); //disable prompt text

            playerInSlot = false;
        }
    }
}

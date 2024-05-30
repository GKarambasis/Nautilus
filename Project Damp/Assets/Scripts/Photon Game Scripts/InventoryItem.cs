using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class InventoryItem : MonoBehaviour
{
    public string Name; //Name of the Object
    [Header("Can Be Consumable, Scrap or Equipment")]
    public string Type; //Type of Inventory Item 
    [Header("Consumable or Scrap Only")]
    public int Value; 
    [Header("Equipment Only")]
    public GameObject ProjectileType;

    [Header("Inventory Display")]
    public Sprite itemImage;

    [Header("Unique for Every Item")]
    public int itemID;

    //When the Sub Collides with a Loot Item add the Item to the list
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Sub"))
        {
            Inventory inventoryScript = collision.GetComponent<Inventory>();
            if (!inventoryScript.hasEntered)
            {
                inventoryScript.hasEntered = true;

                Debug.Log("Check Collision");
                inventoryScript.AddItem(Name, gameObject.GetPhotonView().ViewID); //add item to inventory by passing its item type (name) and its viewID
            }
            else
            {
                Debug.Log("Correct Tag but is already colliding with something");
            }
        }
        else
        {
            Debug.Log("Collision does not detect loot tag");
        }
    }
}

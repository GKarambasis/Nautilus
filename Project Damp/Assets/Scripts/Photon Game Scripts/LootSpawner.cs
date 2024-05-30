using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    public GameObject[] lootPrefabs;
    [SerializeField] GameObject extractionBeaconPrefab;
    [SerializeField]
    private List<Transform> lootSpawns = new List<Transform>();
    
    void Awake()
    {
        Transform parentTransform = gameObject.transform;

        // Loop through each child of the parent GameObject
        foreach (Transform child in parentTransform)
        {
            // Add the child's transform to the list
            lootSpawns.Add(child);
        }
    }
    private void Start()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            InstantiateLoot();
        }
    }
    
    private void InstantiateLoot()
    {
        for (int i = 0; i < lootSpawns.Count; i++)
        {
            List<Transform> innerList = new List<Transform>();

            foreach (Transform child in lootSpawns[i])
            {
                innerList.Add(child);
            }

            // Ensure the inner list has at least 3 elements
            if (innerList.Count < 3)
            {
                Console.WriteLine($"List at index {i} does not have enough elements.");
                continue;
            }

            // Generate three distinct random indexes within the range of the inner list
            System.Random rand = new System.Random();
            int index1 = rand.Next(0, innerList.Count);
            int index2 = rand.Next(0, innerList.Count - 1);
            int index3 = rand.Next(0, innerList.Count - 2);
            if (index2 >= index1)
            {
                index2++;
            }
            if (index3 >= index1)
            {
                index3++;
            }
            if (index3 >= index2)
            {
                index3++;
            }

            List<int> indexes = new List<int>();
            indexes.Add(index1);
            indexes.Add(index2);
            indexes.Add(index3);

            for (int x = 0; x < indexes.Count; x++)
            {
                
                PhotonNetwork.Instantiate(lootPrefabs[UnityEngine.Random.Range(0, lootPrefabs.Length)].name, innerList[indexes[x]].position, Quaternion.identity);
            }
            

        }
        SpawnExtractionBeacon();
    }

    void SpawnExtractionBeacon()
    {
        //Find all loot items
        GameObject[] lootItems = GameObject.FindGameObjectsWithTag("loot");
        //select a random item from the array
        GameObject itemToReplace = lootItems[UnityEngine.Random.Range(0, lootItems.Length)];

        //instantiate beacon in that items position, and remove the item.
        PhotonNetwork.Instantiate(extractionBeaconPrefab.name, itemToReplace.transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(itemToReplace);
    }
}

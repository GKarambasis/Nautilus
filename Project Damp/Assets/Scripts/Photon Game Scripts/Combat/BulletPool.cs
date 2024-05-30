using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; } // Singleton instance

    [SerializeField] private GameObject bulletPrefab; // The bullet prefab
    [SerializeField] private int size = 10; // The size of the pool

    private Queue<GameObject> bullets; // The bullet pool

    private void Awake()
    {
        // Set up the singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    public void InitializePool()
    {
        bullets = new Queue<GameObject>(size);
        Debug.Log("generating bullets");
        for (int i = 0; i < size; i++)
        {
            GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, Vector3.zero, Quaternion.identity);
            bullets.Enqueue(bullet);
        }
    }




    public GameObject GetBullet()
    {
        if (bullets.Count == 0) 
        {
            return PhotonNetwork.Instantiate(bulletPrefab.name, Vector3.zero, Quaternion.identity);
        } 

        GameObject bullet = bullets.Dequeue();
        bullet.SetActive(true);
        return bullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullets.Enqueue(bullet);
    }






}

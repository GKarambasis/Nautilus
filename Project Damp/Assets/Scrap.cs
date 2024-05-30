using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    [SerializeField] int scrap;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getScrap()
    {
        return scrap;
    }

    public void setScrap(int scrap)
    {
        this.scrap = scrap;
    }

    public void PickupScrap()
    {
        scrap += 20;
        Debug.Log("scrap is now at: " +  scrap);
    }
}

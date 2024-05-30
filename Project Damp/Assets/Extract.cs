using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Extract : MonoBehaviour
{

    private int part = 0;

    public int GetPart() { return part; }

    float startTime;
    private ShipHealth winnerShip;
    private ExtractionBeacon beacon;
    [SerializeField] float winTime;
    private bool gameOver = false;
    //need to make sure that beacon does not get removed after used, only one ship allowed
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ship"))
        {
            collision.gameObject.GetComponent<Inventory>().isInExtractionZone = true;
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ship"))
        {
            collision.GetComponent<Inventory>().isInExtractionZone = false;

            ExtractionFailed();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(part == 1){
            startTime = Time.time;
            part = 2;
        }
        else if(part == 2)
        {
            if(Time.time % 1 == 0)
            {
                if (beacon.getPlayer1() == null && beacon.getPlayer2() == null)
                {
                    ExtractionFailed();
                }
            }
            if(Time.time - startTime >= winTime)
            {
                if (!gameOver)
                {
                    gameOver = true;
                    Debug.LogWarning("Victory Achieved by: " + winnerShip.gameObject.name);
                    winnerShip.InvokeWin();
                }
            }
        }
        
    }
    
    public void ExtractionFailed()
    {

        part = 0;
        
    }

    public void beginExtraction(GameObject ship)
    {
        part = 1;
        winnerShip = ship.GetComponent<ShipHealth>();
        beacon = ship.GetComponent<ExtractionBeacon>();
    }


    public GameObject getShip()
    {
        return winnerShip.gameObject;
    }

}

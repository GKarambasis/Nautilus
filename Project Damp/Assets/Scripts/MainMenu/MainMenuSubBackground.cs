using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenuSubBackground : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float[] scale;

    [SerializeField] Transform endPoint;

    [SerializeField] float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
          
    }

    

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(moveSpeed * Time.deltaTime, 0f, 0f);
        
        
        if (gameObject.transform.position.x >= endPoint.position.x)
        {
            //random Scale
            gameObject.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

            //random Scale
            int scaleIndex = Random.Range(0, scale.Length);
            gameObject.transform.localScale = new Vector3(scale[scaleIndex], scale[scaleIndex], scale[scaleIndex]);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    public Transform target;
    private Transform teamSubmarine;
    [SerializeField] GameObject pointerObject;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if(target != null && gameManager.localPlayer.GetComponent<PlayerController>().isActiveAndEnabled)
        {
            teamSubmarine = gameManager.teamSubmarine.transform;
            if(teamSubmarine != null)
            {
                if(!pointerObject.activeInHierarchy)
                {
                    pointerObject.SetActive(true); //set the arrow active
                }
                

            
                // Calculate the screen position of the target relative to the player
                Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(target.position);

                // Calculate the screen position of the player
                Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(teamSubmarine.position);

                // Calculate the direction from player to target in screen space
                Vector3 dirToTargetScreenSpace = (targetScreenPosition - playerScreenPosition).normalized;

                // Calculate the rotation angle based on the direction
                float angle = Mathf.Atan2(dirToTargetScreenSpace.y, dirToTargetScreenSpace.x) * Mathf.Rad2Deg;

                // Set the position of the pointer on the canvas
                transform.position = playerScreenPosition;

                // Rotate the pointer on the canvas
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }

        }
        else
        {
            pointerObject.SetActive(false);
        }
    }


}

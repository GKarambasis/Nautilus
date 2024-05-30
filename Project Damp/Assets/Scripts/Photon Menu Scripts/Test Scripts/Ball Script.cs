using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallScript : MonoBehaviourPunCallbacks
{

    //Starting ball speed
    public float startSpeed = 5f;

    public float maxSpeed = 20f;

    public float speedIncrease = 0.25f;

    [SerializeField] float currentSpeed;

    private Vector2 currentDir;

    public Vector2 score;

    private float scoreGoalL;

    private float scoreGoalR;
        
    
    
    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = startSpeed;

        currentDir = Random.insideUnitCircle.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.CurrentRoom.Players.Count == 0)
        {
            return;
        }
        Vector2 moveDir = currentDir * currentSpeed * Time.deltaTime;
        transform.Translate(new Vector3(moveDir.x, moveDir.y, 0f));



    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
        {
            currentDir.y *= -1;
        }

        else if (other.tag == "Player")
        {
            currentDir.x *= -1;
        }

        else if (other.tag == "Goal_Right")
        {
            score.x++;
            ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
            ChangePositionTo(new Vector3(0f, 1.5f, 1f));
            ChangeDirTo(Random.insideUnitCircle.normalized);
            ChangeScore(score);
        }

        else if (other.tag == "Goal_Left")
        {
            score.y++;
            ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
            ChangePositionTo(new Vector3(0f, 1.5f, 1f));
            ChangeDirTo(Random.insideUnitCircle.normalized);
            ChangeScore(score);
        }


    }

    [PunRPC]
    void ChangeColorTo(Vector3 color)
    {
        GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);

        if (photonView.IsMine)
            photonView.RPC("ChangeColorTo", RpcTarget.OthersBuffered, color);
    }


    [PunRPC]
    void ChangeDirTo(Vector2 mycurrentDir) 
    {
        currentDir = mycurrentDir;
        if (photonView.IsMine)
            photonView.RPC("ChangeDirTo", RpcTarget.OthersBuffered, mycurrentDir);
    }

    [PunRPC]
    void ChangePositionTo (Vector3 myposition)
    {
        GetComponent<Transform>().position = myposition;
        if (photonView.IsMine)
            photonView.RPC("Change" +
                "PositionTo", RpcTarget.OthersBuffered, myposition);
    }

    [PunRPC]
    void Lel(Vector2 score)
    {
        scoreGoalL = score.x;
        scoreGoalR = score.y;
    }

    [PunRPC]
    void ChangeScore(Vector2 myscore)
    {
        Lel(score);
        if (photonView.IsMine)
            photonView.RPC("ChangeScore", RpcTarget.OthersBuffered, score);
    }


}

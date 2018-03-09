using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class NetworkShoot : NetworkBehaviour {


    private CompleteProject.PlayerShooting shootingScript;
    public VRTrackerTag vrGun;
    private CompleteProject.PlayerHealth playerHealth;
    [SyncVar(hook = "OnScoreChanged")]
    public int score;
    public event Action OnUpdateScore;

    public Text text;                      // Reference to the Text component.
    private Text scoreBoardText;

    [SyncVar(hook = "OnReady")]
    public bool ready;
    void Awake()
    {
        // Reset the score.
        score = 0;
        ready = false;
    }

    // Use this for initialization
    void Start () {
        shootingScript = GetComponentInChildren<CompleteProject.PlayerShooting>();
        playerHealth = gameObject.GetComponent<CompleteProject.PlayerHealth>();
        
        //Get the vrtrackertag associted to the gun
        if (VRTracker.instance != null && isLocalPlayer)
            vrGun = VRTracker.instance.GetTag(VRTracker.TagType.Gun);

        //Link the trigger with the shoot function
        if(vrGun != null)
        {
            vrGun.OnDown += FireShot;
        }

    }

    // Update is called once per frame
    void Update () {
		
	}

    // Execute the shoot on server
    [Command]
    void CmdShoot(Vector3 origin, Vector3 directions)
    {
        Vector3 destination = origin;
        int scoreObtained = shootingScript.Shoot(origin, directions, out destination);

        //Update the player status according to the score value
        if (scoreObtained > 0)
        {
            //Score positive means player killed a zombie bear
            score += scoreObtained;
            if(PlayerManager.instance != null)
            {
                Debug.Log("Udpdating player score " + score);
                PlayerManager.instance.UpdatePlayerScore(netId, score);
            }
        }
        else
        {
            //Negative score are used to pass information
            if(scoreObtained == -1)
            {
                //-1 is to start the game when the server hit the target
                ready = true;
                //Set player ready
                PlayerManager.instance.arePlayersReady = true;
                PlayerManager.instance.UpdateGameState();
                RpcReady();
            }
            else if (scoreObtained == -2)
            {
                //-2 is used to set the player that shoot to state ready
                //TODO Update this part, currently not funcitonnal
                //NetworkServer.SendToClient(GetComponent<NetworkIdentity>().connectionToClient.connectionId, MsgType.Ready, "Rea")
                TargetReady(GetComponent<NetworkIdentity>().connectionToClient);
            }
        }
        // Network the shoot to all the clients
        RpcShoot(origin, directions, destination);
    }

    /// <summary>
    /// Fire shoot is used in the local player to inform the server he is shooting
    /// </summary>
    void FireShot()
    {
        if (!playerHealth.isDead)
            CmdShoot(shootingScript.transform.position, shootingScript.transform.parent.transform.forward);
    }

    /// <summary>
    /// RpcShoot is used by the server to inform each player of the shoot
    /// </summary>
    /// <param name="origin">Origin point where the gun shot</param>
    /// <param name="directions">direction of the shoot</param>
    /// <param name="destination">destination of the shoot</param>
    [ClientRpc]
    void RpcShoot(Vector3 origin, Vector3 directions, Vector3 destination)
    {
        shootingScript.ShootEffects(origin, directions, destination);
    }

    /// <summary>
    /// OnScoreChanged is the function hooked to the syncvar score
    /// It will update the player score on the network
    /// </summary>
    /// <param name="value"></param>
    void OnScoreChanged(int value)
    {
        score = value;
        //Update local user score on its gun HUD
        if (isLocalPlayer)
            text.text = "Score: " + score;
        //Update score on the scoreboard
        if (scoreBoardText != null)
            scoreBoardText.text = score.ToString();
        //launch the event update score
        if (OnUpdateScore != null)
            OnUpdateScore();
    }

    /// <summary>
    /// Synchronization of the player ready state on the network
    /// </summary>
    /// <param name="state">state correspond to true if player ready, false otherwise</param>
    void OnReady(bool state)
    {
        ready = state;
    }

    /// <summary>
    /// TargetReady is aime to update the target on each player locally
    /// TODO : Should destroy only the target on the local client when local player hit the target
    /// </summary>
    /// <param name="target">target on the network</param>
    [TargetRpc]
    public void TargetReady(NetworkConnection target)
    {
        GameObject t = GameObject.FindGameObjectWithTag("Ready");
		Debug.Log ("Connection is ready " + target.address);
        if(t != null)
        {
            VRStandardAssets.ShootingGallery.ShootingTarget shootingTarget = GetComponent<VRStandardAssets.ShootingGallery.ShootingTarget>();
            if(shootingTarget != null)
            {
                shootingTarget.ImReady();
            }
        }
    }

    /// <summary>
    /// Broadcast on the server the target was shot
    /// Currently only when server shot the target
    /// </summary>
    [ClientRpc]
    public void RpcReady()
    {
        GameObject t = GameObject.FindGameObjectWithTag("Ready");

        if (t != null)
        {
            VRStandardAssets.ShootingGallery.ShootingTarget shootingTarget = t.GetComponent<VRStandardAssets.ShootingGallery.ShootingTarget>();
            if (shootingTarget != null)
            {
                shootingTarget.ImReady();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [ClientRpc]
    public void RpcAddScoreBoard()
    {
        //TODO add player info on score board
    }
}



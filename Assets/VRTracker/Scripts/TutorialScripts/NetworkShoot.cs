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
        /*if(shootingScript.vrGun != null)
        {
            // Callback for Local layer, not server
            shootingScript.vrGun.OnDown += CmdShoot;
        }*/
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
        //Debug.Log("Shooting " + origin + " direction " + directions);

        if (scoreObtained > 0)
        {
            score += scoreObtained;
        }
        else
        {
            if(scoreObtained == -1)
            {
                ready = true;
                //Set player ready
                //PlayerManager.instance.SetPlayerReady(Network.player.ipAddress);
                //Debug.Log("PLayer ready " + connectionToClient.address);
                PlayerManager.instance.arePlayersReady = true;
                PlayerManager.instance.UpdateGameState();
                RpcReady();
            }
            else if (scoreObtained == -2)
            {
                //NetworkServer.SendToClient(GetComponent<NetworkIdentity>().connectionToClient.connectionId, MsgType.Ready, "Rea")
                TargetReady(GetComponent<NetworkIdentity>().connectionToClient);
            }
        }
        // Execute functions linked to this action
        RpcShoot(origin, directions, destination);

    }

    
    void FireShot()
    {
        if (!playerHealth.isDead)
            CmdShoot(shootingScript.transform.position, shootingScript.transform.parent.transform.forward);
    }

    [ClientRpc]
    void RpcShoot(Vector3 origin, Vector3 directions, Vector3 destination)
    {
        shootingScript.ShootEffects(origin, directions, destination);
    }

    void OnScoreChanged(int value)
    {
        score = value;
        if (isLocalPlayer)
            text.text = "Score: " + score;
        if (scoreBoardText != null)
            scoreBoardText.text = score.ToString();
        if (OnUpdateScore != null)
            OnUpdateScore();
    }

    void OnReady(bool state)
    {
        ready = state;
    }

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
    /*public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        GameObject hud = transform.Find("HUD").gameObject;
        if (hud != null)
        {
            hud.SetActive(true);
        }
        GameObject damageHud = transform.Find("DamageHUD").gameObject;
        if (damageHud != null)
        {
            damageHud.SetActive(true);
        }
    }*/
}



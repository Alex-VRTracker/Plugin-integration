using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkShoot : NetworkBehaviour {


    private CompleteProject.PlayerShooting shootingScript;
    public VRTrackerTag vrGun;
    private CompleteProject.PlayerHealth playerHealth;
    [SyncVar(hook = "OnScoreChanged")]
    int score;
    public Text text;                      // Reference to the Text component.
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
        int scoreObtained = shootingScript.Shoot(origin, directions);
        Debug.Log("Shooting " + origin + " direction " + directions);

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
                PlayerManager.instance.SetPlayerReady(connectionToClient.address);
                Debug.Log("PLayer ready " + connectionToClient.address);
            }
        }
        // Execute functions linked to this action
        RpcShoot(origin, directions);

    }

    
    void FireShot()
    {
        if (!playerHealth.isDead)
            CmdShoot(shootingScript.transform.position, shootingScript.transform.parent.transform.forward);
    }

    [ClientRpc]
    void RpcShoot(Vector3 origin, Vector3 directions)
    {
        shootingScript.ShootEffects(origin, directions);
    }

    void OnScoreChanged(int value)
    {
        score = value;
        if (isLocalPlayer)
            text.text = "Score: " + score;
    }

    void OnReady(bool state)
    {
        ready = state;
    }

}

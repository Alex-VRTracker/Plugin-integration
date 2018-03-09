using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VRTrackerNetworkPlayer : NetworkBehaviour
{

	// Use this for initialization
	void Start () {
        if (isServer)
        {
            VRTrackerNetwork.instance.players.Add(transform.gameObject);
            Debug.Log("Network player ");
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnStartLocalPlayer()
    {
        Debug.LogWarning("On start local player client ");
        VRTracker.instance.SetLocalPlayer(gameObject);
        Debug.Log("Setting local player " + gameObject);
        //Activate the local player gun HUD
        GameObject hud = transform.Find("PlayerGunV2/HUD").gameObject;
        if(hud != null)
        {
            hud.SetActive(true);
        }

    }

}

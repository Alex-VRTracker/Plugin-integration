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
            VRTracker.instance.SetLocalPlayer(transform.parent.gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnStartLocalPlayer()
    {
        Debug.LogWarning("On start local player client ");

        //VRTrackerBoundaries.instance.localPlayer = gameObject;
        //VRTrackerBoundaries.instance.LookForLocalPlayer();
        GameObject hud = transform.Find("PlayerGunV2/HUD").gameObject;
        if(hud != null)
        {
            hud.SetActive(true);
        }
        /*GameObject damageHud = transform.Find("DamageHUD").gameObject;
        if (damageHud != null)
        {
            damageHud.SetActive(true);
        }*/
    }

}

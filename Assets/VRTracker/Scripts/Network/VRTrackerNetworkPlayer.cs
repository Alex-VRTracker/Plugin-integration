using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VRTrackerNetworkPlayer : NetworkBehaviour
{

    private NetworkTransformChild[] ntc;
	// Use this for initialization
	void Start () {
        if (isServer)
        {
            VRTrackerNetwork.instance.players.Add(transform.gameObject);
            Debug.Log("Network player ");
        }
        ntc = gameObject.GetComponents<NetworkTransformChild>();
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < ntc.Length; i++)
        {
            Debug.Log("Target " + ntc[i].target + "," + ntc[i].targetSyncPosition + ", " + ntc[i].target.transform.position);
        }
	}

    public override void OnStartLocalPlayer()
    {
        Debug.LogWarning("On start local player client ");
        VRTracker.instance.SetLocalPlayer(gameObject);
        Debug.Log("Setting local player " + gameObject);
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

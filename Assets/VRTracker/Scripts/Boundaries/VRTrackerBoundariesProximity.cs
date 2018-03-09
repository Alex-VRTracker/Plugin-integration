using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
public class VRTrackerBoundariesProximity : MonoBehaviour {

	public Transform player;
    public Transform controller;
	Renderer render;


	// Use this for initialization
	void Start () {
		render = gameObject.GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!player.Equals(null)) {
            //Update the player position and its controller position if exist
            if (player != null)
                render.sharedMaterial.SetVector("_PlayerPosition", player.position);
            if (controller != null)
                render.sharedMaterial.SetVector("_ControllerPosition", controller.position);
        }
    }
}

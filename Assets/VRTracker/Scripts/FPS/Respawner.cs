using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Respawner handle the respawn point 
/// Update player health once on the respawn point
/// </summary>
public class Respawner : MonoBehaviour {

    private GameObject respawnPoint;
    private Transform myHead;
    private CompleteProject.PlayerHealth playerHealth;

    // Use this for initialization
    void Start()
    {
        respawnPoint = GameObject.FindGameObjectsWithTag("SpawnPoint")[0];
        myHead = VRTracker.instance.GetHeadsetTag().transform;
        if (VRTracker.instance.GetLocalPlayer())
        {
            playerHealth = VRTracker.instance.GetLocalPlayer().GetComponent<CompleteProject.PlayerHealth>();

        }
        else
        {
            Debug.Log("Null player object");
            VRTracker.instance.OnNewLocalPlayer += RetrievePlayerHealth;
        }
    }

    void Update()
    {
        if(playerHealth != null)
        {
            if (playerHealth.isDead && VerifyPosition() && !respawnPoint.GetComponent<Objective>().reached)
            {
                respawnPoint.GetComponent<Objective>().ActivateObjective();
                Debug.Log("Respawing");
                playerHealth.CmdRespawn();
            }
        }
        
    }

    private bool VerifyPosition()
    {
        if (myHead != null)
        {
            if (respawnPoint != null && respawnPoint.activeSelf)
            {
                if (Mathf.Abs(myHead.position.x - respawnPoint.transform.position.x) > 0.4)
                {
                    return false;
                }
                else if (Mathf.Abs(myHead.position.z - respawnPoint.transform.position.z) > 0.4)
                {
                    return false;
                }
                return true;
            }
        }
        return false;
        
    }

    public void SetActiveSpawnPoint(bool isActive)
    {
        respawnPoint.SetActive(isActive);
        respawnPoint.GetComponentInChildren<Objective>().EnableSpawnPoint();
    }

    public void RetrievePlayerHealth()
    {
        if(VRTracker.instance.GetLocalPlayer())
        {
            playerHealth = VRTracker.instance.GetLocalPlayer().GetComponent<CompleteProject.PlayerHealth>();
            Debug.Log("Player health set");

        }
        else
        {
            Debug.Log("still null player object");
        }
    }

}

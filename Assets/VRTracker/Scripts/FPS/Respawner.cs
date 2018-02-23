using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour {


    private GameObject respawnPoint;
    private Transform myHead;
    private CompleteProject.PlayerHealth playerHealth;

    // Use this for initialization
    void Start()
    {
        respawnPoint = GameObject.FindGameObjectsWithTag("SpawnPoint")[0];
        myHead = transform.Find("Player");
        playerHealth = GetComponent<CompleteProject.PlayerHealth>();
    }

    void Update()
    {
        if (VerifyPosition() && !respawnPoint.GetComponent<Objective>().reached && playerHealth.isDead)
        {
            respawnPoint.GetComponent<Objective>().ActivateObjective();
            Debug.Log("Respawing");
            playerHealth.CmdRespawn();
        }
    }

    private bool VerifyPosition()
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
        else
        {
            return false;
        }
    }

    public void SetActiveSpawnPoint(bool isActive)
    {
        respawnPoint.SetActive(isActive);
        respawnPoint.GetComponentInChildren<Objective>().EnableSpawnPoint();
    }

}

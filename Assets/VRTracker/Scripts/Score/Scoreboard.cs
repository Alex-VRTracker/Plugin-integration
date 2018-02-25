using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Scoreboard : NetworkBehaviour
{

    [SerializeField]
    GameObject playerScoreItem;

    [SerializeField]
    Transform playerScoreboardList;

    Dictionary<string, GameObject> playerScore;


    void Start()
    {
        //Player[] players = GameManager.GetAllPlayers();

        /*foreach (Player player in players)
        {
            GameObject itemGO = (GameObject)Instantiate(playerScoreboardItem, playerScoreboardList);
            PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
            if (item != null)
            {
                item.Setup(player.username, player.kills, player.deaths);
            }
        }*/
    }

    /*void OnDisable()
    {
        foreach (Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }*/
    
	public NetworkInstanceId spawnPlayerScore()
	{
		GameObject scoreBoardItem = (GameObject)Instantiate(playerScoreItem, playerScoreboardList);
		PlayerScoreItem item = scoreBoardItem.GetComponent<PlayerScoreItem>();
		NetworkServer.Spawn(scoreBoardItem);
        if (scoreBoardItem != null)
            scoreBoardItem.transform.parent = playerScoreboardList;
        return item.GetComponent<NetworkIdentity> ().netId;
	}

    [ClientRpc]
	public void RpcAddPlayer(string name, NetworkInstanceId playerId, NetworkInstanceId scoreId)
    {
        Debug.LogWarning("Sending to client ");
		GameObject scoreBoardItem = ClientScene.FindLocalObject(scoreId);

        //GameObject scoreBoardItem = (GameObject)Instantiate(playerScoreItem, playerScoreboardList);
        //PlayerScoreItem item = scoreBoardItem.GetComponent<PlayerScoreItem>();
        Debug.LogWarning("Adding a to the list ");
		
		PlayerScoreItem item = scoreBoardItem.GetComponent<PlayerScoreItem>();
        GameObject player = ClientScene.FindLocalObject(playerId);
        if (item != null)
        {
            item.Setup(name, player.GetComponentInChildren<NetworkShoot>());
            
            //NetworkServer.Spawn(scoreBoardItem);
        }

    }
        
}

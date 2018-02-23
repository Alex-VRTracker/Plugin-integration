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
    
    public void AddPlayer(string name, GameObject player)
    {
        GameObject scoreBoardItem = (GameObject)Instantiate(playerScoreItem, playerScoreboardList);
        PlayerScoreItem item = scoreBoardItem.GetComponent<PlayerScoreItem>();
        if (item != null)
        {
            item.Setup(name, player.GetComponentInChildren<NetworkShoot>());
            NetworkServer.Spawn(scoreBoardItem);
        }

    }


    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Scoreboard : NetworkBehaviour
{

    [SerializeField]
    GameObject playerScoreItem;
    [SerializeField]
    PlayerScoreItem[] playersScore;
    [SerializeField]
    Transform playerScoreboardList;

    //[SyncVar(hook ="OnUpdatePlayerScore")]
    Dictionary<NetworkInstanceId, int> playerScore;

    Dictionary<NetworkInstanceId, PlayerScoreItem> playersLine;

    void Awake()
    {
        playerScore = new Dictionary<NetworkInstanceId, int>();
        playersLine = new Dictionary<NetworkInstanceId, PlayerScoreItem>();
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
        /*if(this != null)
        {
            PlayerManager.instance.eventAddPlayer += AddPlayer;
        }*/
    }

    /*void OnDisable()
    {
        foreach (Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }*/
    
	public NetworkInstanceId SpawnPlayerScore()
	{
		GameObject scoreBoardItem = (GameObject)Instantiate(playerScoreItem, playerScoreboardList);
		PlayerScoreItem item = scoreBoardItem.GetComponent<PlayerScoreItem>();
		NetworkServer.Spawn(scoreBoardItem);
        if (scoreBoardItem != null)
            scoreBoardItem.transform.parent = playerScoreboardList;
        return item.GetComponent<NetworkIdentity> ().netId;
	}



    [ClientRpc]
	public void RpcAddPlayer(int number, NetworkInstanceId playerId)
    {
        /*GameObject scoreBoardItem = (GameObject)Instantiate(playerScoreItem, playerScoreboardList);
        if (scoreBoardItem != null)
            scoreBoardItem.transform.parent = playerScoreboardList;

        Debug.LogWarning("Sending to client ");
		//GameObject scoreBoardItem = ClientScene.FindLocalObject(scoreId);

        //GameObject scoreBoardItem = (GameObject)Instantiate(playerScoreItem, playerScoreboardList);
        //PlayerScoreItem item = scoreBoardItem.GetComponent<PlayerScoreItem>();
        Debug.LogWarning("Adding a to the list ");
		
		PlayerScoreItem item = scoreBoardItem.GetComponent<PlayerScoreItem>();
        GameObject player = ClientScene.FindLocalObject(playerId);
        if (item != null)
        {
            item.Setup(name, player.GetComponentInChildren<NetworkShoot>());
            
            //NetworkServer.Spawn(scoreBoardItem);
        }*/
        Debug.LogWarning("RPC Adding player " + number + ", " + playersScore[number - 1].scoreText.transform.gameObject.activeSelf);

        //PlayerScoreItem[] playerline = GetComponentsInChildren<PlayerScoreItem>();
        for (int i = 0; i < number && i < playersScore.Length; i++)
        {
            if(!playersScore[i].transform.parent.gameObject.activeSelf)
                playersScore[i].transform.parent.gameObject.SetActive(true);
        }
        //playersScore[number - 1].scoreText.transform.parent.gameObject.SetActive(true);

    }



    public void AddPlayer(int number, NetworkInstanceId nId)
    {
        Debug.LogWarning("Adding player " + number + ", " + nId);
        Debug.LogWarning("playerLine "  + playersLine);

        //PlayerScoreItem[] playerline = GetComponentsInChildren<PlayerScoreItem>();
        if (playersScore.Length >= number)
        {
            Debug.LogWarning("Score player " + playersScore[number - 1]);

            if (playersScore[number - 1] != null)
                playersLine[nId] = playersScore[number - 1];
        }
        for (int i = 0; i < number - 1 && i < playersScore.Length; i++)
        {
            playersScore[i].scoreText.transform.parent.gameObject.SetActive(true);
        }
    }
        
    public void OnUpdatePlayerScore(NetworkInstanceId nId, int score)
    {
        playerScore[nId] = score;
        playersLine[nId].score = score;
    }

    public void SetPlayerScore(NetworkInstanceId nId, int score)
    {
        playerScore[nId] = score;
        playersLine[nId].score = score;
    }
}

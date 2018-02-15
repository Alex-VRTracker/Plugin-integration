﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{

    public bool startGame = false;
    public bool arePlayersReady = false;
    public static PlayerManager instance;
    public VRStandardAssets.ShootingGallery.ShootingTarget target;
    Dictionary<string, bool> playerReadyState;
    private int playerNumber = 0;
    private int playersReady = 0;
    private int alivePlayer = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Player Manger");
        }
        else
        {
            instance = this;
        }
        playerReadyState = new Dictionary<string, bool>();
    }

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
       
	}

    public void SetPlayerReady(string ip)
    {
        //arePlayersReady = true;
        //Set the player ready state
        Debug.Log("Setting PLayer ready " + ip);

        if (!playerReadyState[ip])
        {
            playerReadyState[ip] = true;
            playersReady++;
            UpdateGameState();
        }
    }

    public void RestartGame()
    {
        WaveManager.instance.EndGame();
        arePlayersReady = false;
        target.ResetTarget();
        startGame = false;
    }

    public void AddPlayer(string ip)
    {
        Debug.LogWarning("Adding player " + ip);

        playerReadyState[ip] = false;
        playerNumber++;
        alivePlayer++;
    }

    public void RemovePlayer(string ip)
    {
        if (playerReadyState[ip])
        {
            playersReady--;
        }
        playerReadyState.Remove(ip);
        playerNumber--;
        if(playerNumber > 0)
        {
            UpdateGameState();
        }
    }

    public void UpdateGameState()
    {
        Debug.LogWarning("Updating ready game " + playersReady + "/" + playerNumber);

        if (playersReady == playerNumber)
        {
            arePlayersReady = true;
        }
        //If all players are ready we start the game
        if (arePlayersReady)
        {
            WaveManager.instance.StartGame();
            startGame = true;
        }
    }

    public void RespawnPlayer()
    {
        Debug.LogWarning("Respawming a player " + alivePlayer + "/" + playerNumber);

        alivePlayer++;

    }

    public void DeadPlayer()
    {
        Debug.LogWarning("Dead player " + alivePlayer + "/" + playerNumber);
        alivePlayer--;
        if(alivePlayer <= 0)
        {
            RestartGame();
        }
    }
}

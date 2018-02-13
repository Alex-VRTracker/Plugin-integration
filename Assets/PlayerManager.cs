using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{

    public bool startGame = false;
    public bool isPlayersReady = false;
    public static PlayerManager instance;

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
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
       
	}

    public void SetPlayerReady()
    {
        isPlayersReady = true;
        if (isPlayersReady)
        {
            startGame = true;
            WaveManager.instance.StartGame();
            //startGame = false;
        }
    }

    
}

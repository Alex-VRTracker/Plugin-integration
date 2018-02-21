using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VRTrackerNetwork : NetworkManager
{
    public static VRTrackerNetwork instance;
    //public static VRTrackerNetwork singleton;
    private bool connectionAsClientEstablished = false;
    public NetworkClient mclient;
    public NetworkConnection mNetworkConn;
    public bool server;

    private bool serverIpReceived = false;
    private bool isWaitingForIP = false;

    public bool playerSpawned = false;

    public List<GameObject> players;

    void Start()
    {
        if (instance != null)
        {
            Debug.LogError("More than one VRTracker Network in the scene");
        }
        else
        {
            instance = this;
        }
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
              
    }
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {

        base.OnServerAddPlayer(conn, playerControllerId);
        //var newPlayer = conn.playerControllers[0].gameObject;
        /*Debug.LogWarning("Adding a new player " + conn.address);
        var newPlayer = conn.playerControllers[0].gameObject;

        if (newPlayer.GetComponent<NetworkIdentity>().isLocalPlayer && !VRTracker.instance.isSpectator)
        {
            Debug.LogWarning("Setting local player " + Network.player.ipAddress);
            VRTracker.instance.SetLocalPlayer(newPlayer);
            PlayerManager.instance.AddPlayer(Network.player.ipAddress);
            //Announcer.instance.SetAnnouncer(newPlayer.transform.Find("Announcer").GetComponentInChildren<Text>()); 
        }
        players.Add(newPlayer);
        */

    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        players.Remove(player.gameObject);
        if (Network.isServer)
        {
            //Add this player in the player manager
            PlayerManager.instance.RemovePlayer(conn.address);
        }
        base.OnServerRemovePlayer(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        foreach (var p in conn.playerControllers)
        {
            if (p != null && p.gameObject != null)
            {
                players.Remove(p.gameObject);
            }
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        
    }

    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);
        Debug.Log("New client " + client);

    }

}

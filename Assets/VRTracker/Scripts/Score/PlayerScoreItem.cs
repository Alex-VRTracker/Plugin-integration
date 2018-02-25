﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerScoreItem : NetworkBehaviour
{

    [SerializeField]
    Text usernameText;

    [SerializeField]
    Text scoreText;

    NetworkShoot scoreScript;

    public void Setup(string username, NetworkShoot ns)
    {
        usernameText.text = username;
        if(ns != null)
        {
            scoreScript = ns;
            scoreText.text = ns.score.ToString();
            ns.OnUpdateScore += UpdateScore;
        }
    }

    public void UpdateScore()
    {
        scoreText.text = scoreScript.score.ToString();
    }

}

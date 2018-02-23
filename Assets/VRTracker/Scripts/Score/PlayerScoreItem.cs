using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreItem : MonoBehaviour {

    [SerializeField]
    Text usernameText;

    [SerializeField]
    Text scoreText;

    public void Setup(string username, int score)
    {
        usernameText.text = username;
        scoreText.text = score.ToString();
    }

}

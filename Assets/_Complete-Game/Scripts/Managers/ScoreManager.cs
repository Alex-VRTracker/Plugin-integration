using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

namespace CompleteProject
{
    public class ScoreManager : NetworkBehaviour
    {

        [SyncVar(hook = "OnScoreChanged")]
        int score;

        //public int score;        // The player's score.


        public  Text text;                      // Reference to the Text component.


        void Awake ()
        {
            // Set up the reference.
            //text = GetComponent <Text> ();

            // Reset the score.
            score = 0;
        }


        void Update ()
        {
            // Set the displayed text to be the word "Score" followed by the score value.
            //text.text = "Score: " + score;
        }

        void OnScoreChanged(int value)
        {
            score = value;
            if (isLocalPlayer)
                text.text = "Score: " + score;
        }


    }
}
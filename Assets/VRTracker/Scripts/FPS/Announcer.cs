using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Announcer is used to handle the Text on the User HUD
/// In this projet it will display the informatino in the Player view
/// </summary>
public class Announcer : NetworkBehaviour
{

    public static Announcer instance;

    public GameObject textTemplate;
    public bool hasMessage = false;

    private Text currentText; //Text element 
    private List<string> messageList; // list containing the different messages
    private GameObject cam; //Associated camera on which to display
    private bool isWaiting = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Announcer in the scene");
        }
        else
        {
            instance = this;
        }
        messageList = new List<string>();
    }

    private void Start()
    {
        if(this != null)
        {
            //Retrieve local player camera
            Debug.Log("Player " + VRTracker.instance.GetLocalPlayer().transform.GetComponentInChildren<Camera>().GetComponentInChildren<Text>());
            if (!VRTracker.instance.isSpectator)
                currentText = VRTracker.instance.GetLocalPlayer().transform.GetComponentInChildren<Camera>().GetComponentInChildren<Text>();
        }

    }

    private void Update()
    {
        //Display message if some in the list
        if (messageList.Count > 0 && !isWaiting)
        {
            ShowMessage(messageList[0]);
            isWaiting = true;
            StartCoroutine(WaitForText());
        }
    }

    /// <summary>
    /// Addd the message into the list
    /// </summary>
    /// <param name="message">new message to display</param>
    public void AddMessage(string message)
    {
        if (!VRTracker.instance.isSpectator)
        {
            messageList.Add(message);
            hasMessage = true;
        }
    }

    /// <summary>
    /// Update the UI text with the next message on the list
    /// </summary>
    /// <param name="message">new message to display</param>
    public void UpdateMessage(string message)
    {
        if (!VRTracker.instance.isSpectator)
        {
            messageList[0] = message;
            if(currentText != null)
                currentText.text = message;
        }
    }

    /// <summary>
    /// Show the the UI Text with the corresponding message
    /// </summary>
    /// <param name="message">new message to display</param>
    private void ShowMessage(string message)
    {
        if (!VRTracker.instance.isSpectator && currentText != null)
        {
            currentText.text = message;
        }
    }

    /// <summary>
    /// Remove message once it has been displayed
    /// </summary>
    private void RemoveMessage()
    {
        if (!VRTracker.instance.isSpectator)
        {
            messageList.Remove(messageList[0]);
            if (currentText != null)
                currentText.text = "";
            if (messageList.Count == 0)
            {
                hasMessage = false;
            }
        }
    }

    /// <summary>
    /// Enumerator to be used in coroutine to automatically destroy message after 5 seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForText()
    {
        yield return new WaitForSeconds(5f);
        RemoveMessage();
        isWaiting = false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Announcer : NetworkBehaviour
{

    public static Announcer instance;

    public GameObject textTemplate;
    public bool hasMessage = false;

    private Text currentText;
    private List<string> messageList;
    private GameObject cam;
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
        //cam = Camera.main.gameObject
        if(this != null)
        {
            if (!VRTracker.instance.isSpectator)
                currentText = VRTracker.instance.GetLocalPlayer().transform.Find("Player").GetComponentInChildren<Text>();

        }

    }

    private void Update()
    {
        if (messageList.Count > 0 && !isWaiting)
        {
            ShowMessage(messageList[0]);
            isWaiting = true;
            StartCoroutine(WaitForText());
        }
    }

    public void AddMessage(string message)
    {
        if (!VRTracker.instance.isSpectator)
        {
            messageList.Add(message);
            hasMessage = true;
        }
    }

    public void UpdateMessage(string message)
    {
        if (!VRTracker.instance.isSpectator)
        {
            messageList[0] = message;
            currentText.text = message;
        }
    }

    private void ShowMessage(string message)
    {
        if (!VRTracker.instance.isSpectator && currentText != null)
        {
            currentText.text = message;
        }
    }

    private void RemoveMessage()
    {
        if (!VRTracker.instance.isSpectator)
        {
            messageList.Remove(messageList[0]);
            currentText.text = "";
            if (messageList.Count == 0)
            {
                hasMessage = false;
            }
        }
    }

    IEnumerator WaitForText()
    {
        yield return new WaitForSeconds(5f);
        RemoveMessage();
        isWaiting = false;
    }

}

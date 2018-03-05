﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;   

public class VRTrackerTagAssociation : MonoBehaviour {


	public static VRTrackerTagAssociation instance;
	public string JsonFilePath = "Player_Data.json";
	public Dictionary<string, string> userTagUID;
	public Dictionary<string, VRTrackerAssociation> prefabAssociation;
	[System.NonSerialized]public bool isWaitingForAssociation;
	public float assignationDelay = 10f; // Delay during which the User can press the red button on the Tag to assign it to one of its object in the game
	private JSONNode playerAssociation;
	[System.NonSerialized] public bool isAssociationLoaded;
	private Dictionary<string, bool> associatedMap; //Contains the association tag/object in the file
    private List<string> objectToAssign; // List of all object to assign from the player prefab
	private int numberOfAssociatedTag;
	private bool canSave;
	private List<string> availableTagMac;

	private float currentTime;


	//Call when script is loaded
	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("More than one VRTrackerTagAssociation in the scene");
		}
		else
		{
			instance = this;
			userTagUID = new Dictionary<string, string>();
			associatedMap = new Dictionary<string, bool>();
            objectToAssign = new List<string>();
			availableTagMac  = new List<string> ();
			prefabAssociation = new Dictionary<string, VRTrackerAssociation>();
			canSave = false;
			assignationDelay = 10f;
            numberOfAssociatedTag = 0;
            isAssociationLoaded = false;
        }
			
	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this);
	}


	// Update is called once per frame
	void Update () {

		/*if (isWaitingForAssociation)
		{
			currentTime -= Time.deltaTime;
			if(currentTime <= 0)
			{
				//Assignation time off
				currentTime = 0;
				isWaitingForAssociation = false;
				foreach (KeyValuePair<string, VRTrackerAssociation> kvp in prefabAssociation) {
					if (kvp.Value.isWaitingForAssignation) {
						kvp.Value.isWaitingForAssignation = false;
					}
				}
			}
		}*/
	}


	public void AddData(string gameObjectName, string tagUID)
	{
		//Store every tag association
		if (tagUID != "" && tagUID != "Enter Your Tag UID")
		{
			if (playerAssociation == null)
			{
				playerAssociation = new JSONObject();
			}
			playerAssociation[gameObjectName] = tagUID;
			canSave = true;
		}
	}

	public void SaveToPlayerPrefs(JSONObject data){

		foreach (KeyValuePair<string, string> kvp in data)
		{
			//TODO Check if value is not a node
			//Store every tag association in the player preferences
			if (kvp.Value != "" && kvp.Value != "Enter Your Tag UID")   
			{
				PlayerPrefs.SetString(kvp.Key, kvp.Value);
			}
		}

	}

    private void Save(){
		string filePath = Path.Combine(Application.persistentDataPath, JsonFilePath);
		Debug.Log ("Save path " + filePath);
		if(playerAssociation != null)
		{
			if(canSave)
			{
				string content = playerAssociation.ToString();
				Debug.Log("File Content " + content);
				System.IO.File.WriteAllText(filePath, content);
			}
		}else
		{
			Debug.Log("No Association to save");
		}
	}

    // Check if Tag association to User is saved in a file
    /*public void LoadAssociation(){
		// Path.Combine combines strings into a file path
		// Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build

		for (short i = 0; i < VRTracker.instance.tags.Count; i++)
		{
			if (VRTracker.instance.tags [i] != null) {
				Debug.Log("Prefab " + VRTracker.instance.tags[i].name);
				associatedMap[VRTracker.instance.tags[i].name] = false;
			}
		}

		string filePath = Path.Combine(Application.persistentDataPath, JsonFilePath);
		Debug.Log ("Opening " + filePath);

		if(File.Exists(filePath))
		{
			// Read the json from the file into a string
			string jsonDataString = File.ReadAllText(filePath); 
			playerAssociation = JSON.Parse(jsonDataString);
			updateAssociation ();
		}
		else
		{
			Debug.LogWarning("Cannot load json file!");
		}
	}*/
    // Check if Tag association to User is saved in a file
    public bool LoadAssociation()
    {

        string filePath = Path.Combine(Application.persistentDataPath, JsonFilePath);
        Debug.Log("Opening " + filePath);

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string jsonDataString = File.ReadAllText(filePath);
            playerAssociation = JSON.Parse(jsonDataString);
            //updateAssociation ();
            return true;
        }
        else
        {
            Debug.LogWarning("Cannot load json file!");
        }
        return false;
    }


    /// <summary>
    /// Update the association data from the file
    /// </summary>
	private void UpdateAssociation(){

        if (playerAssociation == null) {
			isAssociationLoaded = false;
			return;
		}
		JSONObject associationData = playerAssociation.AsObject;
		if(associatedMap == null)
		{
			associatedMap = new Dictionary<string, bool>();
		}
		if(userTagUID == null)
		{
			userTagUID = new Dictionary<string, string>();
		}
		for (short i = 0; i < playerAssociation.Count; i++)
		{
			associatedMap[playerAssociation.KeyAtIndex(i)] = true;
			userTagUID[playerAssociation.KeyAtIndex(i)] = playerAssociation[playerAssociation.KeyAtIndex(i)];
		}
		isAssociationLoaded = associatedMap.Count > 0;
		foreach (KeyValuePair<string, bool> association in associatedMap)
		{
			//Store every tag association in the player preferences
			isAssociationLoaded = isAssociationLoaded && association.Value;
			if (!isAssociationLoaded)
				break;
		}
	}

    /// <summary>
    /// Check if the tags in the association saved are all available
    /// </summary>
    /// <returns></returns>
	public bool isAllTagAvailable()
	{
		if(userTagUID != null && isAssociationLoaded){
			return 	checkAutoAssignation ();
		}
		else{
			return false;
		}
	}

	public void AddAvailableTag(string uid){
        availableTagMac.Add (uid);
	}

	public void resetAvailableTag(){
		availableTagMac.Clear ();
		numberOfAssociatedTag = 0;
	}

    /// <summary>
    /// Check if the auto assignation with the data saved is possible
    /// </summary>
    /// <returns></returns>
	public bool checkAutoAssignation(){
		numberOfAssociatedTag = 0;
        for (int i = 0; i < objectToAssign.Count; i++) {
            if (userTagUID.ContainsKey(objectToAssign[i]))
            {
                if (availableTagMac.Contains(userTagUID[objectToAssign[i]]) && numberOfAssociatedTag < objectToAssign.Count)
                {
                    numberOfAssociatedTag++;
                }
            }
            else
            {
                Debug.LogWarning("user do not contain " + objectToAssign[i]);
            }
        }
		return numberOfAssociatedTag == objectToAssign.Count && userTagUID.Count > 0;
			
	}

    /// <summary>
    /// Return the tag assigned to the prefab
    /// </summary>
    /// <param name="name">Name of the gameObject to which we want to know the associated tag</param>
    /// <returns></returns>
	public string getAssociatedTagID(string name){
		foreach (KeyValuePair<string, VRTrackerAssociation> kvp in prefabAssociation) {
			if (kvp.Key.Contains(name)) {
				return prefabAssociation [name].tagID;
			}
		}
		return "";
	}


	public IEnumerator WaitForAssignation()
	{
		//Prepare for assignation
		currentTime = assignationDelay;
		isWaitingForAssociation = true;
		while (isWaitingForAssociation)
		{
			yield return null;
		}
	}


    // Try to assign the Tag UID from the Player file
    public bool TryAutoAssignTag()
    {
        Debug.LogWarning("Hello");
        Debug.Log("Number of tracked element " + VRTracker.instance.tags.Count);
        Dictionary<VRTrackerTag, string> tagToAssign = new Dictionary<VRTrackerTag, string>();
        bool allTagAreInJSONList = true;

        // Check if the Tags to assign are all in the JSON file
        foreach (VRTrackerTag tag in VRTracker.instance.tags)
        {
            Debug.LogWarning("Tag type : " + tag.GetType());

            if (!tag.IDisAssigned)
            {

                bool tagFoundinJson = false;
                //foreach(KeyValuePair<string,string> jsonTag in playerAssociation){
                for (short i = 0; i < playerAssociation.Count; i++)
                {
                    if (playerAssociation.KeyAtIndex(i) == tag.tagType.ToString())
                    {
                        tagToAssign.Add(tag, playerAssociation[playerAssociation.KeyAtIndex(i)]);
                        Debug.LogWarning("Tag to assign : " + tag);
                        tagFoundinJson = true;
                    }
                }
                if (!tagFoundinJson)
                    allTagAreInJSONList = false;// && tagFoundinJson; // If one of the tag is not present, false
            }
        }

        if (!allTagAreInJSONList)
        {
            Debug.LogWarning("Tag Association Error : Could not find all Tag in the JSON file");
            return false;
        }

        // Check if the Tags to assign are available in the Gateway
        bool allLinkFound = true;
        foreach (KeyValuePair<VRTrackerTag, string> tagUID in tagToAssign)
        {
            bool tagLinkFound = false;
            foreach (string mac in availableTagMac)
            {
                if (mac == tagUID.Value)
                    tagLinkFound = true;
            }
            if (!tagLinkFound)
                allLinkFound = false;
        }

        if (!allLinkFound)
        {
            Debug.LogWarning("Tag Association Error : Could not find all Tag on the Gateway");
            return false;
        }

        foreach (KeyValuePair<VRTrackerTag, string> tagUID in tagToAssign)
        {
            tagUID.Key.AssignTag(tagUID.Value);
        }
        return true;
    }


    /// <summary>
    /// Saves the tag association for the game for this device
    /// </summary>
    public void SaveAssociation(){
        /*foreach(KeyValuePair<string, VRTrackerAssociation> tagAssociation in prefabAssociation)
		{
			if(tagAssociation.Value.tagID != "" && tagAssociation.Value.tagID != "Enter Your Tag UID")
			{
				//Store every tag association
				if (playerAssociation == null)
				{
					playerAssociation = new JSONObject();
				}
				playerAssociation[tagAssociation.Key] = tagAssociation.Value.tagID;
				canSave = true;
			}
		}
        //VRTrackerTagAssociation.instance.Save();
        */
        foreach (VRTrackerTag tag in VRTracker.instance.tags)
        {
            if (tag.UID != "" && tag.UID != "Enter Your Tag UID")
            {
                //Store every tag association
                if (playerAssociation == null)
                {
                    playerAssociation = new JSONObject();
                }
                playerAssociation[tag.tagType.ToString()] = tag.UID;
                canSave = true;
            }
        }
        Save();
	}

    /*public void AddPrefabAssociation(string prefabName, VRTrackerAssociation newAsso)
    {
        prefabAssociation.Add(prefabName, newAsso);
        objectToAssign.Add(prefabName);
    }*/
}
	
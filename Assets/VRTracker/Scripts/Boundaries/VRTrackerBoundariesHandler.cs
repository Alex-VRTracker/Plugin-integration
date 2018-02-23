﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;

public class VRTrackerBoundariesHandler : MonoBehaviour {
    /* 
     * VRTracker Boundaries is a singleton that will retrieve the local user
     * It will then add the player gameObject (Head + Controller) into the different boundaries
     * Each boundaries will appear when the local user will be close
     * 
     * It will also update and resize the boundaries
     */

    public static VRTrackerBoundariesHandler instance;

    private string JsonFilePath = "Boundaries.json";
    private JSONNode jBoundaries;

    public GameObject localPlayer;
    private VRTrackerTag[] vrtrackerTags;


    public GameObject northBoundary; // Boundary with the maximum Y
	public GameObject southBoundary; // Boundary with the minimum Y
    public GameObject eastBoundary;  // Boundary with the maximum X
    public GameObject westBoundary;  // Boundary with the minimum X
    public float borderLimitXMin;
	public float borderLimitXMax;
    public float borderLimitYMin;
    public float borderLimitYMax;
    public float borderLimitZMin;   //Currently not used
    public float borderLimitZMax;   //Currently not used

    //Dimension of the wall that was used in the testroom
    private float xOrigin = 5.8F;   
    private float yOrigin = 6.8F;


    // Use this for initialization
    void Start()
    {
        if (instance != null)
        {
            Debug.LogError("More than one VRTrackerTagAssociation in the scene");
        }
        else
        {
            instance = this;
        }
        RearrangeBoundaries();
        //UpdateValues(-3, 3, -4, 4);
    }

    /// <summary>
    /// LookForLocalPlayer will update the different boundaries with local player component (Head + controller)
    /// TODO : Update the function so that it can automatically handle different configurations for the player
    /// </summary>
    public void LookForLocalPlayer()
    {
        vrtrackerTags = localPlayer.GetComponentsInChildren<VRTrackerTag>();
        VRTrackerBoundariesProximity[] boundaries = GetComponentsInChildren<VRTrackerBoundariesProximity>();

        foreach (VRTrackerBoundariesProximity boundary in boundaries)
        {
            if (vrtrackerTags.Length > 0)
            {
                boundary.player = vrtrackerTags[0].transform;
            }
            if (vrtrackerTags.Length > 1)
            {
                boundary.controller = vrtrackerTags[1].transform;
            }
        }

    }

    // Check if Boundaries is saved in a file
    public void LoadBoundaries()
    {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build

        string filePath = Path.Combine(Application.persistentDataPath, JsonFilePath);
        Debug.Log("Opening " + filePath);

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string jsonDataString = File.ReadAllText(filePath);
            jBoundaries = JSON.Parse(jsonDataString);
            UpdateBoundaries();
        }
        else
        {
            Debug.LogWarning("Cannot load json file!");
        }
    }

    /// <summary>
    /// Update the boundaries data from the file
    /// Add the data into the JSON node object
    /// </summary>
    private void UpdateBoundaries()
    {

        JSONObject associationData = jBoundaries.AsObject;
        borderLimitXMin = jBoundaries["XMin"].AsFloat;
        borderLimitYMin = jBoundaries["YMin"].AsFloat;
        borderLimitZMin = jBoundaries["ZMin"].AsFloat;
        borderLimitXMax = jBoundaries["XMax"].AsFloat;
        borderLimitYMax = jBoundaries["YMax"].AsFloat;
        borderLimitZMax = jBoundaries["ZMax"].AsFloat;
        
    }

    /// <summary>
    /// Save the boundaries into the Json File
    /// </summary>
    public void SaveAssociation()
    {
   
        //Store every tag association
        if (jBoundaries == null)
        {
            jBoundaries = new JSONObject();
        }
        jBoundaries["XMin"] = borderLimitXMin;
        jBoundaries["YMin"] = borderLimitYMin;
        jBoundaries["ZMin"] = borderLimitZMin;
        jBoundaries["XMax"] = borderLimitXMax;
        jBoundaries["YMax"] = borderLimitYMax;
        jBoundaries["ZMax"] = borderLimitZMax;

        string filePath = Path.Combine(Application.persistentDataPath, JsonFilePath);
        if (jBoundaries != null)
        {
            string content = jBoundaries.ToString();
            System.IO.File.WriteAllText(filePath, content);
        }
        else
        {
            Debug.Log("No Boundaries to save");
        }
    }

    /// <summary>
    /// Resize the boundaries and update them
    /// </summary>
    public void RearrangeBoundaries()
    {
        float xLength = (borderLimitXMax - borderLimitXMin);
        float yLenght = (borderLimitYMax - borderLimitYMin); //Y and Z axis are inverted, this is the Y axis in the calibration coordinate
        float xScale = ((northBoundary.transform.localScale.x) * xLength) / xOrigin;
        float zScale = ((eastBoundary.transform.localScale.z) * yLenght) / yOrigin;
        float high = 1.5f;
        xOrigin = xLength;
        yOrigin = yLenght;

        //Front boundary
        northBoundary.transform.position = new Vector3((borderLimitXMax + borderLimitXMin)/2, high, borderLimitYMax);
        northBoundary.transform.localScale = new Vector3(xScale, northBoundary.transform.localScale.y, northBoundary.transform.localScale.z);

        //Back boundary
        southBoundary.transform.position = new Vector3((borderLimitXMax + borderLimitXMin)/2, high, borderLimitYMin);
        southBoundary.transform.localScale = new Vector3(xScale, southBoundary.transform.localScale.y, southBoundary.transform.localScale.z);

        //east boundary
        eastBoundary.transform.position = new Vector3(borderLimitXMax, high, (borderLimitYMax + borderLimitYMin) / 2);
        eastBoundary.transform.localScale = new Vector3(eastBoundary.transform.localScale.x, eastBoundary.transform.localScale.y, zScale);

        //west boundary
        westBoundary.transform.position = new Vector3(borderLimitXMin, high, (borderLimitYMax + borderLimitYMin) / 2);
        westBoundary.transform.localScale = new Vector3(westBoundary.transform.localScale.x, westBoundary.transform.localScale.y, zScale);

    }

    public void UpdateValues(float xMin, float xMax, float yMin, float yMax)
    {
        borderLimitXMin = xMin;
        borderLimitYMin = yMin;
        borderLimitXMax = xMax;
        borderLimitYMax = yMax;
        RearrangeBoundaries();
    }
}

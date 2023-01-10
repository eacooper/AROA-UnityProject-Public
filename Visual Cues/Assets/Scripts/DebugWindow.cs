using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugWindow : MonoBehaviour
    //Displays text from the Unity debugger on the interface
{
    TMP_Text textMesh;

    // Use this for initialization
    void Start()
    {
        textMesh = gameObject.GetComponentInChildren<TMP_Text>(); //Get the text mesh attached to this object
        //textMesh = transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMP_Text>();
        //Debug.Log("text mesh object: " + transform.GetChild(0).GetChild(0).gameObject);
        textMesh.text = "";
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogMessage; //Listen for log messages being received
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }

    public void LogMessage(string message, string stackTrace, LogType type)
        //If the text mesh is active, log the newest debug message on a new line
    {
        if (textMesh != null)
        {
            if (textMesh.text.Length > 400)
            {
                textMesh.text = message + "\n";
            }
            else
            {
                textMesh.text += message + "\n";
            }
        }
        else
        {
            //Debug.Log("Text mesh not found.");
        }
        
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Greyman;
using TMPro;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Audio;

public class ObstacleManager : MonoBehaviour
{
    //<summary>A script to manage obstacles and visual cues.</summary>

    //Key objects and scripts
    [Tooltip("Attach the Collocated Cues object.")]
    public GameObject cuesParent;

    [Tooltip("Attach the Calibration Obstacle object under Collocated Cues.")]
    public GameObject calibrationObstacle; //High obstacle at far back used to determine left/right alignment, under Collocated Cues.

    [Tooltip("Used to display debug text. Attach 'Debug Canvas' object under Main Camera.")]
    public GameObject debugCanvas;

    [Tooltip("Attach Experiment Logger script on this object.")]
    public Experiment_Logger experimentLogger;

    [Tooltip("Object with HUD scripts attached. Attach 'HUD Manager' under main camera.")]
    public GameObject HUD_Manager_Object;

    [Tooltip("Attach Text To Speech object under Main Camera.")]
    public TextToSpeech textToSpeech;

    //Hidden lists
    [HideInInspector]
    public List<GameObject> visualCues; //Hidden list of visual cues.

    //Private objects
    private LayerMask defaultMask = ~0;  //The layer masks are used to hide the collocated obstacles from the viewer.
    private LayerMask obstacleMask;
    private HUD_Manager HUD_manager; //HUD management script.


    //Initial settings
    [Tooltip("Sets whether collocated cues are on.")]
    public bool collocatedCuesOn = true;

    [Tooltip("Sets whether HUD cues are on.")]
    public bool hudCuesOn = true;

    [Tooltip("Whether the distance at which the user can see collocated obstacles is capped.")]
    public bool distanceCap = false;

    [Tooltip("Maximum distance to show obstacles.")]
    public float maxDisplayDistance = 5f;

    [Tooltip("Height of high obstacles in meters.")]
    public float highObstHeight = 1.524f;

    [Tooltip("Default user height in meters.")]
    public float defaultHeight = 1.6256f; //5'4" should be roughly average eye height




    // Start is called before the first frame update
    void Start()
    {
        //SavePositions();  //Saves default obstacle positions.
        HUD_manager = HUD_Manager_Object.GetComponent<HUD_Manager>();  //Assigns HUD_Manager script.

        //Create an obstacle mask that will allow the camera to ignore obstacles
        //By hiding them from the camera while leaving the obstacles there, we can have HUD cues on without collocated cues
        obstacleMask = LayerMask.GetMask("Obstacles");
        obstacleMask = ~obstacleMask;

        //Start with distance cap off
        Camera.main.farClipPlane = 1000;

        textToSpeech.StartSpeaking("Ready");
    }

    // Update is called once per frame
    void Update()
    {
        if (debugCanvas.activeSelf)
            //If the debug canvas is activated, update it with debugging information.
        {
            TextMeshProUGUI debug = debugCanvas.transform.Find("Debug Text").gameObject.GetComponent<TextMeshProUGUI>();

            string forwardOrBackward;
            if (experimentLogger.forward)
                forwardOrBackward = "forward";
            else
                forwardOrBackward = "backward";

            debug.text =
                "Mode, layout, direction: " + experimentLogger.cueCondition + ", " + experimentLogger.layout + ", " + forwardOrBackward + "\n" + 
                "High obstacle height: " + Mathf.Round(highObstHeight * 39.37f) + " inches" + "\n" +
                "Front angle and HUD Threshold: " + HUD_manager.frontAngle + ", " + HUD_manager.HUDThreshold;
                
            if (!distanceCap || HUD_manager.HUDCalibration)
            {
                //If distance is uncapped or HUD calibration is on, this will warn experimenter not to run
                //Note that distance cap and HUD Calibration are now both toggled simultaneously through Toggle Calibration
                debug.text += "\nDEACTIVATE CALIBRATION";
            }
            else
                debug.text += "\nOK TO EXPERIMENT";
        }
    }


    public void CollocatedCuesToggle()
    //Toggles collocated cue visibility.
    {
        //Debug.Log("Toggling collocated visual cues.");

        if (collocatedCuesOn)
        {
            Debug.Log("Collocated cues off.");
            textToSpeech.StartSpeaking("Co-located cues off.");

            Camera.main.cullingMask = obstacleMask; //Applies culling mask that hides obstacles.

            collocatedCuesOn = false;

        }

        else
        {
            Debug.Log("Collocated cues on.");
            textToSpeech.StartSpeaking("Co-located cues on.");

            Camera.main.cullingMask = defaultMask; //Resets to default culling mask.

            collocatedCuesOn = true;
        }
    }

    public void HUDCuesToggle()
    //Toggles HUD cue visibility.
    {
        //Debug.Log("Toggling HUD visual cues.");

        if (hudCuesOn)
        {
            Debug.Log("HUD cues off.");
            textToSpeech.StartSpeaking("HUD cues off.");

            HUD_Manager_Object.SetActive(false);
            hudCuesOn = false;
        }

        else
        {
            Debug.Log("HUD cues on.");
            textToSpeech.StartSpeaking("HUD cues on.");

            HUD_Manager_Object.SetActive(true);
            hudCuesOn = true;

        }
    }



    public void DebugToggle() //Toggles Debug text pane.
    {
        Debug.Log("Toggling debug text.");
        if (debugCanvas.activeSelf)
        {
            debugCanvas.SetActive(false);
            textToSpeech.StartSpeaking("Debug text off.");
        }
        else
        {
            debugCanvas.SetActive(true);
            textToSpeech.StartSpeaking("Debug text on.");
        }
    }


    public void SetLocation(string posName)
    {
        //Sets location of user to a given position by moving entire Collocated Cues object.
        if (posName == "front")
        {
            //Set cues parent to be at user's location and lower by height
            cuesParent.transform.position = Camera.main.transform.position - new Vector3 (0.0f, defaultHeight, 0.0f);

            //Then rotate in Y axis to match camera
            cuesParent.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            cuesParent.transform.Rotate(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
            Debug.Log("Location reset.");
            textToSpeech.StartSpeaking("Location reset.");

        }

        else
        {
            Debug.Log("Position not found. Location unchanged.");
        }
    }


    public void MoveForward(float dist)  //Adjusts position of cues forward or backward
    {
        cuesParent.transform.localPosition += cuesParent.transform.forward * dist;
    }

    public void MoveRight(float dist)    //Adjusts position of cues right or left
    {
        cuesParent.transform.localPosition += cuesParent.transform.right * dist;
    }

    public void MoveUp(float dist)  //Adjusts position of cues up or down
    {
        cuesParent.transform.localPosition += cuesParent.transform.up * dist;
    }


    public void ManualRotate(float degrees)
    {
        //Manually rotates cues left or right.
        cuesParent.transform.Rotate(0f, degrees, 0f);
    }



    public void ToggleDistanceCap (bool turnOn)  
    //Toggles whether collocated obstacles can be seen at any distance 
    {
        if (!turnOn)
        {
            Camera.main.farClipPlane = 1000;
            distanceCap = false;
            Debug.Log("Display distance uncapped.");
            //textToSpeech.StartSpeaking("Display distance uncapped.");
        }

        else
        {
            Camera.main.farClipPlane = maxDisplayDistance;
            distanceCap = true;
            Debug.Log("Display distance capped.");
            //textToSpeech.StartSpeaking("Display distance capped.");
        }
    }

    public void AdjustHighObstHeight (float heightAdjust)
    //Adjusts height of high obstacles
    {
        //This needs to both adjust current cue height, as well as tell the QR codes script to adjust height when moving cues.
        //The latter is done in QRCodes_AROA - it checks highObstHeight on Obstacle Manager.
        highObstHeight += heightAdjust;
        foreach (Transform Cue in cuesParent.transform)
        {
            if (Cue.name.Contains("High")) {
                Cue.position = new Vector3(Cue.position.x, Cue.position.y + heightAdjust, Cue.position.z);
            }
        }
        
        Debug.Log("High obstacle height is now " + Mathf.Round(highObstHeight * 39.37f) + " inches.");

    }
}

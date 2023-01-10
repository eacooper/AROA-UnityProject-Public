using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality;
using System;
using System.IO;
using UnityEngine.Windows.Speech;
using QRTracking;
using Microsoft.MixedReality.Toolkit.Audio;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

/// <summary>
/// Logs data for trial run to a text file.
/// </summary>
public class Experiment_Logger : MonoBehaviour
{

    public GameObject Camera;  //Attach camera
    public ObstacleManager obstacleManager; //Attach obstacle manager
    public QRCodes_AROA qRCodes_AROA;  //Attach QR Codes Manager
    public TextToSpeech textToSpeech;  //Attach Text to Speech object
    public GameObject HUDFrame; //Frame containing HUD cues
    //[HideInInspector]
    //public List<GameObject> HUDCues; //All HUD cue objects

    [HideInInspector]
    public string cueCondition = "Default Condition";  //Current cue mode (e.g. "collocated", "combined")

    [HideInInspector]
    public string layout = "Default Layout"; //Current layout
    private float startTime;
    private float endTime;

    public bool loggingInProcess = false;  //True while logging in process

    private string filePath;  //File path to save data
    //private string filePath2;
    private string fileName;  //File name to save data
    //private string fileName2;
    public bool forward = true; //Forward if true, backward if false
    private Vector3 tempPos;  //Temporary position
    private Vector3 tempRot;  //Temporary rotation
    private float tempTime;  //Temporary time
    //private float timeStamp;  
    private bool eyeTrackingEnabled;  //True if eye tracking enabled
    private bool eyeTrackingDataValid;  //True if eye tracking data is valid
    private Vector3 gazeDirection;  //Gaze direction (i.e. where the camera is pointed)
    private Vector3 eyeMovement;  //Eye movement
    private string directionString;  //String indicating direction (forward or backward)

    //HUD cue game objects
    private GameObject northCue;
    private GameObject southCue;
    private GameObject eastCue;
    private GameObject westCue;

    //HUD cue booleans
    private bool northOn;
    private bool eastOn;
    private bool southOn;
    private bool westOn;

  //private PTDataFileWriter dataWriter = new PTDataFileWriter();


    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform Cue in HUDFrame.transform)
        //Get the HUD cue objects from the HUD Frame
        {
            //HUDCues.Add(Cue.gameObject);

            if (Cue.gameObject.name == "HUD Cue North")
                northCue = Cue.gameObject;

            else if (Cue.gameObject.name == "HUD Cue East")
                eastCue = Cue.gameObject;

            else if (Cue.gameObject.name == "HUD Cue South")
                southCue = Cue.gameObject;

            else if (Cue.gameObject.name == "HUD Cue West")
                westCue = Cue.gameObject;

            else
                Debug.Log("Unidentified HUD cue.");
        }


        //Debug.Log("HUD Cues length: " + HUDCues.Count);


    }

    // Update is called once per frame
    void Update()
    {
        //Adjusting back to only log in fixed update
        /*

        //Set cue condition
        if (obstacleManager.collocatedCuesOn && obstacleManager.hudCuesOn)
            cueCondition = "Combined";
        else if (obstacleManager.collocatedCuesOn && !obstacleManager.hudCuesOn)
            cueCondition = "Collocated";
        else if (!obstacleManager.collocatedCuesOn && obstacleManager.hudCuesOn)
            cueCondition = "HUD";
        else
            cueCondition = "No Cues";
        //Debug.Log("Cue condition: " + cueCondition);

        //Get obstacle layout
        //layout = qRCodes_AROA.layout; //Now passed directly from QRCodes_AROA when a QR code is scanned
        //Debug.Log("Layout: " + layout);


        //Check if each of the HUD cues is on.
        foreach (GameObject Cue in HUDCues)
        {
            if (Cue.name == "HUD Cue North")
            {
                if (Cue.activeSelf)
                    northOn = true;
                else
                    northOn = false;
            }

            else if (Cue.name == "HUD Cue East")
            {
                if (Cue.activeSelf)
                    eastOn = true;
                else
                    eastOn = false;
            }

            else if (Cue.name == "HUD Cue South")
            {
                if (Cue.activeSelf)
                    southOn = true;
                else
                    southOn = false;
            }

            else if (Cue.name == "HUD Cue West")
            {
                if (Cue.activeSelf)
                    westOn = true;
                else
                    westOn = false;
            }
        }

        if (loggingInProcess)
        {

            tempPos = Camera.transform.position;
            tempRot = Camera.transform.rotation.eulerAngles;
            tempTime = Time.time - startTime;
            eyeTrackingEnabled = CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingEnabled;
            eyeTrackingDataValid = CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingDataValid;
            gazeDirection = CoreServices.InputSystem.EyeGazeProvider.GazeDirection;
            eyeMovement = Camera.transform.forward - gazeDirection;


            using (TextWriter writer = File.AppendText(filePath2))
            {
                writer.WriteLine("Update; " + cueCondition + "; " + layout + "; " + tempTime + "; " +
                    tempPos.x + "; " + tempPos.y + "; " + tempPos.z + "; " +
                    tempRot.x + "; " + tempRot.y + "; " + tempRot.z + "; " +
                    eyeTrackingEnabled + "; " + eyeTrackingDataValid + "; " +
                    gazeDirection.x + "; " + gazeDirection.y + "; " + gazeDirection.z + "; " +
                    eyeMovement.x + "; " + eyeMovement.y + "; " + eyeMovement.z + "; " +
                    northOn + "; " + eastOn + "; " + southOn + "; " + westOn);
            }
        }
        */

    }

    public void BeginLogging()
    //Start logging information
    {
        if (!loggingInProcess)  //If logging has already started, ignore this
        {
            loggingInProcess = true;

            Debug.Log("Beginning logging.");
            textToSpeech.StartSpeaking("Beginning logging.");

            // Create filename with experiment conditions

            //Track start time
            startTime = Time.time;

            //Set timestamp, file name, and path name
            string timeStamp = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
            if (forward)
            {
                directionString = "Forward";
                //fileName = timeStamp + '_' + cueCondition + '_' + layout + '_' + "forward.txt";
                //fileName2 = timeStamp + '_' + cueCondition + '_' + layout + '_' + "forward" + "_update.txt";
            }

            else
            {
                directionString = "Backward";
                //fileName = timeStamp + '_' + cueCondition + '_' + layout + '_' + "backward.txt";
                //fileName2 = timeStamp + '_' + cueCondition + '_' + layout + '_' + "backward" + "_update.txt";
            }

            fileName = timeStamp + '_' + cueCondition + '_' + layout + '_' + directionString + ".txt";

            Debug.Log("Filename: " + fileName);
            //Debug.Log("Filename 2: " + fileName2);
            filePath = Path.Combine(Application.persistentDataPath, fileName);
            //filePath2 = Path.Combine(Application.persistentDataPath, fileName2);
            Debug.Log("File path: " + filePath);
            //Debug.Log("File path: " + filePath2);


            //define variable values at start
            tempPos = Camera.transform.position;
            tempRot = Camera.transform.rotation.eulerAngles.normalized;
            gazeDirection = CoreServices.InputSystem.EyeGazeProvider.GazeDirection.normalized;

            //Create output file and write header
            //System.IO.File.WriteAllText(filePath, fileName);

            using (TextWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(
                    "Log Type; Cue Condition; Layout; Direction; Time; Position X; Position Y; Position Z; " +
                    "Rotation X; Rotation Y; Rotation Z; " +
                    "Eye Tracking Enabled (true/false); " + "Eye Tracking Data Valid (true/false); " + 
                    "Gaze Direction X; Gaze Direction Y; Gaze Direction Z; " +
                    "Eye Movement X; Eye Movement Y; Eye Movement Z; " +
                    "HUD Cue Up; HUD Cue Right; HUD Cue Down; HUD Cue Left");
            }
        }
 
        else
        {
            Debug.Log("Logging is currently in process.");
        }


    }

    public void EndLogging()
     //End logging
    {
        if (loggingInProcess) //Activate only if logging is currently running
        {
            loggingInProcess = false;

            Debug.Log("Ending logging.");
            textToSpeech.StartSpeaking("Ending logging.");

            //Append final line with end time
            using (TextWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine("Logging ended at " + tempTime);
            }

            /*
            using (TextWriter writer = File.AppendText(filePath2))
            {
                writer.WriteLine("Logging ended at " + tempTime);
            }
            */

            //Change from forward to backward or vice versa
            forward = !forward;
        }

        else
            Debug.Log("Logging not currently in process.");

    }

    void FixedUpdate()
    //Should activate every 0.02 seconds
    {
        //Set cue condition
        if (obstacleManager.collocatedCuesOn && obstacleManager.hudCuesOn)
            cueCondition = "Combined";
        else if (obstacleManager.collocatedCuesOn && !obstacleManager.hudCuesOn)
            cueCondition = "Collocated";
        else if (!obstacleManager.collocatedCuesOn && obstacleManager.hudCuesOn)
            cueCondition = "HUD";
        else
            cueCondition = "No Cues";
        //Debug.Log("Cue condition: " + cueCondition);

        //Get obstacle layout
        //layout = qRCodes_AROA.layout; //Now passed directly from QRCodes_AROA when a QR code is scanned
        //Debug.Log("Layout: " + layout);


        //Check if each of the HUD cues is on.
        if (northCue.activeSelf)
            northOn = true;
        else
            northOn = false;

        if (eastCue.activeSelf)
            eastOn = true;
        else
            eastOn = false;

        if (southCue.activeSelf)
            southOn = true;
        else
            southOn = false;

        if (westCue.activeSelf)
            westOn = true;
        else
            westOn = false;

        /*
         * Above is more efficient as long as there are only 4 HUD cues
        foreach (GameObject Cue in HUDCues)
        {
            if (Cue.name == "HUD Cue North")
            {
                if (Cue.activeSelf)
                    northOn = true;
                else
                    northOn = false;
            }

            else if (Cue.name == "HUD Cue East")
            {
                if (Cue.activeSelf)
                    eastOn = true;
                else
                    eastOn = false;
            }

            else if (Cue.name == "HUD Cue South")
            {
                if (Cue.activeSelf)
                    southOn = true;
                else
                    southOn = false;
            }

            else if (Cue.name == "HUD Cue West")
            {
                if (Cue.activeSelf)
                    westOn = true;
                else
                    westOn = false;
            }
        }
         */

        if (loggingInProcess)
        {
            //Assign current state to variables
            tempPos = Camera.transform.position;
            tempRot = Camera.transform.rotation.eulerAngles;
            tempTime = Time.time - startTime;
            eyeTrackingEnabled = CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingEnabled;
            eyeTrackingDataValid = CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingDataValid;
            gazeDirection = CoreServices.InputSystem.EyeGazeProvider.GazeDirection;
            eyeMovement = Camera.transform.forward - gazeDirection;

            //Write variables to log
            using (TextWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine("Fixed; " + cueCondition + "; " + layout + "; " + directionString + "; "  + tempTime + "; " +
                    tempPos.x + "; " + tempPos.y + "; " + tempPos.z + "; " +
                    tempRot.x + "; " + tempRot.y + "; " + tempRot.z + "; " + 
                    eyeTrackingEnabled + "; " + eyeTrackingDataValid + "; " + 
                    gazeDirection.x + "; " + gazeDirection.y + "; " + gazeDirection.z + "; " +
                    eyeMovement.x + "; " + eyeMovement.y + "; " + eyeMovement.z + "; " +  
                    northOn + "; " + eastOn + "; " + southOn + "; " + westOn);
            }
        }
    }


}

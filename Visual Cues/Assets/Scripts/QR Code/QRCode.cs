using Microsoft.MixedReality.Toolkit.Audio;
using System.Collections;

using System.Collections.Generic;
using UnityEngine;

namespace QRTracking
{
    [RequireComponent(typeof(QRTracking.SpatialGraphNodeTracker))]
    public class QRCode : MonoBehaviour
    {
        //<summary>A script attached to each QR Code object that tracks its information.</summary>

        public Microsoft.MixedReality.QR.QRCode qrCode;
        private GameObject qrCodeCube;

        public float PhysicalSize { get; private set; }
        public string CodeText { get; private set; }

        private TextMesh QRID;
        private TextMesh QRNodeID;
        private TextMesh QRText;
        private TextMesh QRVersion;
        private TextMesh QRTimeStamp;
        private TextMesh QRSize;
        private GameObject QRInfo;
        private bool validURI = false;
        private bool launch = false;
        private System.Uri uriResult;
        private long lastTimeStamp = 0;
        //AROA EDIT
        private TextMesh QRPositionText;
        public GameObject trackedObject; //Object that will mimic position of QR Code
        public TextToSpeech textToSpeech;
        public ObstacleManager obstacleManager;
        public Experiment_Logger experimentLogger;
        public string layout;
        public GameObject obstLow1;
        public GameObject obstLow2;
        public GameObject obstHigh1;
        public GameObject obstHigh2;
        public GameObject obstWide1;
        public GameObject obstWide2;
        public float startingHeight = 1.5f; //Height in meters of QR code above the ground
        public float startingDist = 0.9f; //Horizontal distance, in meters, from starting line to QR code (half width of hallway)

        public float lowHeight = 0.05f;
        public float medHeight = 0.444f;
        public float highHeight = 1.7f;
        public float wideHeight = 0.9f;

        // Use this for initialization
        void Start()
        {
            PhysicalSize = 0.1f;
            CodeText = "Dummy";
            if (qrCode == null)
            {
                throw new System.Exception("QR Code Empty");
            }

            PhysicalSize = qrCode.PhysicalSideLength;
            CodeText = qrCode.Data;

            qrCodeCube = gameObject.transform.Find("Cube").gameObject;
            QRInfo = gameObject.transform.Find("QRInfo").gameObject;
            QRID = QRInfo.transform.Find("QRID").gameObject.GetComponent<TextMesh>();
            QRNodeID = QRInfo.transform.Find("QRNodeID").gameObject.GetComponent<TextMesh>();
            QRText = QRInfo.transform.Find("QRText").gameObject.GetComponent<TextMesh>();
            QRVersion = QRInfo.transform.Find("QRVersion").gameObject.GetComponent<TextMesh>();
            QRTimeStamp = QRInfo.transform.Find("QRTimeStamp").gameObject.GetComponent<TextMesh>();
            QRSize = QRInfo.transform.Find("QRSize").gameObject.GetComponent<TextMesh>();
            //AROA EDIT - gets position text from QR code info
            QRPositionText = QRInfo.transform.Find("QRPositionText").gameObject.GetComponent<TextMesh>();

            QRID.text = "Id:" + qrCode.Id.ToString();
            QRNodeID.text = "NodeId:" + qrCode.SpatialGraphNodeId.ToString();
            QRText.text = CodeText;

            if (System.Uri.TryCreate(CodeText, System.UriKind.Absolute,out uriResult))
            {
                validURI = true;
                QRText.color = Color.blue;
            }

            QRVersion.text = "Ver: " + qrCode.Version;
            QRSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";
            QRTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
            QRTimeStamp.color = Color.yellow;
            Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " QRVersion = " + qrCode.Version + " QRData = " + CodeText);
        }

        void UpdatePropertiesDisplay()
        {
            // Update properties that change
            if (qrCode != null && lastTimeStamp != qrCode.SystemRelativeLastDetectedTime.Ticks)
            {
                QRSize.text = "Size:" + qrCode.PhysicalSideLength.ToString("F04") + "m";

                QRTimeStamp.text = "Time:" + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff");
                QRTimeStamp.color = QRTimeStamp.color == Color.yellow ? Color.white : Color.yellow;
                PhysicalSize = qrCode.PhysicalSideLength;
                Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " Time = " + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff"));

                qrCodeCube.transform.localPosition = new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);
                qrCodeCube.transform.localScale = new Vector3(PhysicalSize, PhysicalSize, 0.005f);
                lastTimeStamp = qrCode.SystemRelativeLastDetectedTime.Ticks;
                QRInfo.transform.localScale = new Vector3(PhysicalSize / 0.2f, PhysicalSize / 0.2f, PhysicalSize / 0.2f);

                //AROA EDIT
                //Get and log QR code position
                QRPositionText.text = "Position: " + qrCodeCube.transform.position;
                Debug.Log("Position = " + qrCodeCube.transform.position);
                if (trackedObject != null) { 
                    if (trackedObject.name == "Collocated Cues") //If tracked object is the Collocated Cues object, move whole room
                        //Note that this was coded when we were considering having a separate QR code on each obstacle, but this was never fully developed
                    {
                        if (!textToSpeech.IsSpeaking())
                            textToSpeech.StartSpeaking(layout);
                        //obstacleManager.ResetPositions(); //Reset positions of obstacles
                        trackedObject.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                        trackedObject.transform.Rotate(0f, qrCodeCube.transform.rotation.eulerAngles.y + 90f, 0f);//Rotate to match QR code, then 90 degrees 
                        trackedObject.transform.position = qrCodeCube.transform.position; //new Vector3 (0.9f, 0f, 0f);  //Adjust position to QR code location
                        //make local changes to adjust
                        //trackedObject.transform.localPosition -= trackedObject.transform.forward * 0.86f;
                        trackedObject.transform.localPosition -= trackedObject.transform.up * startingHeight; //Assumes height of QR Code is 1.5m (~5ft) off the ground
                        trackedObject.transform.localPosition += trackedObject.transform.right * startingDist; //Assumes hallway is 1.8m wide
                        highHeight = obstacleManager.highObstHeight;



                        if (layout == "Layout 1")
                        {
                            //Assign obstacles to position 1
                            obstWide1.transform.localPosition = new Vector3(-0.45f, wideHeight, 1.5f);
                            obstWide2.transform.localPosition = new Vector3(0.45f, wideHeight, 3.0f);
                            obstLow1.transform.localPosition = new Vector3(1000f, lowHeight, 1000f);
                            obstLow2.transform.localPosition = new Vector3(0f, lowHeight, 13.5f);
                            obstHigh1.transform.localPosition = new Vector3(0f, highHeight, 6.0f);
                            obstHigh2.transform.localPosition = new Vector3(0f, highHeight, 12.0f);

                        }

                        else if (layout == "Layout 2")
                        {
                            //Assign obstacles to position 2
                            obstWide1.transform.localPosition = new Vector3(-0.45f, wideHeight, 1.5f);
                            obstWide2.transform.localPosition = new Vector3(0.45f, wideHeight, 4.5f);
                            obstLow1.transform.localPosition = new Vector3(0f, lowHeight, 6f);
                            obstLow2.transform.localPosition = new Vector3(0f, lowHeight, 7.5f);
                            obstHigh1.transform.localPosition = new Vector3(0f, highHeight, 13.5f);
                            obstHigh2.transform.localPosition = new Vector3(0f, highHeight, 12.0f);
                        }

                        else if (layout == "Layout 3")
                        {
                            //Assign obstacles to position 3
                            obstWide1.transform.localPosition = new Vector3(-0.45f, wideHeight, 3.0f);
                            obstWide2.transform.localPosition = new Vector3(0.45f, wideHeight, 4.5f);
                            obstLow1.transform.localPosition = new Vector3(1000f, lowHeight, 1000f);
                            obstLow2.transform.localPosition = new Vector3(0f, lowHeight, 9.0f);
                            obstHigh1.transform.localPosition = new Vector3(0f, highHeight, 6.0f);
                            obstHigh2.transform.localPosition = new Vector3(0f, highHeight, 13.5f);
                        }

                        else if (layout == "Layout 4")
                        {
                            //Assign obstacles to position 4
                            obstWide1.transform.localPosition = new Vector3(-0.45f, wideHeight, 3.0f);
                            obstWide2.transform.localPosition = new Vector3(0.45f, wideHeight, 1.5f);
                            obstLow1.transform.localPosition = new Vector3(0f, lowHeight, 4.5f);
                            obstLow2.transform.localPosition = new Vector3(0f, lowHeight, 10.5f);
                            obstHigh1.transform.localPosition = new Vector3(0f, highHeight, 6.0f);
                            obstHigh2.transform.localPosition = new Vector3(0f, highHeight, 7.5f);
                        }

                        else if (layout == "Layout 5")
                        {
                            //Assign obstacles to position 5
                            obstWide1.transform.localPosition = new Vector3(-0.45f, wideHeight, 4.5f);
                            obstWide2.transform.localPosition = new Vector3(0.45f, wideHeight, 1.5f);
                            obstLow1.transform.localPosition = new Vector3(0f, lowHeight, 10.5f);
                            obstLow2.transform.localPosition = new Vector3(0f, lowHeight, 13.5f);
                            obstHigh1.transform.localPosition = new Vector3(0f, highHeight, 7.5f);
                            obstHigh2.transform.localPosition = new Vector3(0f, highHeight, 12.0f);
                        }


                        else if (layout == "Layout 6")
                        {
                            //Assign obstacles to position 6
                            obstWide1.transform.localPosition = new Vector3(-0.45f, wideHeight, 4.5f);
                            obstWide2.transform.localPosition = new Vector3(0.45f, wideHeight, 3.0f);
                            obstLow1.transform.localPosition = new Vector3(0f, lowHeight, 6f);
                            obstLow2.transform.localPosition = new Vector3(0f, lowHeight, 10.5f);
                            obstHigh1.transform.localPosition = new Vector3(0f, highHeight, 7.5f);
                            obstHigh2.transform.localPosition = new Vector3(1000f, highHeight, 1000f);
                        }

                        else if (layout == "Layout 7")
                        {
                            //Assign obstacles to position 7
                            obstWide1.transform.localPosition = new Vector3(-0.45f, wideHeight, 4.5f);
                            obstWide2.transform.localPosition = new Vector3(0.45f, wideHeight, 1.5f);
                            obstLow1.transform.localPosition = new Vector3(0f, lowHeight, 6f);
                            obstLow2.transform.localPosition = new Vector3(0f, lowHeight, 10.5f);
                            obstHigh1.transform.localPosition = new Vector3(0f, highHeight, 7.5f);
                            obstHigh2.transform.localPosition = new Vector3(0f, highHeight, 13.5f);
                        }

                        else if (layout == "Layout 8")
                        {
                            //Assign obstacles to position 8
                            obstWide1.transform.localPosition = new Vector3(-0.45f, wideHeight, 1.5f);
                            obstWide2.transform.localPosition = new Vector3(0.45f, wideHeight, 3.0f);
                            obstLow1.transform.localPosition = new Vector3(0f, lowHeight, 10.5f);
                            obstLow2.transform.localPosition = new Vector3(1000f, lowHeight, 1000f);
                            obstHigh1.transform.localPosition = new Vector3(0f, highHeight, 9f);
                            obstHigh2.transform.localPosition = new Vector3(0f, highHeight, 13.5f);
                        }

                        else if (layout == "Demo Layout")
                        {
                            //Assign a low obstacle to the middle of the room, and put the rest far away.
                            obstWide1.transform.localPosition = new Vector3(1000f, wideHeight, 1000f);
                            obstWide2.transform.localPosition = new Vector3(1000f, wideHeight, 1000f);
                            obstLow1.transform.localPosition = new Vector3(1000f, lowHeight, 1000f);
                            obstLow2.transform.localPosition = new Vector3(2f, lowHeight, 2f);
                            obstHigh1.transform.localPosition = new Vector3(1000f, highHeight, 1000f);
                            obstHigh2.transform.localPosition = new Vector3(1000f, highHeight, 1000f);
                        }

                        experimentLogger.layout = layout;
                        Debug.Log("Room position adjusted via QR code.");


                    }

                    else //Normal object
                    {
                        if (!textToSpeech.IsSpeaking())
                            textToSpeech.StartSpeaking("QR code detected.");
                        trackedObject.transform.localPosition = new Vector3(0f, 0f, 0f); //reset local position
                        trackedObject.transform.position = qrCodeCube.transform.position;
                        trackedObject.transform.rotation = qrCodeCube.transform.rotation;
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdatePropertiesDisplay();
            if (launch)
            {
                launch = false;
                LaunchUri();
            }
        }

        void LaunchUri()
        {
#if WINDOWS_UWP
            // Launch the URI
            UnityEngine.WSA.Launcher.LaunchUri(uriResult.ToString(), true);
#endif
        }

        public void OnInputClicked()
        {
            if (validURI)
            {
                launch = true;
            }
// eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }
    }
}
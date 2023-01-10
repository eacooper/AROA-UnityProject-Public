using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.Toolkit.Audio;

namespace QRTracking
{
    public class QRCodes_AROA : MonoBehaviour
    {
        //<summary>A script to help the AROA project work with QR codes.</summary>

        public GameObject qrCodePrefab;   //Attach the QRCode prefab
        public GameObject obstLow1;  //Attach the main obstacles
        public GameObject obstLow2;
        public GameObject obstHigh1;
        public GameObject obstHigh2;
        public GameObject obstWide1;
        public GameObject obstWide2;
        public GameObject obstacleCollection; //Attach the Collocated Cues object
        public Experiment_Logger experimentLogger;  //Attach the experiment logger
        public string layout = "Default"; //Current layout
        public float highObstHeight; //Holds the height of the high obstacle so it's maintained for a new code. Set to 0 by default.

        private System.Collections.Generic.SortedDictionary<System.Guid, GameObject> qrCodesObjectsList;  //List of QR code objects
        private bool clearExisting = false;  

        //AROA Edit
        public TextToSpeech textToSpeech;  //Attach text to speech object
        public ObstacleManager obstacleManager; //Attach obstacle manager

        struct ActionData
        {
            public enum Type
            {
                Added,
                Updated,
                Removed
            };
            public Type type;
            public Microsoft.MixedReality.QR.QRCode qrCode;

            public ActionData(Type type, Microsoft.MixedReality.QR.QRCode qRCode) : this()
            {
                this.type = type;
                qrCode = qRCode;
            }
        }

        private System.Collections.Generic.Queue<ActionData> pendingActions = new Queue<ActionData>();
        void Awake()
        {

        }

        // Use this for initialization
        void Start()
        {
            Debug.Log("QRCodesVisualizer start");
            qrCodesObjectsList = new SortedDictionary<System.Guid, GameObject>();

            QRCodesManager.Instance.QRCodesTrackingStateChanged += Instance_QRCodesTrackingStateChanged;
            QRCodesManager.Instance.QRCodeAdded += Instance_QRCodeAdded;
            QRCodesManager.Instance.QRCodeUpdated += Instance_QRCodeUpdated;
            QRCodesManager.Instance.QRCodeRemoved += Instance_QRCodeRemoved;

            if (qrCodePrefab == null || obstLow1 == null || obstLow2 == null || obstHigh1 == null || obstHigh2 == null || obstWide1 == null || obstWide2 == null || obstacleCollection == null)
            {
                //Check to make sure QR Code and obstacles are assigned properly
                throw new System.Exception("Prefab or obstacles not assigned");
            }
        }
        private void Instance_QRCodesTrackingStateChanged(object sender, bool status)
        {
            if (!status)
            {
                clearExisting = true;
            }
        }

        private void Instance_QRCodeAdded(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeAdded");

            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Added, e.Data));
            }
        }

        private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeUpdated");

            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Updated, e.Data));
            }
        }

        private void Instance_QRCodeRemoved(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
        {
            Debug.Log("QRCodesVisualizer Instance_QRCodeRemoved");

            lock (pendingActions)
            {
                pendingActions.Enqueue(new ActionData(ActionData.Type.Removed, e.Data));
            }
        }

        private void HandleEvents()
        {
            lock (pendingActions)
            {
                while (pendingActions.Count > 0)
                {
                    var action = pendingActions.Dequeue();
                    if (action.type == ActionData.Type.Added)
                    {
                        GameObject qrCodeObject = Instantiate(qrCodePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        qrCodeObject.GetComponent<SpatialGraphNodeTracker>().Id = action.qrCode.SpatialGraphNodeId;
                        qrCodeObject.GetComponent<QRCode>().qrCode = action.qrCode;
                        qrCodesObjectsList.Add(action.qrCode.Id, qrCodeObject);

                        //AROA EDIT - Assign object to QRCode script
                        Debug.Log("Action.qrCode.Data = " + action.qrCode.Data);
                        qrCodeObject.GetComponent<QRCode>().textToSpeech = textToSpeech;
                        qrCodeObject.GetComponent<QRCode>().obstacleManager = obstacleManager;
                                               
                        //Assign layout based on QR Code data
                        if (action.qrCode.Data == "QR Code 1")
                        {
                            layout = "Layout 1";
                        }

                        else if (action.qrCode.Data == "QR Code 2")
                        {
                            layout = "Layout 2";
                        }

                        else if (action.qrCode.Data == "QR Code 3")
                        {
                            layout = "Layout 3";
                        }

                        else if (action.qrCode.Data == "QR Code 4")
                        {
                            layout = "Layout 4";
                        }

                        else if (action.qrCode.Data == "QR Code 5")
                        {
                            layout = "Layout 5";
                        }

                        else if (action.qrCode.Data == "QR Code 6")
                        {
                            layout = "Layout 6";
                        }

                        else if (action.qrCode.Data == "QR Code 7")
                        {
                            layout = "Layout 7";
                        }

                        else if (action.qrCode.Data == "QR Code 8")
                        {
                            layout = "Layout 8";
                        }

                        else if (action.qrCode.Data == "Demo")
                        {
                            layout = "Demo Layout";
                        }

                        else
                        {
                            layout = "Unrecognized Layout";
                        }

                        experimentLogger.layout = layout;
                        qrCodeObject.GetComponent<QRCode>().layout = layout;
                        qrCodeObject.GetComponent<QRCode>().trackedObject = obstacleCollection;
                        qrCodeObject.GetComponent<QRCode>().experimentLogger = experimentLogger;

                        //Assign obstacles to QR Code object
                        qrCodeObject.GetComponent<QRCode>().obstLow1 = obstLow1;
                        qrCodeObject.GetComponent<QRCode>().obstLow2 = obstLow2;
                        qrCodeObject.GetComponent<QRCode>().obstHigh1 = obstHigh1;
                        qrCodeObject.GetComponent<QRCode>().obstHigh2 = obstHigh2;
                        qrCodeObject.GetComponent<QRCode>().obstWide1 = obstWide1;
                        qrCodeObject.GetComponent<QRCode>().obstWide2 = obstWide2;


                    }
                    else if (action.type == ActionData.Type.Updated)
                    {
                        if (!qrCodesObjectsList.ContainsKey(action.qrCode.Id))
                        {
                            GameObject qrCodeObject = Instantiate(qrCodePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                            qrCodeObject.GetComponent<SpatialGraphNodeTracker>().Id = action.qrCode.SpatialGraphNodeId;
                            qrCodeObject.GetComponent<QRCode>().qrCode = action.qrCode;
                            qrCodesObjectsList.Add(action.qrCode.Id, qrCodeObject);

                            //AROA EDIT
                            //QR codes created using https://www.qr-code-generator.com/
                            Debug.Log("Action.qrCode.Data = " + action.qrCode.Data);
                            qrCodeObject.GetComponent<QRCode>().textToSpeech = textToSpeech;
                            qrCodeObject.GetComponent<QRCode>().obstacleManager = obstacleManager;

                            //Same as above; assigning layouts based on QR code text
                            if (action.qrCode.Data == "QR Code 1")
                            {
                                layout = "Layout 1";
                            }

                            else if (action.qrCode.Data == "QR Code 2")
                            {
                                layout = "Layout 2";                            
                            }

                            else if (action.qrCode.Data == "QR Code 3")
                            {
                                layout = "Layout 3";
                            }

                            else if (action.qrCode.Data == "QR Code 4")
                            {
                                layout = "Layout 4";             
                            }

                            else if (action.qrCode.Data == "QR Code 5")
                            {
                                layout = "Layout 5";
                            }

                            else if (action.qrCode.Data == "QR Code 6")
                            {
                                layout = "Layout 6";
                            }

                            else if (action.qrCode.Data == "QR Code 7")
                            {
                                layout = "Layout 7";
                            }

                            else if (action.qrCode.Data == "QR Code 8")
                            {
                                layout = "Layout 8";
                            }

                            else if (action.qrCode.Data == "Demo")
                            {
                                layout = "Demo Layout";
                            }

                            else
                            {
                                layout = "Unrecognized Layout";
                            }

                            qrCodeObject.GetComponent<QRCode>().layout = layout;
                            qrCodeObject.GetComponent<QRCode>().trackedObject = obstacleCollection;
                            qrCodeObject.GetComponent<QRCode>().experimentLogger = experimentLogger;

                            //Assign obstacles to QR Code object
                            qrCodeObject.GetComponent<QRCode>().obstLow1 = obstLow1;
                            qrCodeObject.GetComponent<QRCode>().obstLow2 = obstLow2;
                            qrCodeObject.GetComponent<QRCode>().obstHigh1 = obstHigh1;
                            qrCodeObject.GetComponent<QRCode>().obstHigh2 = obstHigh2;
                            qrCodeObject.GetComponent<QRCode>().obstWide1 = obstWide1;
                            qrCodeObject.GetComponent<QRCode>().obstWide2 = obstWide2;

                        }
                    }
                    else if (action.type == ActionData.Type.Removed)
                    {
                        if (qrCodesObjectsList.ContainsKey(action.qrCode.Id))
                        {
                            Destroy(qrCodesObjectsList[action.qrCode.Id]);
                            qrCodesObjectsList.Remove(action.qrCode.Id);
                        }
                    }
                }
            }
            if (clearExisting)
            {
                clearExisting = false;
                foreach (var obj in qrCodesObjectsList)
                {
                    Destroy(obj.Value);
                }
                qrCodesObjectsList.Clear();

            }
        }

        // Update is called once per frame
        void Update()
        {
            HandleEvents();
        }
    }

}
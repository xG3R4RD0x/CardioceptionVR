using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ExciteOMeter;

[RequireComponent(typeof(AudioSource))]
public class HeartBeatVisualWithSignal: MonoBehaviour
{
    private bool start = true;
    //Collection
    public AudioClip [] Systole;
    public AudioClip [] Diastole;

    //Frequency
    private float BPM;
    [SerializeField] private float Hertz;
    [SerializeField] private float PeriodT; //1/FrequencyHertz
    [SerializeField] private float remainPeriod;
    [SerializeField] private float remainPeriodMillisec;
    public float returnTime = .01f;
    [SerializeField] private float startReturnTime;
    [SerializeField] private float startReturnTimeMillisec;

    //State
    public float [] GlobalTimer = { 0, 0 };
    public float[] CatchGlobalTimer = { 0, 0 };
    [SerializeField] private bool Lub;
    public int stateIndex = 0;
    public int isBeating;


    //Limbs
    [SerializeField] private AudioSource Speaker;
    public Toggle debugToggle;

    //Variable
    public bool useAsync; //make hearbeat multithread. after a lub, heartbeat can lub again without wait for dubb from first thread.
    // Start is called before the first frame update
    void Start()
    {   
    
    }
    // Invoked by the system when the component is enabled
    void OnEnable()
    {
        // EoM_Events acts a central nexus where all the data flows through
        // Here we add local methods as callbacks to events which are fired
        // whenever a new stream connects or disconnects and whenever a new
        // data packet is received
        ExciteOMeter.EoM_Events.OnStreamConnected += StreamConnection;
        ExciteOMeter.EoM_Events.OnStreamDisconnected += StreamDisconnection;
        ExciteOMeter.EoM_Events.OnDataReceived += DataReceived;
    }

    // Invoked by the system when the component is disbled
    void OnDisable()
    {
        // Remove callback methods when this component is disabled
        ExciteOMeter.EoM_Events.OnStreamConnected -= StreamConnection;
        ExciteOMeter.EoM_Events.OnStreamDisconnected -= StreamDisconnection;
        ExciteOMeter.EoM_Events.OnDataReceived -= DataReceived;
    }

    // Method which is called when a new stream is connected. The type param
    // indicates which stream has just connected
    private void StreamConnection(DataType type)
    {
        Debug.Log("Stream connected: " + type);
    }

    // Method which is called when a new stream is disconnected. The type
    // param indicates which stream has just disconnected
    private void StreamDisconnection(DataType type)
    {
        Debug.Log("Stream disconnected: " + type);
    }

    // This method is called whenever a new data packet is received.
    // The param 'type' indicates the type of of the data packet,
    // 'timestamp' indicates the time the packet was generated at and
    // 'value' contains the sesor value which was received.
    private void DataReceived(DataType type, float timestamp, float value)
    {
        // Check whether we have received a heart rate packet
        if (type == DataType.HeartRate) {
            // Print timestamp and value to Unity Log
            Debug.Log("Data Received: " + timestamp + " " + value);
        }
        if (type == DataType.HeartRate && start == true) {
            BPM = value;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
       
}



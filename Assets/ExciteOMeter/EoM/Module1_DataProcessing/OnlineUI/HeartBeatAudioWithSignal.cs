using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ExciteOMeter;

[RequireComponent(typeof(AudioSource))]
public class HeartBeatAudioWithSignal: MonoBehaviour
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
        Speaker = GetComponent<AudioSource>();
        // Hertz = BPM * 60f;
        // PeriodT = 1 / Hertz;
        // startReturnTime = returnTime;
        // startReturnTimeMillisec = startReturnTime * 1000;
        // remainPeriod = PeriodT;
        // remainPeriodMillisec = remainPeriod * 1000;
        // Debug.LogError(value);
        // Debug.LogError("Hello World");
        // Console.WriteLine("Hello World");
        // System.Diagnostics.Debug.WriteLine("Hello World");
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
            // Debug.Log("Data Received: " + timestamp + " " + value);
        }
        if (type == DataType.HeartRate && start == true) {
            Debug.Log("Starting HR "+ value);
            BPM = value;
            Speaker = GetComponent<AudioSource>();
            Hertz = BPM * 60f;
            PeriodT = 1 / Hertz;
            startReturnTime = returnTime;
            startReturnTimeMillisec = startReturnTime * 1000;
            remainPeriod = PeriodT;
            remainPeriodMillisec = remainPeriod * 1000;
            start = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < GlobalTimer.Length; i++)
        {
        }
        //Time=1/f
        Hertz = BPM / 60f;
        PeriodT = 1 / Hertz; //https://www.quora.com/How-do-you-convert-Hertz-to-seconds
        if (!Lub)
        {
            remainPeriod -= Time.deltaTime;
            remainPeriodMillisec -= Time.deltaTime * 1000;
            startReturnTime = returnTime;
            startReturnTimeMillisec = returnTime * 1000;
            if(remainPeriodMillisec <= 0f)
            {
                stateIndex = 1;
                if(debugToggle) debugToggle.isOn = true;
                if (Systole.Length > 0)
                {
                    for (int i = 0; i < Systole.Length; i++)
                    {
                        Speaker.PlayOneShot(Systole[i]);
                    }
                }
                Lub = true;
            }
        } else
        {
            remainPeriod = PeriodT;
            remainPeriodMillisec = PeriodT * 1000;
            startReturnTime -= Time.deltaTime;
            startReturnTimeMillisec -= Time.deltaTime * 1000;
            if(startReturnTimeMillisec <= 0f)
            {
                stateIndex = 0;
                if(debugToggle) debugToggle.isOn = false;
                if (Diastole.Length > 0)
                {
                    for (int i = 0; i < Diastole.Length; i++)
                    {
                        Speaker.PlayOneShot(Diastole[i]);
                    }
                }
                Lub = false;
            }
        }


    }
}



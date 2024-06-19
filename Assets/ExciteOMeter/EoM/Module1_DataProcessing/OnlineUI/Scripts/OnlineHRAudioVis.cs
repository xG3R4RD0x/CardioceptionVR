using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ExciteOMeter;
using System.Linq;
using UnityEngine.SceneManagement;

// This file is a combination of the audio and visual representations
[RequireComponent(typeof(AudioSource))]
public class OnlineHRAudioVis: MonoBehaviour
{
    // private GameObject heart;
    private int beat;
    // private bool restart = false;
    public float scaleChange;
    private Vector3 originalScale;
    // Borrowed from HeartBeatAudio:
    private Vector3 alteredScale;
    private Vector3 correctScale;
    private bool start = true;

    private bool change = true;
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
    private bool isBeating = false;
    private bool beatStarted = false;
    
    //Limbs
    [SerializeField] private AudioSource Speaker;
    public Toggle debugToggle;

    //Variable
    public bool useAsync; //make hearbeat multithread. after a lub, heartbeat can lub again without wait for dubb from first thread.
    // Start is called before the first frame update
    void Awake()
    {
        // heart = GameObject.Find("Heart");
        originalScale = transform.localScale;
        // Debug.LogError("After local is set: originalScale: " + originalScale.ToString("f4")+ "type:" + alteredScale.GetType());
        alteredScale = Vector3.Scale(originalScale, new Vector3(scaleChange, scaleChange, scaleChange));
        correctScale = originalScale;
        // Debug.LogError("altered scale" + alteredScale.ToString("f4") + "type:" + alteredScale.GetType());
        
    }
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
        // Debug.Log("Stream connected: " + type);
    }

    // Method which is called when a new stream is disconnected. The type
    // param indicates which stream has just disconnected
    private void StreamDisconnection(DataType type)
    {
        // Debug.Log("Stream disconnected: " + type);
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
            // BPM = changeHeartRate(value);
            // BPM = value;
            BPM = 70;
            // Debug.Log("changed HR "+ BPM);
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

    private void AssignChangeData(DataType type, float timestamp, float value)
    {
        // Check whether we have received a heart rate packet
        if (type == DataType.HeartRate & change) {
            Debug.Log("Within Test Function, HR: "+ value);
            BPM = changeHeartRate(value);
            Debug.Log("After Change, Within Test Function, HR: "+ BPM);
            change = false;
        }
    }

    private void startBeat()
    {
        if(!beatStarted){
            StartCoroutine(turnOnHR());
            beatStarted = true;
        }
       
        // Debug.LogError("beat started check, is beatiing is: " + isBeating.ToString() + " and beatStarted is: " + beatStarted.ToString());
    }

    private float changeHeartRate(float receivedHR)
    {
        // Multiply rate by .3 (getting 30%) and add, subtract, or neither to BPM randomly:
        // List of 3 integers, where 0 corresponds to no change, -1 corresponds to 30% slower, and +1 corresponds to 30% higher
        List<int> numberList = new List<int>() { -1, 1, 0 };
        // foreach (var number in numberList)
        // {
        //    Debug.LogError("Number: " + number);
        // }
        // // Randomly Order it by Guid..
        numberList = numberList.OrderBy(i => Guid.NewGuid()).ToList();
        // foreach (var number in numberList)
        // {
        //    Debug.LogError("New Number: " + number);
        // }
        switch (numberList[0])
        {
        case -1:
            receivedHR = receivedHR - receivedHR*(.3f);
        break;
        case 1:
           receivedHR = receivedHR + receivedHR*(.3f);
        break;
        default:
        break;
        }
        // It's now shuffled.
        return receivedHR;
    }

     IEnumerator LoadExpAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

     // If you ever need to adjust the length of time that the heart shows and hide, change the "WaitForSeconds" float, where show + hide = total duration of HB
    IEnumerator ShowHide()
    {
        // Debug.LogError("Started Coroutine SHOWHIDE at timestamp : " + Time.time);
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine( Show() );
        transform.localScale = alteredScale;
        yield return StartCoroutine( Hide() );
        // Debug.LogError("---EndeDED Coroutine SHOWHIDE at timestamp : " + Time.time);
    }

    IEnumerator Show()
    {
        // Debug.LogError("Started Coroutine SHOW at timestamp : " + Time.time);
        yield return new WaitForSeconds(.3f);
        Speaker.PlayOneShot(Systole[0]);
        transform.localScale = alteredScale;
        // Debug.LogError("ended Coroutine SHOW at timestamp : " + Time.time);
    }

    IEnumerator Hide()
    {
        // Debug.LogError("Started Coroutine HIDE at timestamp : " + Time.time);
        yield return new WaitForSeconds(.5f);
        transform.localScale = originalScale;
        beat++;
        // Debug.LogError("ended Coroutine HIDE at timestamp : " + Time.time);
    }
   IEnumerator turnOnHR()
    {
        // Debug.LogError("isBeating within turnOnHR is:" + isBeating.ToString() );
        if(isBeating == false){
            //var used to turn on blinking
            isBeating = true;
            yield return new WaitForSeconds(10);
            StartCoroutine(turnOffHR());
        }
    }

    IEnumerator turnOffHR()
    {
        if(isBeating == true){ //var used to turn on blinking
            isBeating = false;
            yield return new WaitForSeconds(5);
            change = true;
            ChangeScene("HR-Confidence Questions");
            // TODO: Figure out where to stick this:
            // ExciteOMeter.EoM_Events.OnDataReceived += AssignChangeData;
            // StartCoroutine(turnOnHR());
        }
    }


    public void ChangeScene(string sceneName){
        StartCoroutine(LoadExpAsyncScene(sceneName));
    }
    // IEnumerator DelayHeartBeatStart()
    //     {
    //         // Debug.LogError("Started Coroutine HIDE at timestamp : " + Time.time);
    //         yield return new WaitForSeconds(3);
    //         startBeat();
    //     }

    void Update()
    { 
        // Debug.LogError("update called, isBeating is: " + isBeating.ToString() );
        startBeat();
        // Debug.LogError("update called, isBeating is: " + isBeating.ToString() + "after turnOnHR is called");
        if (isBeating){
            for(int i = 0; i < GlobalTimer.Length; i++)
            {
            }
            // Debug.LogError("is beating is:" + isBeating.ToString() );
            //Time=1/f
            // ============
            // If you want to customize this further and make use of the Diastole, follow this tutorial: https://www.youtube.com/watch?v=hE5TVuoEypY&ab_channel=LeonardoH%C3%A9ceca
            // This is because all lub dub real heart rate audio sources have background noise that this system cannot account for
            // with this tutorial, you could probably skirt that problem since there is no background noise
            // ============
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
                    StartCoroutine(ShowHide());
                    if (Systole.Length > 0)
                    {
                        // for (int i = 0; i < Systole.Length; i++)
                        // {
                        //     Speaker.PlayOneShot(Systole[0]);
                        // }
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
}







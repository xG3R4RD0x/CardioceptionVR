using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExciteOMeter;
using System.Linq;

[RequireComponent(typeof(AudioSource))]

public class CircleRateMod : MonoBehaviour
{
    //TODO: Before running particiapnts, confirm participant number system is working as intended.

    //Collection
    public ExperimentData experimentData = null;
    private int PIDint;
    private string PIDstring;
    public AudioClip[] Systole;

    //Variables to track heart visual state
    public float scaleChange;
    public float scaleChangeMini = .3f;
    private Vector3 originalScale;
    private Vector3 medScale;
    private Vector3 bigScale;


    //Frequency
    public float BPM = 40f;
    public float timeDurationInSeconds;

    // state
    public bool beatStopped;
    public bool invoke = true;
    private bool start = true;
    private bool endOfPractice;
    private bool hrConfidenceScene;
    private bool recordHR = true;

    //Limbs
    [SerializeField] private AudioSource Speaker;

    public int hrModifier;

    public IList<int> trialsList = new List<int>(new int[] { 1, -1, -1, 0, 1 });  // 5 pre-determined orderings

    public string hrMod;
    public int totalNumTrials = 5;

    //Variables used for debugging only
    public int lubBeat;
    public int beat;
    public int HRChange;
    public static List<float> timeDurs = new List<float>();
    public static List<float> timeDursCalc = new List<float>();

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

    private void DataReceived(DataType type, float timestamp, float value) // this is the only place HR is used
    {
        if (type == DataType.HeartRate & recordHR)
        {
            experimentData.startingHR = value;
            recordHR = false;
        }
    }

    private void StartOfPractice()
    {
            var value = 40f;
            ExperimentData.trialsList = trialsList; // add the trial list order
            BPM = ModifyHeartRate(value, trialsList, experimentData.practiceTrialNumber);
            experimentData.startOfPractice = false;
            start = false;
            timeDurationInSeconds = 1 / (0.0166666666666667f * BPM);
    }

    private void StartOfTrial()
    {
        if (start) // executed only once, on start of experiment
        {
            var value = 40f;
            BPM = ModifyHeartRate(value, trialsList, experimentData.practiceTrialNumber);
            start = false;
            timeDurationInSeconds = 1 / (0.0166666666666667f * BPM);
        }
    }

    // Will change HR to be fast, slower or not change it at all at random
    private float ModifyHeartRate(float receivedHR, IList<int> modifiedList, int trialNum)
    {
        // Multiply rate by .3 (getting 30%) and add, subtract, or neither to BPM randomly:
        // List of 5 integers, where 0 corresponds to no change,
        // -2 corresponds to 30% slower, -1 is 15% slower, 1 is 15% faster, 2 is 30% faster and +1 corresponds to 30% higher
        switch (modifiedList[trialNum]) // apply the randomization
        {

            case -2:
                hrMod = "-20";
                receivedHR = receivedHR - receivedHR * (.20f);
                hrModifier = -2;
                break;
            case -1:
                hrMod = "-10";
                receivedHR = receivedHR - receivedHR * (.10f);
                hrModifier = -1;
                break;
            case 1:
                hrMod = "+10";
                receivedHR = receivedHR + receivedHR * (.10f);
                hrModifier = 1;
                break;
            case 2:
                hrMod = "+20";
                receivedHR = receivedHR + receivedHR * (.20f);
                hrModifier = 2;
                break;
            default:
                hrMod = "real";
                break;
        }
        experimentData.practiceHrMod = hrMod;
        return receivedHR;
    }

    // -------------- Misc. helper functions ----------------

    // rendering audio, visual heart beat, or both
    void renderHR()
    {
        beat++; //debug variable
        timeDurs.Add(Time.time * 1000); //debug tool to check time between beats
        Speaker.PlayOneShot(Systole[0]);
        StartCoroutine(StartDub(.15f));
        // Show heart visualization (which is hidden in Start)
        if (transform.localScale == new Vector3(0, 0, 0))
        {
            transformScale(originalScale);
        }
        transformScale(bigScale);
        StartCoroutine(delayedTransformScale(medScale, .15f));
        StartCoroutine(delayedTransformScale(originalScale, .43f)); // the
        // audio of sound file 'dub' lasts .28. So, to return to
        // original size, add the delay betweem lub and dub, .15, to
        // the length of the audio clip, .28.
    }

    private void CheckOnEndCondition()
    {
        if (experimentData.practiceTrialNumber >= totalNumTrials) // check if its the end condition
        {
            endOfPractice = true;
            experimentData.endOfPractice = true;
            ChangeScene("Start Screen");
        }
    }

    //changes next scene
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadNextSceneAsync(sceneName));
    }

    private bool IsStartOfPractice()
    {
        if (experimentData.startOfPractice)
        {
            return true;
        }
        else return false;
    }

    // loads next scene
    IEnumerator LoadNextSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // stops a HRtrial 
    IEnumerator StopHR(float duration)
    {
        yield return new WaitForSeconds(duration);
        CancelInvoke("renderHR");
        // debug section
        Debug.Log("stop beat at " + duration + " seconds with beat count:  " + beat + " beats, and Lub Count was: " + lubBeat);
        for (int i = 1; i < timeDurs.Count - 1; i++)
        {
            timeDursCalc.Add(timeDurs[i - 1] - timeDurs[i]);
        }
        // end debug section
        yield return new WaitForSeconds(2);
        experimentData.practiceTrialNumber++;
        Debug.Log($"trial: {experimentData.practiceTrialNumber}");
        hrConfidenceScene = true;   //automatically is set to false on scene load
        ChangeScene("Rate-Confidence Questions"); // at end of a trial, show 2 questions
    }

    // ----- Function for Audio Modality ------------
    IEnumerator StartDub(float dubDelay)
    {
        yield return new WaitForSeconds(dubDelay);
        lubBeat++;
        Speaker.PlayOneShot(Systole[1]);
    }

    // ---------- Functions for Visual Modality ------------
    public void assignHeartScaleVariables()
    {
        originalScale = transform.localScale;
        bigScale = Vector3.Scale(originalScale, new Vector3(scaleChange, scaleChange, scaleChange));
        medScale = Vector3.Scale(originalScale, new Vector3(scaleChange, scaleChangeMini, scaleChangeMini));
    }

    public void transformScale(Vector3 scaleSize)
    {
        transform.localScale = scaleSize;
    }

    IEnumerator delayedTransformScale(Vector3 scaleSize, float delayDuration)
    {
        yield return new WaitForSeconds(delayDuration);
        transformScale(scaleSize);
    }

    // ------------ Unity Specific Functions (start, update) ------------
    void Awake()
    {
        // Save variables, like trial and mod string, in between scenes
        experimentData = FindObjectOfType<ExperimentData>();
        if (IsStartOfPractice())
        {
            StartOfPractice();
        }
    }

    void Start()
    {
        CheckOnEndCondition();
        if (!endOfPractice)
        {
            StartOfTrial();
            Speaker = GetComponent<AudioSource>();
            assignHeartScaleVariables(); // set the size of the visual heart rate scale
            transformScale(new Vector3(0, 0, 0));  // hide heart visualization

            if (!IsStartOfPractice())
            {
                trialsList = ExperimentData.trialsList;
            }
        }
    }


    void Update()
    {
       
        if (!beatStopped)
        {
            StartCoroutine(StopHR(7f));
            beatStopped = true;

        }

        if (invoke && timeDurationInSeconds != 0)
        {
            InvokeRepeating("renderHR", 1f, timeDurationInSeconds);
            invoke = false;
        }
    }
}

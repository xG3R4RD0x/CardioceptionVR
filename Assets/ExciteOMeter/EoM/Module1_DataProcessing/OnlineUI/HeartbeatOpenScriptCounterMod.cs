using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExciteOMeter;
using System.Linq;
using System.IO;

[RequireComponent(typeof(AudioSource))]

public class HeartbeatOpenScriptCounterMod : MonoBehaviour
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
    private float OGValue;

    // state
    public bool beatStopped;
    public bool invoke = true;
    private bool start = true;
    private bool hrConfidenceScene;
    private bool shuffled;
    private bool ecgStream;

    //Limbs
    [SerializeField] private AudioSource Speaker;

    //Variables used for debugging only
    // todo: convert to useful info to store in txt file
    // todo: increment beat by one to account for phantom first beat (and investigate at somepoint plz)
    public int lubBeat;
    public int beat;
    public int HRChange;
    public static List<float> timeDurs = new List<float>();
    public static List<float> timeDursCalc = new List<float>();

    // Modality toggle:
    private bool audioRep;
    private bool visioRep;
    private string modLetter;

    // Counterbalancing Modalities
    public string[] modCombos = new string[] { "avc", "acv", "vac", "vca", "cav", "cva" };  // 6 possible modality orderings
    public string modOrderString;
    public bool StartOfNewBlock = false; // the variable "numberTrials" determines this
    public int hrModifier;
    public bool firstHRRecieved = true;
    private static System.Random rng = new System.Random(); // variable used to randomize a list

    // Counterbalancing Starting HR
    // public IList<int> trialsList = new List<int>(new int[] { -2, -1, 0, 1, 2,  -2, -1, 0, 1, 2,
    //                                      -2, -1, 0, 1, 2,  -2, -1, 0, 1, 2,
    //                                      -2, -1, 0, 1, 2, });  // 25 possible HR modification orderings (per block, 75 ordering total)
    public IList<int> trialsList = new List<int>(new int[] { -2, -1, 0, 1, 2,  -2, -1, 0, 1, 2,
                                          -2, -1, 0, 1, 2});  // 15 possible HR modification orderings (per block, 45 ordering total)
    public IList<int> randomizedList;
    public List<int> startingTrialOrders = new List<int>(new int[] { });
    public string hrMod;
    public int totalNumTrials = 5;

    // data logging
    private bool logEndHR;
    // --------------- Get data from excite-o-meter -----------

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
        ExciteOMeter.EoM_Events.OnDataReceived += ChangeTimeDurationInBetweenSystole;

    }

    // Invoked by the system when the component is disbled
    void OnDisable()
    {
        // Remove callback methods when this component is disabled
        ExciteOMeter.EoM_Events.OnStreamConnected -= StreamConnection;
        ExciteOMeter.EoM_Events.OnStreamDisconnected -= StreamDisconnection;
        ExciteOMeter.EoM_Events.OnDataReceived -= DataReceived;
        ExciteOMeter.EoM_Events.OnDataReceived -= ChangeTimeDurationInBetweenSystole;

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

    // This method is called whenever a new data packet is received.
    // The param 'type' indicates the type of of the data packet,
    // 'timestamp' indicates the time the packet was generated at and
    // 'value' contains the sesor value which was received.
    // TODO alter this function to allow BPM updates mid trial
    private void DataReceived(DataType type, float timestamp, float value)
    {
        if(start == true && experimentData.startOfExperiment && !shuffled)
        {
            trialsList = Shuffle(trialsList); // Randomly Order list of trials by Guid,
                                              // get called only once at start, and again at end of block
            trialsList = ModifyTrialsListWithCounterBalance(trialsList); // determine starting heart rate based on counterbalance
            ExperimentData.trialsList = trialsList; // add the trial list order
            string result = "List contents: ";
            foreach (var trialMod in trialsList)
            {
                result += trialMod.ToString() + ", ";
            }
            WriteToFile(experimentData.filePath, $"\n Trial List for Block {experimentData.blockNumber + 1}: {result} \n"); // write it into participant data
            shuffled = true;
        }
        if (logEndHR & type == DataType.HeartRate)
        {
            WriteToFile(experimentData.filePath, $" End HR: {value}, Time { DateTimeOffset.Now.ToUnixTimeMilliseconds() }; \n");
            logEndHR = false;
            WriteToFile(experimentData.filePath, $"\nHR Changes in between trials: ");
        }
        // Check whether we have received a heart rate packet

        if (type == DataType.HeartRate && start == true && experimentData.startOfExperiment) // executed only once, on start of experiment
        {
            Debug.Log("Starting HR " + value);
            experimentData.startingHR = value;
            experimentData.startOfTrialLog = true;
            BPM = ModifyHeartRate(value, trialsList, experimentData.trialNumber);
            Debug.Log("Start of Experiment HR " + value);
            experimentData.startOfExperiment = false;
            start = false;
            timeDurationInSeconds = 1 / (0.0166666666666667f * BPM);
            WriteToFile(experimentData.filePath, $"\n\nTrial {experimentData.trialNumber +1} - Starting HR: {value}, Modified HR: {BPM}, Time: { DateTimeOffset.Now.ToUnixTimeMilliseconds()};");
            RecordHRMod(trialsList, experimentData.trialNumber);
        }
        else if (type == DataType.HeartRate && start == true)
        {
            experimentData.startingHR = value;
            experimentData.startOfTrialLog = true;
            BPM = ModifyHeartRate(value, trialsList, experimentData.trialNumber);
            start = false;
            timeDurationInSeconds = 1 / (0.0166666666666667f * BPM);
            // Print timestamp and value to Unity Log
            WriteToFile(experimentData.filePath, $"\n\nTrial {experimentData.trialNumber+1} - Starting HR: {value}, Modified HR: {BPM}, Time: { DateTimeOffset.Now.ToUnixTimeMilliseconds()};");
            RecordHRMod(trialsList, experimentData.trialNumber);
        }


        // Specifically for ECG data:
        if(type == DataType.RawECG)
        {
            ecgStream = true;
        }
     }

    // ------------------- using the data gathered from excite-o-meter ---------------

    private void ChangeTimeDurationInBetweenSystole(DataType type, float timestamp, float value)
    {
        // Check whether we have received a heart rate packet, and HR has
        // changed from the previous value
        // ---------- old problem code
        if (!start & type == DataType.HeartRate)
        {
            OGValue = value;
            value = ModifyHeartRate(value, trialsList, experimentData.trialNumber);
        }
        if (type == DataType.HeartRate && BPM != value && !start)
        {
            timeDurationInSeconds = 1 / (0.0166666666666667f * value);
            timeDurs.Add(0f); //debug tool to check time between beats
            HRChange++;
            BPM = value;
            Debug.Log("New HR recieved that is different from previous: " + value);
            // record that the heart rate has changed for the logs
            WriteToFile(experimentData.filePath, $" HR: {OGValue}, Modified HR: {BPM}, Time { DateTimeOffset.Now.ToUnixTimeMilliseconds()};");
        }
        // -------------- new fixed code:
        // 
    }

    public void WriteToFile(string filePath, string contents)
    {
        StreamWriter writer = new StreamWriter(filePath, true);
        writer.Write(contents);
        writer.Close();
    }

    // ------------- Counterbalancing Starting HR ----------------

    private List<int> ModifyTrialsListWithCounterBalance(IList<int> modifiedList) // includes a counterbalance for the start of the experiemnt
    {
        PIDstring = experimentData.participantID;
        if (int.TryParse(PIDstring, out PIDint))
        {
            // real starting condition
            if (PIDint % 2 == 0)
            {
                var indexOfFirst0 = modifiedList.IndexOf(0); // find position of first 0 in the list
                modifiedList.RemoveAt(indexOfFirst0); // remove it
                modifiedList.Insert(0, 0); //'re-add' it to the front
                startingTrialOrders.AddRange(modifiedList); // return modified list, which is now ordered for the starting trial
                return startingTrialOrders;
            }
            // fake starting condition
            if (modifiedList[0] == 0)
            {
                var i = 1;
                // find the first non-zero item in list (that is not the first item)
                while (modifiedList[0] == 0){
                    var removedMod = modifiedList[i];
                    modifiedList.RemoveAt(i); // remove it
                    modifiedList.Insert(0, removedMod); // 're-add' it to the front
                    i++;
                }
                startingTrialOrders.AddRange(modifiedList);
                return startingTrialOrders;
            }
            startingTrialOrders.AddRange(modifiedList);
            return startingTrialOrders;
        }
        else
        {
            Debug.Log("Bad input, start again");
            ChangeScene("Error Screen");
            return startingTrialOrders;
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
                receivedHR = receivedHR - receivedHR * (.30f);
                break;
            case -1:
                receivedHR = receivedHR - receivedHR * (.15f);
                break;
            case 1:
                receivedHR = receivedHR + receivedHR * (.15f);
                break;
            case 2:
                receivedHR = receivedHR + receivedHR * (.30f);
                break;
            default:
                break;
        }
        return receivedHR;
    }

    private void RecordHRMod(IList<int> modifiedList, int trialNum)
    {
        // List of 5 integers, where 0 corresponds to no change,
        // -2 corresponds to 30% slower, -1 is 15% slower, 1 is 15% faster, 2 is 30% faster and +1 corresponds to 30% higher
        switch (modifiedList[trialNum]) // apply the randomization
        {

            case -2:
                hrMod = "-30";
                break;
            case -1:
                hrMod = "-15";
                break;
            case 1:
                hrMod = "+15";
                break;
            case 2:
                hrMod = "+30";
                break;
            default:
                hrMod = "real";
                break;
        }
        Debug.Log($"---Rayna: hrMod is set in experiment data as: {hrMod} for trial number {trialNum}");
        experimentData.hrMod = hrMod;
    }

    public IList<T> Shuffle<T>(IList<T> intList)
    {

        //var rnd = new System.Random();
        //randomizedList = new intList.OrderBy(item => rnd.Next());
        //return randomizedList;
        int n = intList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = intList[k];
            intList[k] = intList[n];
            intList[n] = value;
        }
        return intList;
    }


    // ------------------ Counter Balancing Modality------------------------

    private string determineModality(string modOrderString)
    {
        var currentModLetter = modOrderString[experimentData.blockNumber].ToString();
        return currentModLetter;
    }

    private void setModality()
    {
        modOrderString = experimentData.modOrderString;
        modLetter = determineModality(modOrderString);
        if (modLetter == "a")
        {
            audioRep = true;
        }
        else if (modLetter == "v")
        {
            visioRep = true;
        }
        else if (modLetter == "c")
        {
            audioRep = true;
            visioRep = true;
        }
        else
        {
            ChangeScene("Error Screen");
        }
    }

    private string getModString()
    {
        PIDstring = experimentData.participantID;
        if (int.TryParse(PIDstring, out PIDint))
        {
            modOrderString = modCombos[PIDint % 6];
        }
        else
        {
            ChangeScene("Error Screen");
        }
        experimentData.modOrderString = modOrderString; //store it in between scenes
        return modOrderString;
    }

    // -------------- Misc. helper functions ----------------

    // rendering audio, visual heart beat, or both
    void renderHR()
    {
        beat++; //debug variable
        timeDurs.Add(Time.time * 1000); //debug tool to check time between beats
        if (audioRep)
        {
            Speaker.PlayOneShot(Systole[0]);
            StartCoroutine(StartDub(.15f));
        }
        if (visioRep)
        {
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
    }

    private void CheckOnEndCondition()
    {
        if (experimentData.blockNumber == 2 && experimentData.trialNumber > totalNumTrials-1) // check if its the end condition
        {
            ChangeScene("Survey Scene");
        }
        else
        {
            modOrderString = modCombos[PIDint % 6]; 
        }
    }

    //changes next scene
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadNextSceneAsync(sceneName));
    }

    private void OnStartOfExperiment()
    {
        modOrderString = getModString(); // instantiate the modality
                                         // order only at the start of the experiment
        experimentData.blockNumber = 0;  // at start of experiment, block number is set to 0
                                         // There will be 3 blocks total, block 0, 1, and 2
    }

    private bool IsStartOfTheExperiment()
    {
        if (experimentData.startOfExperiment)
        {
            return true;
        }
        else return false;
    }

    private void OnEndOfBlock()
    {
        trialsList = Shuffle(trialsList); // gets called only once per block
        ExperimentData.trialsList = trialsList; // save it between scenes
        string result = "List contents: ";
        foreach (var trialMod in trialsList)
        {
            result += trialMod.ToString() + ", ";
        }
        if(experimentData.blockNumber +2 < 4)
        {
            WriteToFile(experimentData.filePath, $"\n Trial List for Block {experimentData.blockNumber + 2}: {result} \n"); // write it into participant data
        }
        experimentData.trialNumber = 0;
        experimentData.blockNumber++;
        Debug.Log("End of Block");
        if (experimentData.blockNumber <= 2)
        {
            ChangeScene("Break Between Blocks");
        }
    }

    private bool EndOfBlock(int numberOfTrials) //Determines if it is the end of block 
    {
        if (experimentData.trialNumber >= numberOfTrials && !hrConfidenceScene) //hrConfidenceScene is set false on start,
                                                                                //and true right before the HR confidence scene starts
        {
            return true;
        }
        return false;
    }

    // wait until ECG data is connected to do anything, else, send error message
    IEnumerator WarnECGisNotConnect()
    {
        yield return new WaitForSeconds(2f);
        if (!ecgStream)
        {
            Debug.LogError("ECG STREAM IS DISCONNECTED");
            //display warning that ECG stream is disconnected
        }
      
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
        logEndHR = true; // used to indicate that end HR should be logged
        // debug section
        Debug.Log("stop beat at " + duration + " seconds with beat count:  " + beat + " beats, and Lub Count was: " + lubBeat);
        for(int i = 1; i < timeDurs.Count-1; i++)
        {
            timeDursCalc.Add(timeDurs[i-1] - timeDurs[i]);
        }
        // end debug section
        yield return new WaitForSeconds(2);
        experimentData.trialNumber++;
        Debug.Log($"trial: {experimentData.trialNumber}");
        hrConfidenceScene = true;   //automatically is set to false on scene load
        ChangeScene("HR-Confidence Questions"); // at end of a trial, show 2 questions
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
        if (IsStartOfTheExperiment())
        {
            OnStartOfExperiment();
        }
    }

    void Start()
    {
        StartCoroutine(WarnECGisNotConnect()); //Warns if the ECG stream is not connected after 2 seconds
        CheckOnEndCondition(); // checks if end condition reached and if so,
                               // show end screen
        Speaker = GetComponent<AudioSource>();
        assignHeartScaleVariables(); // set the size of the visual heart rate scale
        transformScale(new Vector3(0, 0, 0));  // hide heart visualization
        
        if(!IsStartOfTheExperiment())
        {
            trialsList = ExperimentData.trialsList;
        }
        setModality(); // set the modality at start of each trial
    }


    void Update()
    {
        // while there are still modality blocks to be shown, and it is not
        // the end of the current block:
        if (!EndOfBlock(totalNumTrials) && experimentData.blockNumber < 3 && ecgStream) // won't do anything unless the ECG stream is there
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
        else if (EndOfBlock(totalNumTrials))
        {
            OnEndOfBlock();
        }
    }
}

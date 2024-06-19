using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentData : MonoBehaviour
{
    // Practice Experiment Data
    public int practiceTrialNumber;
    public bool startOfPractice;
    public bool endOfPractice;
    public float practiceStartingHR;
    public string practiceHrMod;
    public static IList<int> practiceTrialsList;

    // Proper Experiment Data
    public string participantID;
    public string modOrderString;
    public int blockNumber;
    public int trialNumber;
    public string dirPath;
    public string filePath;
    public bool startOfExperiment;
    public bool endOfExperiment;
    public float startingTimeOfStudy;
    public float startingTimeOfExperiment;
    public float startingHR;
    public bool startOfTrialLog;
    public string hrMod;
    public static IList<int> trialsList;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TrackTime : MonoBehaviour
{
    public float hrQuestionDuration;
    // use the following two variables to calculate hrQuestionDuration
    public float hrQStart;
    public float hrQEnd;

    public float confQuestionDuration;
    // use the following two variables to calculate hrQuestionDuration
    public float confQStart;
    public float confQEnd;
    public string filePath; 

    public ExperimentData experimentData = null;
    private int indexConf;

    void Awake()
    {
        if (experimentData == null)
        {
            experimentData = FindObjectOfType<ExperimentData>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // since start is called *when the script is enabled* we are starting the HR Yes No timer here
        hrQStart = Time.time * 1000;
    }

    // Update is called once per frame
    void Update()
    {
        //log time when HR question panel is active
        //log time when Conf question panel is active
    }

    public void OnButtonPress(Button button)
    {
        // if then logic based on button name passed through here: so if yes or no button, record time took to answer Q
        // when conf panel continue button is pressed, log that time too. All should be added directly to the CSV file through
        // custom but similar function to exportToCSV style saveToFile functions. Just write on the fly, updating as it goes.
        // since there will only be two time points being added, time_placeholder_1, time_placeholder_2, these will be the two to
        // be updated.
        // on start experiment, and on end experiment will be logged as well? Figure out later.
        if (button.name == "ButtonYes" || button.name == "ButtonNo")
        {
            hrQEnd = Time.time * 1000;
            confQStart = Time.time * 1000;
            SaveToFile(hrQEnd, hrQStart);
        }
        if (button.name == "ButtonContinue")
        {
            confQEnd = Time.time * 1000;
            SaveToFile(confQEnd, confQStart);
        }
        //no else! we don't want weird unexpected behavior
        // if button is button ok, do something else
    }

    //write the duration of the Qs to CSV file:
    public void SaveToFile(float finishTime, float startTime)
    {
        if (!experimentData.endOfPractice)
        {
            filePath = experimentData.dirPath + $"/Practice_Round_PID_{experimentData.participantID}.csv";
        }
        else
        {
            filePath = experimentData.dirPath + $"/HRConfTiming_PID_{experimentData.participantID}_condition_{experimentData.modOrderString}.csv";
        }
        string fileData = File.ReadAllText(filePath);
        string[] lines = fileData.Split("\n"[0]);
        // get the last row (because length-1 is white space):
        string[] lineData = (lines[lines.Length - 2].Trim()).Split(","[0]);
        // find the first instance of duration placeholder !!!! important !!!! confidence must be logged after HR using this method
        indexConf = Array.IndexOf(lineData, "duration_placeholder");
        // replace that index with the duration value 
        var duration = finishTime - startTime;
        lineData[indexConf] = duration.ToString();
        // rebuild current line string and add it to lines array
        lines[lines.Length - 2] = string.Join(",", lineData);
        // write all back to CSV file by creating new CSV file:
        File.WriteAllText(filePath, "");
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] != "")
            {
                File.AppendAllText(filePath, lines[i] + "\n");
            }

        }
    }
}

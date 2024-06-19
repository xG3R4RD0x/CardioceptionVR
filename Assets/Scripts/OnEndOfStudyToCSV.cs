using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OnEndOfStudyToCSV : MonoBehaviour
{
    public string dirPath;
    public string filePath;
    public ExperimentData experimentData = null;
    int indexConf;
    public bool fileWritten;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (experimentData.endOfExperiment && !fileWritten)
        {
            SaveToFile();
            fileWritten = true;
        }
    }

    public void SaveToFile()
    {
        string filePath = experimentData.dirPath + $"/HRConfTiming_PID_{experimentData.participantID}_condition_{experimentData.modOrderString}.csv";
        string fileData = File.ReadAllText(filePath);
        string[] lines = fileData.Split("\n"[0]);
        // get the last row (because length-1 is white space):
        string[] lineData = (lines[lines.Length - 2].Trim()).Split(","[0]);
        // find the first instance of duration placeholder !!!! important !!!! must be the last time stamp added
        indexConf = Array.IndexOf(lineData, "end_time_placeholder");
        // replace that index with the duration value 
        var duration = Time.time*1000 - experimentData.startingTimeOfStudy; // first calculate duration of entire study
                                                                            // by subtracting current time in milliseconds by starting time in milliseconds                                                                         
        TimeSpan ts = TimeSpan.FromMilliseconds(duration);  // then format it in a nice, readable way
        lineData[indexConf] = ts.ToString();
        // rebuild current line string and add it to lines array
        lines[lines.Length - 2] = string.Join(",", lineData);
        //TODO: remove all instances of end_time_placeholder before closing this file
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

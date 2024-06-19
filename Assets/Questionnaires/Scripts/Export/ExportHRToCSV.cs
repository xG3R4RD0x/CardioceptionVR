using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEngine.UI;

// This document generates the CSV file. It is always called first, right after HR is chosen for the first time
public class ExportHRToCSV : MonoBehaviour
{
    public string participantID;
    public string modOrderString;
    public int blockNumber;
    public int trialNumber;
    public string dirPath;
    public string filePath;
    public ExperimentData experimentData = null;
    private List<string[]> rowData = new List<string[]>();
    public string str;

    // Start is called before the first frame update
    void Awake()
    {
        if (experimentData == null)
        {
            experimentData = FindObjectOfType<ExperimentData>();
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Source: 

    public void SaveToFile(Button button)
    {
        string filePath = experimentData.dirPath + $"/HRConfTiming_PID_{experimentData.participantID}_condition_{experimentData.modOrderString}.csv";
        string[] rowDataTemp = new string[10];
        if (!File.Exists(filePath))
        {// Creating First row of titles manually.. 
            rowDataTemp[0] = "Trial #";
            rowDataTemp[1] = "Modality";
            rowDataTemp[2] = "HR Mod";
            rowDataTemp[3] = "Actual HR";
            rowDataTemp[4] = "HR Participant Response";
            rowDataTemp[5] = "Accuracy";
            rowDataTemp[6] = "Confidence Rating";
            rowDataTemp[7] = "HR Question Duration (ms)"; // how long it takes the participant to choose
            rowDataTemp[8] = "Confidence Question Duration (ms)"; // how long it takes participant to report confidence
            rowDataTemp[9] = "Total Duration of Entire Study (ms)"; // how long it takes participant to complete the entire study
            rowData.Add(rowDataTemp);
        }

        rowDataTemp = new string[10];
        rowDataTemp[0] = "Trial" + experimentData.trialNumber;
        rowDataTemp[1] = experimentData.modOrderString[experimentData.blockNumber].ToString();
        Debug.Log($"---Rayna: HR Modifier in HR CSV Exporter: {experimentData.hrMod}");
        rowDataTemp[2] = experimentData.hrMod;
        rowDataTemp[3] = experimentData.startingHR.ToString();
        if (button.name == "ButtonYes")
        {
            rowDataTemp[4] = "matched";
        }
        else
        {
            rowDataTemp[4] = "no match";
        }
        if(experimentData.hrMod != "real" && rowDataTemp[4] == "matched")
        {
            rowDataTemp[5] = "0";
        }
        else if(experimentData.hrMod == "real" && rowDataTemp[4] == "matched")
        {
            rowDataTemp[5] = "1";
        }
        else if (experimentData.hrMod != "real" && rowDataTemp[4] == "no match")
        {
            rowDataTemp[5] = "1";
        }
        else // participant said it was fake but it was real
        {
            rowDataTemp[5] = "0";
        }
        rowDataTemp[6] = "confidence_placeholder";
        rowDataTemp[7] = "duration_placeholder";
        rowDataTemp[8] = "duration_placeholder";
        rowDataTemp[9] = "end_time_placeholder";
        rowData.Add(rowDataTemp);

        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));
        str = sb.ToString();

        if (!File.Exists(filePath))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filePath)) ;
            File.AppendAllText(filePath, str);
        }
        else
        {
            File.AppendAllText(filePath, str);
        }

       
    }
    public void OnButtonPress(Button button)
    {
      SaveToFile(button);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ExportPracticeData : MonoBehaviour
{
    public ExperimentData experimentData = null;
    Slider slider;
    private List<string[]> rowData = new List<string[]>();
    private string str;
    private string filePath;
    private int indexConf;

    void Awake()
    {
        if (experimentData == null)
        {
            experimentData = FindObjectOfType<ExperimentData>();
        }
    }
    void Start()
    {
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        filePath = experimentData.dirPath + $"/Practice_Round_PID_{experimentData.participantID}.csv";
    }

    void SaveConfToFile()
    {
       
        string fileData = File.ReadAllText(filePath);
        string[] lines = fileData.Split("\n"[0]);
        // get the last row (because length-1 is white space):
        string[] lineData = (lines[lines.Length - 2].Trim()).Split(","[0]);
        // find the first instance of confidence placeholder
        indexConf = Array.IndexOf(lineData, "confidence_placeholder");
        // replace that index with the confidence value 
        var sliderVal = slider.value.ToString();
        lineData[indexConf] = sliderVal;
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

    void SaveResponseToFile(Button button)
    {
        string[] rowDataTemp = new string[8];
        if (!File.Exists(filePath))
        {// Creating First row of titles manually.. 
            rowDataTemp[0] = "Trial #";
            rowDataTemp[1] = "Speed Mod";
            rowDataTemp[2] = "Starting HR";
            rowDataTemp[3] = "Participant Response";
            rowDataTemp[4] = "Accuracy";
            rowDataTemp[5] = "Confidence Rating";
            rowDataTemp[6] = "40 BPM Question Duration (ms)"; // how long it takes the participant to choose
            rowDataTemp[7] = "Confidence Question Duration (ms)"; // how long it takes participant to report confidence
            rowData.Add(rowDataTemp);
        }

        rowDataTemp = new string[8];
        rowDataTemp[0] = "Trial " + experimentData.practiceTrialNumber;
        rowDataTemp[1] = experimentData.practiceHrMod.ToString();
        rowDataTemp[2] = experimentData.startingHR.ToString();
        if (button.name == "ButtonYes")
        {
            rowDataTemp[3] = "matched";
        }
        else
        {
            rowDataTemp[3] = "no match";
        }
        if (experimentData.practiceHrMod != "real" && rowDataTemp[3] == "matched")
        {
            rowDataTemp[4] = "0";
        }
        else if (experimentData.practiceHrMod == "real" && rowDataTemp[3] == "matched")
        {
            rowDataTemp[4] = "1";
        }
        else if (experimentData.practiceHrMod != "real" && rowDataTemp[3] == "no match")
        {
            rowDataTemp[4] = "1";
        }
        else // participant said it was fake but it was real
        {
            rowDataTemp[4] = "0";
        }
        rowDataTemp[5] = "confidence_placeholder";
        rowDataTemp[6] = "duration_placeholder";
        rowDataTemp[7] = "duration_placeholder";
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
        if (button.name == "ButtonContinue")
        {
            SaveConfToFile();
        }
        else
        {
            SaveResponseToFile(button);
        }
    }
}



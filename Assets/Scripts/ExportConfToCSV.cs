using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEngine.UI;

public class ExportConfToCSV : MonoBehaviour
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
    Slider slider;
    int indexConf;
    string[] lines;

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
        slider = GameObject.Find("Slider").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Source: 

    public void SaveToFile()
    {
        string filePath = experimentData.dirPath + $"/HRConfTiming_PID_{experimentData.participantID}_condition_{experimentData.modOrderString}.csv";
        string fileData = File.ReadAllText(filePath);
        string[] lines = fileData.Split("\n"[0]);
        // get the last row (because length-1 is white space):
        string[] lineData = (lines[lines.Length-2].Trim()).Split(","[0]);
        // find the first instance of confidence placeholder
        indexConf = Array.IndexOf(lineData, "confidence_placeholder");
        // replace that index with the confidence value 
        var sliderVal = slider.value.ToString();
        lineData[indexConf] = sliderVal;
        // rebuild current line string and add it to lines array
        lines[lines.Length-2] = string.Join(",", lineData);
        // write all back to CSV file by creating new CSV file:
        File.WriteAllText(filePath, "");
        for (int i = 0; i < lines.Length; i++)
        {
            if(lines[i] != "")
            {
                File.AppendAllText(filePath, lines[i] + "\n");
            }
          
        }
    }

    public void OnButtonPress()
    {
        SaveToFile();
    }
}

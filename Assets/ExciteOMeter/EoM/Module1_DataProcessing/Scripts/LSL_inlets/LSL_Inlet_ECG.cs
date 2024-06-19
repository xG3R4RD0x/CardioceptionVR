using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ExciteOMeter
{
    public class LSL_Inlet_ECG : InletIntSamples
    {
        private int indexConf;
        public ExperimentData experimentData = null;
        private int currentTrial = -1;
        private bool startRecorded;


        void Awake()
        {
       
        }

        void Start()
        {
            // [optional] call this only, if your gameobject hosting this component
            // got instantiated during runtime
            
            registerAndLookUpStream();
            if (experimentData == null)
            {
                experimentData = FindObjectOfType<ExperimentData>();
            }
        }


        /// <summary>
        /// Override this method to implement whatever should happen with the samples...
        /// IMPORTANT: Avoid heavy processing logic within this method, update a state and use
        /// coroutines for more complexe processing tasks to distribute processing time over
        /// several frames
        /// </summary>
        /// <param name="newSample"></param>
        /// <param name="timeStamp"></param>
        protected  void Process2(int numSamples, int[,] newSamples, double[] timeStamps)
        //protected override void Process(int numSamples, int[,] newSamples, double[] timeStamps)
        {
            ExciteOMeterManager.DebugLog("ECG NumSamples: " + numSamples);

            // pull as long samples are available
            // for (int i = 0; i < numSamples; i++)
            // {
            //     if(newSamples[0,i] > 1.0e7)  // Check values with bad parsing
            //     {
            //         ExciteOMeterManager.DebugLog("Error parsing value: " + BitConverter.ToString(BitConverter.GetBytes(newSamples[0,i])) + ", " + newSamples[0,i].ToString("F2"));
            //         continue;
            //     }
            //     else
            //     {
            //         EoM_Events.Send_OnDataReceived(VariableType, ExciteOMeterManager.GetTimestamp(), newSamples[0,i]);
            //         LoggerController.instance.WriteLine(LogName.VariableRawECG, newSamples[0,i].ToString("F0") + "," + newSamples[0,i].ToString("F0"));
            //         ExciteOMeterManager.DebugLog("Value of new sample: " + BitConverter.ToString(BitConverter.GetBytes(newSamples[0,i])) + ", " + newSamples[0,i].ToString("F2"));
            //     }
            // }

            // Debug.Log($"Receiving from stream {StreamName}, first sample {newSample[0]}");

            //TODO: The event only sends float[], all samples need to be parsed to float
            
            // EoM_Events.Send_OnDataReceived(VariableType, new float[1]{(float) newSample[0]});            
            //LoggerController.instance.WriteLine(LogName.VariableRawECG, timestamp.ToString("F0") + "," + newSample[0].ToString("F0"));
        }

        // In case it is inheriting from InletIntSample instead of InletIntChunk
        //protected void Process2(int[] newSample, double timestamp)
        protected override void Process(int[] newSample, double timestamp)
        {
            //Debug.Log($"Receiving from stream {StreamName}, first sample {newSample[0]}");

            //TODO: The event only sends float[], all samples need to be parsed to float
            
            if(newSample[0] > 1.0e7)  // Check values with bad parsing
            {
                //Debug.Log("Error parsing value ECG: " + BitConverter.ToString(BitConverter.GetBytes(newSample[0])) + ", " + newSample[0].ToString("F2"));
            }
            else
            {
                EoM_Events.Send_OnDataReceived(VariableType, ExciteOMeterManager.GetTimestamp(), newSample[0]);            
                LoggerController.instance.WriteLine(LogName.VariableRawECG, timestamp.ToString("F0") + "," + newSample[0].ToString("F0"));
                // todo delete me
                //Debug.Log("ECG packet : " +newSample + ", " + newSample[0].ToString("F2"));
                SaveToFile(newSample[0].ToString());
                // TODO save the above line to CSV file. FInd good way to pre-emptively store the data with good time stamp (use the same one you use for marking HR changes)
                //Debug.Log("Value of new sample:  " + BitConverter.ToString(BitConverter.GetBytes(newSample[0])) + ", " + newSample[0].ToString("F2"));
            }
        }

        public void SaveToFile( string value)
        {
            if(experimentData != null & experimentData.startOfTrialLog == true)
            {
                string filePath = experimentData.dirPath + $"/RawECGData.csv";
                File.AppendAllText(filePath, $" Start of Trial {experimentData.trialNumber +1} at {DateTimeOffset.Now.ToUnixTimeMilliseconds()} with starting HR of {experimentData.startingHR}\n");
                experimentData.startOfTrialLog = false;
            }
            if (experimentData != null & experimentData.startOfExperiment & !startRecorded)
                {
                    string filePath = experimentData.dirPath + $"/RawECGData.csv";
                    File.AppendAllText(filePath, $" Start of Experiment at {DateTimeOffset.Now.ToUnixTimeMilliseconds()} with starting HR of {experimentData.startingHR}\n");
                    startRecorded = true;
                }
                if (experimentData != null & experimentData.trialNumber != currentTrial & experimentData.endOfPractice)

            {
                currentTrial = experimentData.trialNumber;
                CreateTrialColumn("", currentTrial);
            }

            if(experimentData != null & experimentData.endOfPractice)
            {
                //todo add trial number and timestamp with miliseconds
                string filePath = experimentData.dirPath + $"/RawECGData.csv";
                File.AppendAllText(filePath, $" { DateTimeOffset.Now.ToUnixTimeMilliseconds()}, {value} \n");
                //string fileData = File.ReadAllText(filePath);
                //string[] lines = fileData.Split("\n"[0]);
            }

            if (experimentData != null & experimentData.practiceTrialNumber != currentTrial & !experimentData.endOfPractice)
            {
                currentTrial = experimentData.practiceTrialNumber;
                CreateTrialColumn("Practice", currentTrial);
            }

            if (experimentData != null & !experimentData.endOfPractice)
            {
                //todo add trial number and timestamp with miliseconds
                string filePath = experimentData.dirPath + $"/RawECGData.csv";
                File.AppendAllText(filePath, $" { DateTimeOffset.Now.ToUnixTimeMilliseconds()}, {value} \n");
                //string fileData = File.ReadAllText(filePath);
                //string[] lines = fileData.Split("\n"[0]);
            }
           
        }

        public void CreateTrialColumn(string trialKind, int trial) // creates column header for each trial (todo actually make this work and not just be one column)
            //Plan: at the end of the experiment, find second instance of "trial" and make that a new column until there are no further second instances
        {
            if(trial != 0)
            {
                string filePath = experimentData.dirPath + $"/RawECGData.csv";
                File.AppendAllText(filePath, $"End of {trialKind} Trial {trial} Time, {trialKind} Trial {trial} Values \n");
            }
        }

        //create trial row: trial 1 value
        // create trial row: trial 1 time
        // append all values to list
        // append all timestamps to list
        // when trial num changes, write both rows to file (append one after the other) as text strings joined by commad with new line at the end
    }
}

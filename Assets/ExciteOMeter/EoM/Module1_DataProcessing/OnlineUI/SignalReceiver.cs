using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExciteOMeter;
using System.IO;

public class SignalReceiver : MonoBehaviour
{ 
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {

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
        //Debug.Log("Stream connected: " + type);
    }

    // Method which is called when a new stream is disconnected. The type
    // param indicates which stream has just disconnected
    private void StreamDisconnection(DataType type)
    {
        //Debug.Log("Stream disconnected: " + type);
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
            //Debug.Log("Heart Rate Specific Data Received: " + timestamp + " with a rate of " + value);
        }
         // Check whether we have received a heart rate packet
        if (type == DataType.RawECG) {
            // Print timestamp and value to Unity Log
            Debug.Log("rawECG Data Received: " + timestamp + " " + value);
            //if(value < 1.0e7)
            //{
            //    StreamWriter writer = new StreamWriter(particpantID + "/Data.txt", true);
            //    writer.Write(timestamp.ToString("0.0000") + " " + value + "\n");
            //    writer.Close();
            //}
           
        }
    }
    private void DataReceived2(DataType type, float timestamp, float value)
    {
         // Check whether we have received a heart rate packet
        if (type == DataType.RawECG) {
            // Print timestamp and value to Unity Log
            Debug.Log("rawECG Data Received: " + timestamp + " which as a value of " + value);
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartExperiment : MonoBehaviour
{
    public ExperimentData experimentData = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator LoadExpAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Start Experiment");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // TODO: Change to CSV format
    public void WriteToFile(string filePath, string fileName, string contents)
    {
        StreamWriter writer = new StreamWriter(filePath + fileName, true);
        writer.Write(contents);
        writer.Close();
    }

    public void OnButtonPress(Button button)
    {
        WriteToFile(experimentData.dirPath, experimentData.filePath, $"Restarted Experiment at: {DateTime.Now}");
        StartCoroutine(LoadExpAsyncScene());
    }
}
using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using ExciteOMeter;
using UnityEngine.SceneManagement;
using System;

public class StartExperiment : MonoBehaviour
{
    public ExperimentData experimentData = null;

    private void Awake()
    {
        // this should happen as one of the very first things
        // todo: put a check for if participant file exists already
        if (experimentData == null)
        {
            experimentData = FindObjectOfType<ExperimentData>();
        }
        // write relevant data to file
        WriteToFile(experimentData.filePath, $"Started the experiment at: {DateTime.Now}");
        experimentData.trialNumber = 0;  // initiate trials to 0
        experimentData.startingTimeOfExperiment = Time.time * 1000; // time in milliseconds (will be converted to
                                                               // minutes when written to CSV file, which is why it is stored anyway
    }
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

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Experiment");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // TODO: Change to CSV format
    public void WriteToFile(string filePath, string contents)
    {
        StreamWriter writer = new StreamWriter(filePath, true);
        writer.Write(contents);
        writer.Close();
    }

    public void OnButtonPress()
    {
        experimentData.startOfExperiment = true;
        StartCoroutine(LoadExpAsyncScene());
    }

}

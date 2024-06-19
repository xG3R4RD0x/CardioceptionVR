using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using ExciteOMeter;
using UnityEngine.SceneManagement;
using System;

public class StartExperimentButton : MonoBehaviour
{
    public KeyCode _Key;
    private Button _button;

    // Start is called before the first frame update
    void Awake()
    {
        _button = GetComponent<Button>();
    }
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_Key))
        {
            Debug.Log("button press start");
            StartCoroutine(LoadExpAsyncScene());
        }
    }
}

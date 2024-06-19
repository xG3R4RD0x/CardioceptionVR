using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHidePanel : MonoBehaviour
{
    public GameObject confidence;

    // Start is called before the first frame update
    void Start()
    {
        confidence = GameObject.Find("Confidence Panel");
        if(confidence == null)
        {
            confidence = GameObject.Find("Confidence Panel Practice");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonPress(Button button)
    {
        GameObject.Find("YesNoPanel").SetActive(false);
        confidence.SetActive(true);

    }
}

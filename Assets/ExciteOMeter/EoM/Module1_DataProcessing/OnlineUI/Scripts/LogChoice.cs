using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using DT = System.DateTime;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class LogChoice : MonoBehaviour {
   int n;
   private string someID;
   private string particpantID;
   public GameObject itemToHide;
   public GameObject itemToShow;
   public  GameObject confidence;
   public  GameObject isHeartRate;
   public Text questionText;
   public float sliderValue;
   public int hrTrial = 1;
   public int confTrial = 1;
   // public bool particpantUpdate;
   void Awake () {
   }

   void Start()
   {
      if (GameObject.Find("SliderButtons") != null){
         confidence = GameObject.Find("SliderButtons");
         isHeartRate = GameObject.Find("YesNoButtons");
         Hide(confidence);
      }
   }

   void Update()
   {

   }


   public void Hide(GameObject objectName)
    {
      objectName.SetActive(false);
    }

   public void Show(GameObject objectName)
    {
      objectName.SetActive(true);
    }

    public void ChangeText(){
      questionText=GameObject.Find("Question").GetComponent<Text>();
      questionText.text = "How confident are you in your decision?";
    }

    public void WriteToFile(string someStrings){
      
    }

   public void OnButtonPress(Button button){
      n++;
      ChangeText();
      Hide(isHeartRate);
      Show(confidence);
      if(button.name == "Button_Ok")
      {
         confTrial ++;
         sliderValue = GameObject.Find ("SliderButtons").GetComponent <Slider> ().value;
         StartCoroutine(LoadAsyncScene("Main Experiment")); 
      }
      else if((button.name == "Button_Yes") || (button.name == "Button_No")){
         hrTrial ++;
      }
   }
   IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}



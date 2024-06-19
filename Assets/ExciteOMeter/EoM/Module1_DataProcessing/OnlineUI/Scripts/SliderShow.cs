using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace ExciteOMeter
{
    public class SliderShow : MonoBehaviour
    {
   
   Slider slider;
    
        public float sliderValue;
        public Text VisualDisplay;
    
        private void Start()
        {
            // Debug.Log("Global Instance from another file, SliderShow(): "+GlobalInstance.Instance.particpantID.ToString());
            slider = GetComponent<Slider>();
        }
    
        private void Update()
        {
            sliderValue = slider.value;
            VisualDisplay.text = sliderValue.ToString();
        }
    }
}
    
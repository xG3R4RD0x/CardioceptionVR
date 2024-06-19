using UnityEngine;
using System.Collections;

public class ModifySpeed : MonoBehaviour
{
    public Animator sizeChange;
    //Value from the slider, and it converts to speed level
    float m_MySliderValue;
    // public Animation anim;


    // public int Speed;

    // private Animation anim;
    
    void start()
        {
            // Get a reference to the Animation component on the grenade:
      
        // anim = GetComponent<Animation>();
        Debug.LogError("Speed: ");
         //Get the animator, attached to the GameObject you are intending to animate.
        sizeChange = gameObject.GetComponent<Animator>();

    //     {
    //         state.speed = 10F;
    //     }
        }
    void update()
    {
        // lubDub.Play(scaleUp.name);
        Debug.LogError("Something");
        GameObject heart = GameObject.Find("Cube");
        // foreach (AnimationState state in anim){
        //    state.speed = .9f;
        // }
    }

    void speedUp(int speedInput)
    {
        // lubDub.Play(scaleUp.name);
        // anim=GetComponent<Animation>();
        Debug.LogError("Something Speeds up: " + speedInput);
        // foreach (AnimationState state in sizeChange){
        //    state.speed = .9f;
        // }
    }

     void OnGUI()
    {
        //Create a Label in Game view for the Slider
        GUI.Label(new Rect(0, 25, 40, 60), "Speed");
        //Create a horizontal Slider to control the speed of the Animator. Drag the slider to 1 for normal speed.

        m_MySliderValue = GUI.HorizontalSlider(new Rect(45, 25, 200, 60), m_MySliderValue, 0.0F, 1.0F);
        //Make the speed of the Animator match the Slider value
        sizeChange.speed = m_MySliderValue;
    }
    // public void PrintEvent(string s) 
    // {
    //     Debug.Log("PrintEvent: " + s + " called at: " + Time.time);
    // }
}



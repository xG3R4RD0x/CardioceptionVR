using System.Collections;
using UnityEngine;

public class TriggerScripts : MonoBehaviour
{
    Animator animator;
    // Array with Trigger Names
    //To add a new trigger, we just put the name in the array
    string[] triggers = { "BicepCurlT", "ComboPunchT", "FishingCastT", "GolfChipT", "PunchingT", "LookingT" };

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(ActivateRandomTrigger());
    }

    IEnumerator ActivateRandomTrigger()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // wait 5s
            string randomTrigger = triggers[Random.Range(0, triggers.Length)];
            animator.SetTrigger(randomTrigger); // activates random trigger
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    IEnumerator delayedInactive()
    {
        yield return new WaitForSeconds(.01f);
        gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delayedInactive());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

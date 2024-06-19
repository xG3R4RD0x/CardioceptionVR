using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRQuestionnaireToolkit;

public class QuestionnaireController : MonoBehaviour
{
    private GameObject _vrQuestionnaireToolkit;
    private GenerateQuestionnaire _generateQuestionnaire;

    void Start()
    {
        _vrQuestionnaireToolkit = GameObject.FindGameObjectWithTag("VRQuestionnaireToolkit");
        _generateQuestionnaire = _vrQuestionnaireToolkit.GetComponentInChildren<GenerateQuestionnaire>();
    }

    void ToggleQuestionnaires()
    {
      //_generateQuestionnaire.Questionnaires[0].SetActive(false); // enable questionnaire 1
      //_generateQuestionnaire.Questionnaires[1].SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {

    }
}

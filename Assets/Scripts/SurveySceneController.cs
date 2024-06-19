using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRQuestionnaireToolkit;

public class SurveySceneController : MonoBehaviour
{
    private GameObject _vrQuestionnaireToolkit;
    private GenerateQuestionnaire _generateQuestionnaire;
    private ExportToCSV _exportToCsvScript;
    private GameObject _exportToCsv;
    private bool questionnairesDone;
    public ExperimentData experimentData = null;

    void Awake()
    {
        if (experimentData == null)
        {
            experimentData = FindObjectOfType<ExperimentData>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _exportToCsv = GameObject.FindGameObjectWithTag("ExportToCSV");
        _exportToCsvScript = _exportToCsv.GetComponent<ExportToCSV>();
        _vrQuestionnaireToolkit = GameObject.FindGameObjectWithTag("VRQuestionnaireToolkit");
        _generateQuestionnaire = _vrQuestionnaireToolkit.GetComponentInChildren<GenerateQuestionnaire>();
        _exportToCsvScript.QuestionnaireFinishedEvent.AddListener(FireEvent);
    }

    void ToggleQuestionnaire(int qNum, bool onOrOff)
    {

        _generateQuestionnaire.Questionnaires[qNum].SetActive(onOrOff);
    }
    
    private void FireEvent()
    {
        if (!questionnairesDone)
        {
            ToggleQuestionnaire(0, false);
            ToggleQuestionnaire(1, true);
            questionnairesDone = true;
        }
        else
        {
            experimentData.endOfExperiment = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
 
    }
}

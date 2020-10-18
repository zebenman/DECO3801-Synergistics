using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class CouncilRoomController : MonoBehaviour
{
    public GameObject FocusSelection;
    public GameObject AdvisorOpinions;
    public TextMeshProUGUI ModeChangeText;
    public bool IsFocusSelectActive = true;

    public TextMeshProUGUI[] ActionTexts;
    public TextMeshProUGUI StoryText;

    public ButtonScript ButtonScript;

    public void Start()
    {
        FocusSelection.SetActive(IsFocusSelectActive);
        AdvisorOpinions.SetActive(!IsFocusSelectActive);

        EventData active = GameController.Instance.GetFocusedEvents()[0];

        StoryText.text = active.OutcomeDescriptor;
        
        for (int i = 0; i < ActionTexts.Length; i ++)
        {
            EventSolution solution = active.EventSolutions.Find(x => x.SolutionIndex == i);
            if (solution == null) continue;
            ActionTexts[i].text = solution.ActionDescription;
        }
    }

    public void SwitchUIMode()
    {
        IsFocusSelectActive = !IsFocusSelectActive;
        FocusSelection.SetActive(IsFocusSelectActive);
        AdvisorOpinions.SetActive(!IsFocusSelectActive);

        ModeChangeText.text = IsFocusSelectActive ? "View Advisor Opinions" : "View Possible Actions";
    }

    public void ViewAdvisorOpinion(string advisorType)
    {
        if(!Enum.TryParse(advisorType, out AdvisorType aType))
        {
            Debug.LogError($"Invalid Advisor: {advisorType}");
            return;
        }

        switch (aType)
        {
            case AdvisorType.MILITARY:
                ButtonScript.Btn_change_scene(SceneInformation.MILITARY_BIO);
                break;
            case AdvisorType.SCHOLAR:
                ButtonScript.Btn_change_scene(SceneInformation.SCHOLAR_BIO);
                break;
            case AdvisorType.AGRICULTURAL:
                ButtonScript.Btn_change_scene(SceneInformation.AGRICULTURAL_BIO);
                break;
        }
    }

    public void DetermineSummaryOutcome()
    {
        EventData active = GameController.Instance.GetFocusedEvents()[0];
        switch (active.EventID)
        {
            case 0:
            case 2:
                ButtonScript.Btn_change_scene(SceneInformation.SUMMARY_POSITIVE);
                break;
            case 1:
            case 3:
                ButtonScript.Btn_change_scene(SceneInformation.SUMMARY_NEGATIVE);
                break;
        }
    }
}

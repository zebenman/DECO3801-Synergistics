﻿using System.Collections;
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

    public Canvas Canvas;

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

    [HideInInspector]
    [NonSerialized]
    private AreYouSureScript AreYouSure = null;

    private int bufferedActionOutcome = -1;
    private EventData bufferedEvent = null;

    public void DetermineSummaryOutcome()
    {        
        if(bufferedEvent.IsValidStory)
            ButtonScript.Btn_change_scene(SceneInformation.SUMMARY_POSITIVE);
        else
            ButtonScript.Btn_change_scene(SceneInformation.SUMMARY_NEGATIVE);        
    }

    private void AreYouSureStepZero()
    {
        if (AreYouSure == null)
        {
            AreYouSure = Instantiate(PrefabManager.Instance.AreYouSurePrefab, Canvas.transform).GetComponent<AreYouSureScript>();
            AreYouSure.Setup(new UnityEngine.Events.UnityAction(() => ConfirmFocusSuccess()), new UnityEngine.Events.UnityAction(() => ConfirmFocusDeny()));
        }

        AreYouSure.gameObject.SetActive(true);
    }

    public void ConfirmFocusDeny()
    {
        AreYouSure.gameObject.SetActive(false);
    }

    public void ConfirmFocusSuccess()
    {
        ConfirmFocusDeny();

        // TODO -> Move to summary screen and stuff?
        GameController.Instance.SetLastEventData(bufferedEvent, bufferedActionOutcome);
        DetermineSummaryOutcome();
    }

    public void SelectAction(int action)
    {
        // Store the outcome we selected for later
        bufferedActionOutcome = action;
        bufferedEvent = GameController.Instance.GetFocusedEvents()[0];
        AreYouSureStepZero();
    }
}

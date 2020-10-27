using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class SummaryScreenTransition : MonoBehaviour
{
    public GameObject EventSummary;
    public GameObject DecisionSummary;
    public bool IsEventSumActive = true;

    public TextMeshProUGUI HeaderText;
    public TextMeshProUGUI ActionText;


    // Start is called before the first frame update
    public void Start()
    {
        EventSummary.SetActive(IsEventSumActive);
        DecisionSummary.SetActive(!IsEventSumActive);

        EventData lastEvent = GameController.Instance.LastEvent;
        EventSolution lastSolution = GameController.Instance.LastEventOutcome;

        HeaderText.text = lastEvent.EventSummary;
        ActionText.text = lastSolution.ActionSummary;
    }

    public void TransitionUI()
    {
        EventSummary.SetActive(!IsEventSumActive);
        DecisionSummary.SetActive(IsEventSumActive);
    }
}

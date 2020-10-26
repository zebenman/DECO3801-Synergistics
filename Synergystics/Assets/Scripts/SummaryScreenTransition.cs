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


    // Start is called before the first frame update
    public void Start()
    {
        EventSummary.SetActive(IsEventSumActive);
        DecisionSummary.SetActive(!IsEventSumActive);

        EventData active = GameController.Instance.GetFocusedEvents()[0];
    }

    public void TransitionUI()
    {
        EventSummary.SetActive(!IsEventSumActive);
        DecisionSummary.SetActive(IsEventSumActive);
    }
}

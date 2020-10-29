using UnityEngine;
using TMPro;
using System;

// Controller for council room
public class CouncilRoomController : MonoBehaviour
{
    public GameObject FocusSelection;
    public GameObject AdvisorOpinions;
    public TextMeshProUGUI ModeChangeText;
    public bool IsFocusSelectActive = true;

    // Action and story text components
    public TextMeshProUGUI[] ActionTexts;
    public TextMeshProUGUI StoryText;

    public ButtonScript ButtonScript;

    // Canvas
    public Canvas Canvas;

    public void Start()
    {
        // Enable actions, disable advisor opinions
        FocusSelection.SetActive(IsFocusSelectActive);
        AdvisorOpinions.SetActive(!IsFocusSelectActive);

        // Get active event, and set story text
        EventData active = GameController.Instance.GetFocusedEvents()[0];
        StoryText.text = active.OutcomeDescriptor;
        
        // Insert solution data
        for (int i = 0; i < ActionTexts.Length; i ++)
        {
            EventSolution solution = active.EventSolutions.Find(x => x.SolutionIndex == i);
            if (solution == null) continue;
            ActionTexts[i].text = solution.ActionDescription;
        }
    }

    // Switch between possible actions and advisor opinions
    public void SwitchUIMode()
    {
        IsFocusSelectActive = !IsFocusSelectActive;
        FocusSelection.SetActive(IsFocusSelectActive);
        AdvisorOpinions.SetActive(!IsFocusSelectActive);

        ModeChangeText.text = IsFocusSelectActive ? "View Advisor Opinions" : "View Possible Actions";
    }

    // Change to the appropriate advisor bio
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
			case AdvisorType.ECONOMICS:
                ButtonScript.Btn_change_scene(SceneInformation.ECONOMICS_BIO);
                break;
			case AdvisorType.INTELLIGENCE:
                ButtonScript.Btn_change_scene(SceneInformation.INTEL_BIO);
                break;
			case AdvisorType.FOREIGN:
                ButtonScript.Btn_change_scene(SceneInformation.FOREIGN_BIO);
                break;
        }
    }

    [HideInInspector]
    [NonSerialized]
    private AreYouSureScript AreYouSure = null;

    private int bufferedActionOutcome = -1;
    private EventData bufferedEvent = null;

    // Go to summary screen
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

        GameController.Instance.SetLastEventData(bufferedEvent, bufferedEvent.EventSolutions[bufferedActionOutcome]);
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

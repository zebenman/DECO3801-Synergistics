using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GenericSelectionSceneController : MonoBehaviour
{
    [Serializable]
    public class AdvisorTextMapper
    {
        public AdvisorType AdvisorType;
        public TextMeshProUGUI OpinionText;
        public Text NameText;
        public Image AdvisorImage;
    }

    public string MapSource;
    public Canvas Canvas;
    public TextMeshProUGUI StoryText;
    public TextMeshProUGUI StoryTitle;
    public List<AdvisorTextMapper> AdvisorText;
    public Button ConfirmEventButton;
    public TextMeshProUGUI ConfirmEventText;
    public Button BackButton;
    public TextMeshProUGUI BackButtonText;
    public ButtonScript ButtonScript;

    public bool advisors1_shown;
    public bool advisors2_shown;
    public GameObject AdvisorOpinions1;
    public GameObject AdvisorOpinions2;

    private EventData GetActiveEvent()
    {
        List<EventData> eventSelectionList = GameController.Instance.HasSelectedFocus ? GameController.Instance.GetFocusedEvents() : GameController.Instance.GetPossibleEvents();
        return eventSelectionList.Find(x => x.MapSource.Equals(MapSource));
    }

    public void Start()
    {
        advisors1_shown = true;
        advisors2_shown = false;

        if (Canvas == null)
            Canvas = transform.parent.GetComponent<Canvas>();

        EventData activeEvent = GetActiveEvent();
        if (activeEvent == null) return;

        StoryText.text = GameController.Instance.HasSelectedFocus ? activeEvent.OutcomeDescriptor : activeEvent.StoryDescriptor;
        StoryTitle.text = activeEvent.StoryTitle;
        foreach (AdvisorTextMapper mapper in AdvisorText)
        {
            string textValue = null;
            if(GameController.Instance.HasSelectedFocus)
            {
                textValue = activeEvent.SolutionOpinions.Find(x => x.AdvisorType == mapper.AdvisorType).Opinion;
            } else
            {
                textValue = activeEvent.PreSelectionOpinions.Find(x => x.AdvisorType == mapper.AdvisorType).Opinion;
            }
            mapper.OpinionText.text = textValue;
            mapper.NameText.text = GameController.Instance.GetAdvisors().Find(x => x.GetAdvisorType() == mapper.AdvisorType).GetAdvisorName();
            mapper.AdvisorImage.sprite = GameController.Instance.GetAdvisors().Find(x => x.GetAdvisorType() == mapper.AdvisorType).AdvisorSprite;
        }

        if (GameController.Instance.GetFocusedEvents().Contains(GetActiveEvent()))
        {
            ConfirmEventButton.enabled = false;
            ConfirmEventText.text = "Focus Confirmed";
        }
    }

    [HideInInspector]
    [NonSerialized]
    private AreYouSureScript AreYouSure = null;

    public void ConfirmFocusStepZero()
    {
        if(AreYouSure == null)
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
        GameController.Instance.AddFocusedEvent(GetActiveEvent());
        ConfirmEventButton.enabled = false;
        ConfirmEventText.text = "Focus Confirmed";

        if(GameController.Instance.HasSelectedFocus)
        {
            BackButton.onClick = new Button.ButtonClickedEvent();
            BackButton.onClick.AddListener(new UnityEngine.Events.UnityAction(() => ButtonScript.Btn_change_scene(SceneInformation.THRONE_ROOM)));
            BackButtonText.text = "To Throne Room";
        }
    }

    public void SwitchUIMode()
    {
        advisors1_shown = !advisors1_shown;
        advisors2_shown = !advisors2_shown;
        AdvisorOpinions1.SetActive(advisors1_shown);
        AdvisorOpinions2.SetActive(advisors2_shown);
    }
}

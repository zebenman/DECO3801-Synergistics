using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AdvisorBioController : MonoBehaviour
{
    public AdvisorType AdvisorType;

    public TextMeshProUGUI ShortStoryText;

    public TextMeshProUGUI[] ActionDescriptorText;
    public TextMeshProUGUI[] ActionOpinionText;

    public ButtonScript ButtonScript;

    public TextMeshProUGUI AdvisorName;
    public TextMeshProUGUI AdvisorTypeText;
    public TextMeshProUGUI AdvisorTrait;
    public Image AdvisorImage;


    public void Start()
    {
        EventData active = GameController.Instance.GetFocusedEvents()[0];
        ShortStoryText.text = active.ShortOutcomeDescriptor;

        for(int i = 0; i < ActionDescriptorText.Length; i++)
        {
            ActionDescriptorText[i].text = active.EventSolutions.Find(x => x.SolutionIndex == i).ActionDescription;
            ActionOpinionText[i].text = active.SolutionOpinions.Where(x => x.SolutionIndex == i).ToList().Find(x => x.AdvisorType == AdvisorType).Opinion;
        }
    }

    public void ReturnToCouncil()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync("CouncilRoom").completed += (a) =>
        {
            GameController.Instance.OnSceneTransition("CouncilRoom", currentScene);
            FindObjectOfType<CouncilRoomController>().SwitchUIMode();
        };
    }
}

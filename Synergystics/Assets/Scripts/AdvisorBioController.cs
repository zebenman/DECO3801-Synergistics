using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// Controller for advisor bio scenes
public class AdvisorBioController : MonoBehaviour
{
    // Type for this bio
    public AdvisorType AdvisorType;

    // Shortened story text
    public TextMeshProUGUI ShortStoryText;

    // Arrays of text for action descriptions & opinions
    public TextMeshProUGUI[] ActionDescriptorText;
    public TextMeshProUGUI[] ActionOpinionText;

    public ButtonScript ButtonScript;

    // Advisor data
    public TextMeshProUGUI AdvisorName;
    public TextMeshProUGUI AdvisorTypeText;
    public TextMeshProUGUI AdvisorTrait;
    public Image AdvisorImage;


    public void Start()
    {
        // Load the active event and set the story text
        EventData active = GameController.Instance.GetFocusedEvents()[0];
        ShortStoryText.text = active.ShortOutcomeDescriptor;

        // Load actions & opinion text for each action
        for(int i = 0; i < ActionDescriptorText.Length; i++)
        {
            ActionDescriptorText[i].text = active.EventSolutions.Find(x => x.SolutionIndex == i).ActionDescription;
            ActionOpinionText[i].text = active.SolutionOpinions.Where(x => x.SolutionIndex == i).ToList().Find(x => x.AdvisorType == AdvisorType).Opinion;
        }

        // Grab advisor data and set bio information
        Advisor data = GameController.Instance.GetAdvisors().Find(x => x.GetAdvisorType() == AdvisorType);
        AdvisorName.text = data.GetAdvisorName();
        AdvisorTypeText.text = data.GetAdvisorTypeFancy();
        AdvisorTrait.text = data.GetTraitFancy();
        AdvisorImage.sprite = data.AdvisorSprite;
    }

    // Go back to council room
    public void ReturnToCouncil()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(SceneInformation.COUNCIL_ROOM).completed += (a) =>
        {
            GameController.Instance.OnSceneTransition(SceneInformation.COUNCIL_ROOM, currentScene);
            FindObjectOfType<CouncilRoomController>().SwitchUIMode();
        };
    }
}

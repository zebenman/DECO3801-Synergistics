using UnityEngine;
using UnityEngine.UI;

public class ThroneRoomController : MonoBehaviour
{
    // All the different widgets in the throne room
    public GameObject MapWidget;
    public GameObject CouncilRoomWidget;
    public GameObject LocationWidget;
    public GameObject BackgroundImage;
    public GameObject AdvisorText;
    public GameObject MapText;

    public void Start()
    {
        // Disable council room without a focus selected, Disable map room when a focus is selected
        CouncilRoomWidget.GetComponent<Button>().enabled = GameController.Instance.HasSelectedFocus;
        MapWidget.GetComponent<Button>().enabled = !GameController.Instance.HasSelectedFocus;

        AdvisorText.SetActive(!GameController.Instance.HasSelectedFocus);
        MapText.SetActive(GameController.Instance.HasSelectedFocus);

        float cc = 65 / 255f;
        if (!GameController.Instance.HasSelectedFocus)
        {
            Utilities.SetWidgetColorRecursive(CouncilRoomWidget, new Color(cc, cc, cc), AdvisorText);
        } else
        {
            Utilities.SetWidgetColorRecursive(MapWidget, new Color(cc, cc, cc), MapText);
        }
    }
}

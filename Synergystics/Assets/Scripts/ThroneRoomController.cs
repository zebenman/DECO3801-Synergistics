using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThroneRoomController : MonoBehaviour
{
    public GameObject MapWidget;
    public GameObject CouncilRoomWidget;
    public GameObject LocationWidget;
    public GameObject BackgroundImage;
    public GameObject AdvisorText;
    public GameObject MapText;

    public void Start()
    {
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

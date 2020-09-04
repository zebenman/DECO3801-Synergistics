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

    public void Start()
    {
        CouncilRoomWidget.GetComponent<Button>().enabled = GameController.Instance.HasSelectedFocus;
        AdvisorText.SetActive(!GameController.Instance.HasSelectedFocus);
        if(!GameController.Instance.HasSelectedFocus)
        {
            float cc = 65f / 255f;
            Utilities.SetWidgetColorRecursive(CouncilRoomWidget, new Color(cc, cc, cc), AdvisorText);
        }
    }
}

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapController : MonoBehaviour
{
    public Button FarmWidget;
    public Button TownSquareWidget;
    public Button TownHallWidget;
    public Button MonasteryWidget;

    public void Start()
    {
        List<EventData> eventSelectionList = GameController.Instance.HasSelectedFocus ? GameController.Instance.GetFocusedEvents() : GameController.Instance.GetPossibleEvents();
        List<string> areasToDisable = Utilities.MapSources.Where(x => !eventSelectionList.Any(y => y.MapSource.Equals(x))).ToList();
        foreach(string area in areasToDisable)
        {
            Button rootObject = null;
            switch(area)
            {
                case "FARM":
                    rootObject = FarmWidget;
                    break;
                case "MONASTERY":
                    rootObject = MonasteryWidget;
                    break;
                case "TOWN_SQUARE":
                    rootObject = TownSquareWidget;
                    break;
                case "TOWN_HALL":
                    rootObject = TownHallWidget;
                    break;
            }
            rootObject.enabled = false;
            Utilities.SetWidgetColorRecursive(rootObject.gameObject, Utilities.DarkGrey);
        }
    }
}

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Controller for the map room
public class MapController : MonoBehaviour
{
    // Map locations again
    public enum Locations
    {
        FARM, MONASTERY, MARKET, TAVERN, DUNGEON, BARRACKS, INVALID_LOCATION
    }

    // Map location buttons
    public Button FarmWidget;
    public Button TownMarketWidget;
    public Button TavernWidget;
    public Button MonasteryWidget;
    public Button DungeonWidget;
    public Button BarracksWidget;

    public void Start()
    {
        // Work out what list to select from
        List<EventData> eventSelectionList = GameController.Instance.HasSelectedFocus ? GameController.Instance.GetFocusedEvents() : GameController.Instance.GetPossibleEvents();

        // Disable all areas that don't have a story associated with them
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
                case "TOWN_MARKET":
                    rootObject = TownMarketWidget;
                    break;
                case "TAVERN":
                    rootObject = TavernWidget;
                    break;
                case "DUNGEON":
                    rootObject = DungeonWidget;
                    break;
                case "BARRACKS":
                    rootObject = BarracksWidget;
                    break;
            }
            rootObject.enabled = false;
            Utilities.SetWidgetColorRecursive(rootObject.gameObject, Utilities.DarkGrey);
        }
    }
}

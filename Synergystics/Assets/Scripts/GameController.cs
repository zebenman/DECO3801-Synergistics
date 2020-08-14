using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Controller singleton instance
    public static GameController Instance { get; private set; }
    
    // Dictionary used to substitute information into event and dialogue text
    public readonly Dictionary<string, string> ReplacementMap = new Dictionary<string, string>();

    // Dialogue loader that reads dialogue information from file
    public readonly DialogueLoader DialogugeLoader = new DialogueLoader("Resources\\Dialogue");

    // Flag describing if the user has selected a focus (hidden from Unity inspector)
    [HideInInspector]
    public bool HasSelectedFocus = false;

    // Last dialogue option selected (hidden from Unity inspector)
    [HideInInspector]
    public int LastSelectedOption = -1;

    // Last event selected (hidden from Unity inspector)
    [HideInInspector]
    public GameEvent LastSelectedEvent = null;

    // Maximum number of focuses
    public int MaxFocusSelectionCount = 2;

    // Internal list of possible events that has been buffered so that list order is always the same
    private List<GameEvent> BufferedPossibleEvents;

    // Internal list of focused events
    private List<GameEvent> FocusedEvents;

    private void SelectPossibleEvents()
    {
        BufferedPossibleEvents = new List<GameEvent>()
        {
            new GameEvent(0, "Bugs were spotted in several of our farmers fields"), 
            new GameEvent(1, "Obvious red herring 1"),
            new GameEvent(2, "Obvious red herring 2"),
            new GameEvent(3, "Obvious red herring 3")
        };
    }

    public List<GameEvent> GetPossibleEvents()
    {
        if(BufferedPossibleEvents == null || BufferedPossibleEvents.Count == 0)
        {
            // Implies BufferedPossibleEvents is not null and is not empty
            SelectPossibleEvents();
        }

        return BufferedPossibleEvents;
    }

    public List<GameEvent> GetFocusedEvents()
    {
        if(FocusedEvents == null && HasSelectedFocus)
        {
            Debug.LogError("Focused Event list is null despite focus being selected!");
        }

        if(FocusedEvents != null && FocusedEvents.Count == 0 && HasSelectedFocus)
        {
            Debug.LogError("Focused Event list is empty despite focus being selected!");
        }

        return FocusedEvents == null ? new List<GameEvent>() : FocusedEvents;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Debug stuff
        Debug.Log("Logging replacement map");
        foreach(KeyValuePair<string, string> entry in ReplacementMap)
        {
            Debug.Log($"{entry.Key}, {entry.Value}");
        }
    }
}

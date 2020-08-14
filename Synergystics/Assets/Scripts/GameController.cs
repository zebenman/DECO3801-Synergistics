using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Controller singleton instance
    public static GameController Instance { get; private set; }
    
    // Dictionary used to substitute information into event and dialogue text
    public readonly Dictionary<string, string> ReplacementMap = new Dictionary<string, string>();

    // Set containing all active background flags
    public readonly HashSet<string> BackgroundFlags = new HashSet<string>();

    // Dialogue loader that reads dialogue information from file
    public readonly DialogueLoader DialogugeLoader = new DialogueLoader("Resources\\Dialogue");

    public readonly GameConfig GameConfig = GameConfig.LoadFrom("Resources\\config.json", true);

    // Flag describing if the user has selected a focus (hidden from Unity inspector)
    [HideInInspector]
    [NonSerialized]
    public bool HasSelectedFocus = false;

    // Last dialogue option selected (hidden from Unity inspector)
    [HideInInspector]
    [NonSerialized]
    public int LastSelectedOption = -1;

    // Last event selected (hidden from Unity inspector)
    [HideInInspector]
    [NonSerialized]
    public GameEvent LastSelectedEvent = null;

    // Maximum number of focuses
    public int MaxFocusSelectionCount = 2;

    // Advisor List
    public List<Advisor> Advisors;

    // Internal list of possible events that has been buffered so that list order is always the same
    private List<GameEvent> BufferedPossibleEvents;

    // Internal list of focused events
    private List<GameEvent> FocusedEvents;

    private void SelectPossibleEvents()
    {
        // TODO - Testing
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
            Debug.LogWarning("Focused Event list is null despite focus being selected!");
        }

        if(FocusedEvents != null && FocusedEvents.Count == 0 && HasSelectedFocus)
        {
            Debug.LogWarning("Focused Event list is empty despite focus being selected!");
        }

        // TODO - should do something else?
        if(FocusedEvents == null)
        {
            FocusedEvents = new List<GameEvent>() { GetPossibleEvents()[0] };
        }

        return FocusedEvents;
    }

    // Get the list of advisors
    public List<Advisor> GetAdvisors()
    {
        if(Advisors == null)
        {
            // TODO - should grab advisor objects
            Advisors = new List<Advisor>();
        }

        return Advisors;
    }

    private void Awake()
    {
        // Set singleton instance
        Instance = this;
    }

    private void Start()
    {
        // Will probably use at some point
    }
}

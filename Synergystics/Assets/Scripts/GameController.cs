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
    //public readonly DialogueLoader DialogugeLoader = new DialogueLoader("Resources\\Dialogue");

    // Event loader that reads event data from file
    //public readonly EventLoader EventLoader = new EventLoader("Resources\\Events");

    public readonly DataLoader DataLoader = new DataLoader("Resources\\Events");

    // Config file that reads/writes config information
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
    public EventData LastSelectedEvent = null;

    // Maximum number of focuses
    public int MaxFocusSelectionCount = 1;

    // Advisor List
    public List<Advisor> Advisors;

    // Internal list of possible events that has been buffered so that list order is always the same
    private List<EventData> BufferedPossibleEvents;

    // Internal list of focused events
    private List<EventData> FocusedEvents;

    public EventData LastEvent { get; private set; } = null;
    public int LastEventOutcome { get; private set; } = -1;

    public void SetLastEventData(EventData last, int outcome)
    {
        LastEvent = last;
        LastEventOutcome = outcome;
    }

    private void SelectPossibleEvents()
    {
        BufferedPossibleEvents = new List<EventData>(DataLoader.GetEvents());
    }

    public List<EventData> GetPossibleEvents()
    {
        if(BufferedPossibleEvents == null || BufferedPossibleEvents.Count == 0)
        {
            // Implies BufferedPossibleEvents is not null and is not empty
            SelectPossibleEvents();
        }

        return BufferedPossibleEvents;
    }

    public void AddFocusedEvent(EventData gameEvent)
    {
        if (HasSelectedFocus) return;

        GetFocusedEvents().Add(gameEvent);
        if(GetFocusedEvents().Count == MaxFocusSelectionCount)
        {
            HasSelectedFocus = true;
        }
    }

    public List<EventData> GetFocusedEvents()
    {
        if(FocusedEvents == null && HasSelectedFocus)
        {
            Debug.LogWarning("Focused Event list is null despite focus being selected!");
        }

        if(FocusedEvents != null && FocusedEvents.Count == 0 && HasSelectedFocus)
        {
            Debug.LogWarning("Focused Event list is empty despite focus being selected!");
        }
        
        if(FocusedEvents == null)
        {
            FocusedEvents = new List<EventData>();
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
        DontDestroyOnLoad(this);

        // Set singleton instance
        Instance = this;
    }

    public void OnSceneTransition(string scene)
    {
        // TODO?
    }
}

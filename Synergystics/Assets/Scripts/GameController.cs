using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public List<AdvisorPicture> AdvisorPictures;

    // Controller singleton instance
    public static GameController Instance { get; private set; }
    
    // Dictionary used to substitute information into event and dialogue text
    public readonly Dictionary<string, string> ReplacementMap = new Dictionary<string, string>();

    // Set containing all active background flags
    public readonly HashSet<string> BackgroundFlags = new HashSet<string>();

    // Dialogue loader that reads dialogue information from file
    //public readonly DialogueLoader DialogugeLoader = new DialogueLoader("Resources\\Dialogue");

    // Event loader that reads event data from file (deprecated)
    //public readonly EventLoader EventLoader = new EventLoader("Resources\\Events");

    // Loads most game related data (events mostly)
    public readonly DataLoader DataLoader = new DataLoader("Resources\\Events");

    // Advisor data generator
    public AdvisorDataGenerator AdvisorGenerator { get; private set; }

    // Story manager
    public StoryManager StoryManager { get; private set; }

    [HideInInspector]
    [NonSerialized]
    public StoryManager.StoryThread ActiveThread = null;

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

    // Last event we focused on
    public EventData LastEvent { get; private set; } = null;
    
    // Last solution we picked
    public EventSolution LastEventOutcome { get; private set; } = null;

    // Set last event & solution
    public void SetLastEventData(EventData last, EventSolution outcome)
    {
        LastEvent = last;
        LastEventOutcome = outcome;
    }

    // Find possible events
    private void SelectPossibleEvents()
    {
        ActiveThread = StoryManager.GetNextThread();
        BufferedPossibleEvents = ActiveThread.GetAllEvents();
    }

    // Get a list of possible events, will select new ones if there are none
    public List<EventData> GetPossibleEvents()
    {
        if(BufferedPossibleEvents == null || BufferedPossibleEvents.Count == 0)
        {
            // Implies BufferedPossibleEvents is not null and is not empty
            SelectPossibleEvents();
        }

        return BufferedPossibleEvents;
    }

    // Add an event to the focussed list
    public void AddFocusedEvent(EventData gameEvent)
    {
        if (HasSelectedFocus) return;

        GetFocusedEvents().Add(gameEvent);
        if(GetFocusedEvents().Count == MaxFocusSelectionCount)
        {
            HasSelectedFocus = true;
        }
    }

    // Get a list of focussed events
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

    // Run startup code
    private void Awake()
    {
        DontDestroyOnLoad(this);

        // Set singleton instance
        Instance = this;

        // Load advisor pictures
        AdvisorPictures = new List<AdvisorPicture>(Resources.LoadAll<AdvisorPicture>("Scriptables"));

        // Load story config, and advisor generator
        StoryManager = new StoryManager("Resources\\StoryConfig.json");
        AdvisorGenerator = new AdvisorDataGenerator("Resources\\MaleNames.txt", "Resources\\FemaleNames.txt", AdvisorPictures);
    }

    // Transition scenes, and stuff that can go with it
    public void OnSceneTransition(string to, string from)
    {
        if(to.Equals(SceneInformation.MAIN_MENU))
        {
            Instance = null;
            Destroy(gameObject);
        }

        if (from.Equals(SceneInformation.INTERMISSION_SCREEN))
        {
            HasSelectedFocus = false;
            LastSelectedOption = -1;
            LastSelectedEvent = null;
            LastEvent = null;
            LastEventOutcome = null;
            BufferedPossibleEvents = null;
            FocusedEvents = null;

            /*if (StoryManager.PeekNextThread() == null)
            {
                SceneManager.LoadSceneAsync(SceneInformation.FINAL_SUMMARY).completed += (a) =>
                {
                    OnSceneTransition(SceneInformation.FINAL_SUMMARY, to);
                };
            }*/
        }
    }
}

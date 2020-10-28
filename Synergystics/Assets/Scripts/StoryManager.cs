using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class StoryManager
{
    public class StoryThread
    {
        // A list of all used events (so we don't use duplicates)
        internal static List<EventData> ReadEvents = new List<EventData>();

        // ID used for ordering
        public int ChapterID;

        // Number of real stories in this chapter
        public int RealStoryCount { get; private set; } = 0;

        // Number of filler stories in this chapter
        public int FillerCount { get; private set; } = 0;

        // Locations these stories are restricted to
        public MapController.Locations[] RestrictedLocations { get; private set; } = null;

        // List of events in this chapter
        private List<EventData> AllEvents = null;

        // Is this chapter initialized?
        private bool IsInitialized = false;

        public StoryThread(int realCount, int fillerCount)
        {
            RealStoryCount = realCount;
            FillerCount = fillerCount;                        
        }

        public StoryThread(int realCount, int fillerCount, params MapController.Locations[] restricted) : this(realCount, fillerCount)
        {
            RestrictedLocations = restricted;
        }       

        public StoryThread(int fillerCount, params EventData[] validEvents) : this(fillerCount, validEvents.ToList())
        {

        }

        public StoryThread(int fillerCount, List<EventData> validEvents)
        {
            AllEvents = validEvents;
            FillerCount = fillerCount;
            ReadEvents.AddRange(validEvents);
        }

        public StoryThread(List<EventData> invalidEvents, List<EventData> validEvents)
        {
            AllEvents = new List<EventData>();
            AllEvents.AddRange(invalidEvents);
            AllEvents.AddRange(validEvents);
            ReadEvents.AddRange(AllEvents);
            IsInitialized = true;            
        }

        public StoryThread(List<EventData> allEvents)
        {
            AllEvents = allEvents;
            ReadEvents.AddRange(AllEvents);
            IsInitialized = true;
        }

        // Initialize this chapter
        private void Initialize()
        {
            if (IsInitialized)
                return;

            if (AllEvents == null)
                AllEvents = new List<EventData>();

            // Get a list of possible events, ensuring that they haven't been used already, and they conform to the restricted locations
            List<EventData> possibleEvents = GameController.Instance.DataLoader.GetEvents().Where(x => !ReadEvents.Contains(x) && (RestrictedLocations == null || RestrictedLocations.Length == 0 || RestrictedLocations.Contains(x.MapLocation))).ToList();

            List<EventData> validEvents = possibleEvents.Where(x => x.IsValidStory).ToList();
            List<EventData> invalidEvents = possibleEvents.Where(x => !x.IsValidStory).ToList();

            validEvents = validEvents.SelectRandom(RealStoryCount).ToList();
            invalidEvents = invalidEvents.SelectRandom(FillerCount).ToList();

            AllEvents.AddRange(validEvents);
            AllEvents.AddRange(invalidEvents);

            ReadEvents.AddRange(validEvents);
            ReadEvents.AddRange(invalidEvents);            
        }

        public List<EventData> GetAllEvents()
        {
            Initialize();
            return AllEvents;
        }

        public List<EventData> GetValidEvents()
        {
            Initialize();
            return AllEvents.Where(x => x.IsValidStory).ToList();
        }

        public List<EventData> GetInvalidEvents()
        {
            Initialize();
            return AllEvents.Where(x => !x.IsValidStory).ToList();
        }
    }

    // Queue of story threads
    private Queue<StoryThread> StoryThreads = new Queue<StoryThread>();

    // Create a manger and load a story config from the resource path
    public StoryManager(string resourcePath)
    {
        List<StoryThread> Chapters = new List<StoryThread>();
        List<EventData> AllEvents = GameController.Instance.DataLoader.GetEvents();

        JObject config = JObject.Parse(File.ReadAllText(resourcePath));
        List<JObject> jChap = config["Chapters"].ToObject<List<JObject>>();

        // Create chapters
        foreach(JObject c in jChap)
        {
            int id = c["ChapterID"].ToObject<int>();
            if (c["EventOverride"] != null)
            {
                // If we have an override field then use it to populate the chapter
                List<string> events = c["EventOverride"].ToObject<List<string>>();
                StoryThread thread = new StoryThread(AllEvents.Where(x => events.Contains(x.EventName)).ToList());
                thread.ChapterID = id;
                Chapters.Add(thread);
            } else
            {
                // Create a new chapter based on filler info
                int filler = c["Filler"].ToObject<int>();
                List<string> storyEvents = c["StoryEvents"].ToObject<List<string>>();
                StoryThread thread = new StoryThread(filler, AllEvents.Where(x => storyEvents.Contains(x.EventName)).ToArray());
                thread.ChapterID = id;
                Chapters.Add(thread);
            }           
        }

        // Add the chapter with the smallest id to the queue until we have no more chapters
        while(Chapters.Count != 0)
        {
            StoryThread min = Chapters.Find(x => x.ChapterID == Chapters.Min(y => y.ChapterID));
            StoryThreads.Enqueue(min);
            Chapters.Remove(min);
        }
    }

    public StoryThread GetNextThread()
    {
        return StoryThreads.Count == 0 ? null : StoryThreads.Dequeue();
    }  

    public StoryThread PeekNextThread()
    {        
        return StoryThreads.Count == 0 ? null : StoryThreads.Peek();
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
public class StoryManager
{
    public class StoryThread
    {
        internal static List<EventData> ReadEvents = new List<EventData>();

        public int RealStoryCount { get; private set; } = 0;
        public int FillerCount { get; private set; } = 0;

        public MapController.Locations[] RestrictedLocations { get; private set; } = null;

        private List<EventData> AllEvents = null;
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

        private void Initialize()
        {
            if (IsInitialized)
                return;

            if (AllEvents == null)
                AllEvents = new List<EventData>();

            List<EventData> possibleEvents = GameController.Instance.DataLoader.GetEvents().Where(x => !ReadEvents.Contains(x)).ToList();

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

    private Queue<StoryThread> StoryThreads = new Queue<StoryThread>();

    public StoryManager()
    {
        List<EventData> allEvents = GameController.Instance.DataLoader.GetEvents();

        StoryThread chapter0 = new StoryThread(allEvents.Where(x => !x.IsValidStory && x.StoryTitle.Equals("Missing Sheep")).ToList(), allEvents.Where(x => x.IsValidStory && x.StoryTitle.Equals("Plague")).ToList());
        allEvents = allEvents.Where(x => !StoryThread.ReadEvents.Contains(x)).ToList();
        StoryThread chapter1 = new StoryThread(allEvents.Where(x => !x.IsValidStory).ToList(), allEvents.Where(x => x.IsValidStory).ToList());

        StoryThreads.Enqueue(chapter0);
        StoryThreads.Enqueue(chapter1);
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

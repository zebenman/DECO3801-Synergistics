using System;
using System.Collections.Generic;
using System.Linq;
public class StoryManager
{
    public class StoryThread
    {
        private static List<EventData> ReadEvents = new List<EventData>();

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
}

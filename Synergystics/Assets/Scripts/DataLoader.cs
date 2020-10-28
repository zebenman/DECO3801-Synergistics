using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

// All data for events
public class EventData
{
    public int EventID;
    public string EventName;
    public string MapSource;
    public MapController.Locations MapLocation { get => Enum.TryParse(MapSource, out MapController.Locations loc) ? loc : MapController.Locations.INVALID_LOCATION; }
    public bool IsValidStory;

    public string StoryTitle;

    public string StoryDescriptor;
    public string OutcomeDescriptor;
    public string ShortOutcomeDescriptor;
    public string EventSummary;

    public List<AdvisorPreSelectionOpinion> PreSelectionOpinions;
    public List<AdvisorSolutionOpinion> SolutionOpinions;
    public List<EventSolution> EventSolutions;
}

// Advisor opinion before focus selection
public class AdvisorPreSelectionOpinion
{
    public AdvisorType AdvisorType;
    public string Opinion;
}

// Advisor opinion for a solution
public class AdvisorSolutionOpinion
{
    public AdvisorType AdvisorType;
    public string Opinion;
    public int SolutionIndex;
}

// Event solution data
public class EventSolution
{
    public int SolutionIndex;
    public string ActionDescription;
    public string ActionSummary;
}

// Loads event data
public class DataLoader
{
    // List of all events
    private readonly List<EventData> EventDataList = new List<EventData>();

    // Get a list of all events
    public List<EventData> GetEvents()
    {
        return EventDataList;
    }

    // Load events at a specific path
    public DataLoader(string resourcePath)
    {
        foreach (string path in Directory.GetFiles(resourcePath, "*.json"))
        {
            JObject data = JObject.Parse(File.ReadAllText(path));

            // Grab everything from the top level file
            int eventID = data.GetValue("EventID").ToObject<int>();
            string eventName = data.GetValue("EventName").ToObject<string>();
            string source = data.GetValue("MapSource").ToObject<string>();
            string storyTitle = data.GetValue("StoryTitle").ToObject<string>();
            string storyDescription = data.GetValue("StoryDescriptor").ToObject<string>();
            string outcomeDescription = data.GetValue("OutcomeDescriptor").ToObject<string>();
            string shortOutcomeDescription = data.GetValue("ShortOutcomeDescriptor").ToObject<string>();
            bool isValidStory = data.GetValue("IsValidStory").ToObject<bool>();
            string eventSummary = data.GetValue("EventSummary").ToObject<string>();

            string preSelectionFilePrefix = data.GetValue("PreSelectionPrefix").ToObject<string>();
            string solutionSelectionFilePrefix = data.GetValue("SolutionOpinionPrefix").ToObject<string>();
            string eventSolutionSelectionFilePrefix = data.GetValue("EventSolutionPrefix").ToObject<string>();

            string dataFolder = data.GetValue("DataFolder").ToObject<string>();
            string rPath = $"{resourcePath}\\{dataFolder}";

            EventData eventData = new EventData()
            {
                EventID = eventID,
                EventName = eventName,
                MapSource = source,
                StoryTitle = storyTitle,
                StoryDescriptor = storyDescription,
                OutcomeDescriptor = outcomeDescription,
                ShortOutcomeDescriptor = shortOutcomeDescription,
                IsValidStory = isValidStory,
                EventSummary = eventSummary,
                PreSelectionOpinions = LoadPreSelection(rPath, preSelectionFilePrefix),
                SolutionOpinions = LoadSolutionOpinions(rPath, solutionSelectionFilePrefix),
                EventSolutions = LoadSolutions(rPath, eventSolutionSelectionFilePrefix)
            };

            EventDataList.Add(eventData);
        }
    }

    // Load pre focus selection opinions
    private List<AdvisorPreSelectionOpinion> LoadPreSelection(string dataFolder, string preSelectionPrefix)
    {
        List<AdvisorPreSelectionOpinion> rList = new List<AdvisorPreSelectionOpinion>();

        foreach(string path in Directory.GetFiles(dataFolder, $"{preSelectionPrefix}_*.json"))
        {
            JObject data = JObject.Parse(File.ReadAllText(path));

            // Special case for agricultural advisor (didnt name her correctly in template docs)
            string advisorTypeString = data.GetValue("AdvisorType").ToObject<string>();
            if(advisorTypeString.Equals("AGRICULTURE", StringComparison.OrdinalIgnoreCase))
            {
                // Just change the string so we can parse it
                advisorTypeString = "AGRICULTURAL";
            }
            AdvisorType aType = (AdvisorType)Enum.Parse(typeof(AdvisorType), advisorTypeString, true);
            string opinion = data.GetValue("Opinion").ToObject<string>();

            AdvisorPreSelectionOpinion apso = new AdvisorPreSelectionOpinion()
            {
                AdvisorType = aType,
                Opinion = opinion
            };

            rList.Add(apso);
        }

        return rList;
    }

    // Load advisor solution opinions
    private List<AdvisorSolutionOpinion> LoadSolutionOpinions(string dataFolder, string opinionSelectionPrefix)
    {
        List<AdvisorSolutionOpinion> rList = new List<AdvisorSolutionOpinion>();

        foreach(string path in Directory.GetFiles(dataFolder, $"{opinionSelectionPrefix}_*.json"))
        {
            JObject data = JObject.Parse(File.ReadAllText(path));

            // Another agri advisor fix
            string advisorTypeString = data.GetValue("AdvisorType").ToObject<string>();
            if (advisorTypeString.Equals("AGRICULTURE", StringComparison.OrdinalIgnoreCase))
            {
                advisorTypeString = "AGRICULTURAL";
            }
            AdvisorType aType = (AdvisorType)Enum.Parse(typeof(AdvisorType), advisorTypeString, true);
            List<string> opinions = data.GetValue("SolutionOpinions").ToObject<List<string>>();

            // Create objects for each solution
            foreach((int index, string opinion) in opinions.IndexedForeach())
            {
                AdvisorSolutionOpinion aso = new AdvisorSolutionOpinion()
                {
                    AdvisorType = aType,
                    Opinion = opinion,
                    SolutionIndex = index
                };

                rList.Add(aso);
            }
        }

        return rList;
    }

    // Load solutions
    private List<EventSolution> LoadSolutions(string dataFolder, string solutionSelectionPrefix)
    {
        List<EventSolution> rList = new List<EventSolution>();

        foreach (string path in Directory.GetFiles(dataFolder, $"{solutionSelectionPrefix}_*.json"))
        {
            JObject data = JObject.Parse(File.ReadAllText(path));

            int solutionIndex = data.GetValue("SolutionIndex").ToObject<int>();
            string actionDescription = data.GetValue("ActionDescription").ToObject<string>();
            string actionSummary = data.GetValue("ActionSummary").ToObject<string>();

            EventSolution es = new EventSolution()
            {
                SolutionIndex = solutionIndex,
                ActionDescription = actionDescription,
                ActionSummary = actionSummary
            };

            rList.Add(es);
        }

        return rList;
    }
}

﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

public class AdvisorPreSelectionOpinion
{
    public AdvisorType AdvisorType;
    public string Opinion;
}

public class AdvisorSolutionOpinion
{
    public AdvisorType AdvisorType;
    public string Opinion;
    public int SolutionIndex;
}

public class EventSolution
{
    public int SolutionIndex;
    public string ActionDescription;
    public string ActionSummary;
}

public class DataLoader
{
    private readonly List<EventData> EventDataList = new List<EventData>();

    public List<EventData> GetEvents()
    {
        return EventDataList;
    }

    public DataLoader(string resourcePath)
    {
        foreach (string path in Directory.GetFiles(resourcePath, "*.json"))
        {
            JObject data = JObject.Parse(File.ReadAllText(path));

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

    private List<AdvisorPreSelectionOpinion> LoadPreSelection(string dataFolder, string preSelectionPrefix)
    {
        List<AdvisorPreSelectionOpinion> rList = new List<AdvisorPreSelectionOpinion>();

        foreach(string path in Directory.GetFiles(dataFolder, $"{preSelectionPrefix}_*.json"))
        {
            JObject data = JObject.Parse(File.ReadAllText(path));

            string advisorTypeString = data.GetValue("AdvisorType").ToObject<string>();
            if(advisorTypeString.Equals("AGRICULTURE", StringComparison.OrdinalIgnoreCase))
            {
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

    private List<AdvisorSolutionOpinion> LoadSolutionOpinions(string dataFolder, string opinionSelectionPrefix)
    {
        List<AdvisorSolutionOpinion> rList = new List<AdvisorSolutionOpinion>();

        foreach(string path in Directory.GetFiles(dataFolder, $"{opinionSelectionPrefix}_*.json"))
        {
            JObject data = JObject.Parse(File.ReadAllText(path));

            string advisorTypeString = data.GetValue("AdvisorType").ToObject<string>();
            if (advisorTypeString.Equals("AGRICULTURE", StringComparison.OrdinalIgnoreCase))
            {
                advisorTypeString = "AGRICULTURAL";
            }
            AdvisorType aType = (AdvisorType)Enum.Parse(typeof(AdvisorType), advisorTypeString, true);
            List<string> opinions = data.GetValue("SolutionOpinions").ToObject<List<string>>();

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

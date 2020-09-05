using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class EventLoader
{
    public class EventVariants
    {
        public int EventID;

        public string MapSource;

        public string Descriptor;

        public string FocusSelectedDescriptor;

        public List<EventResponseContainer> Responses;
    }

    public class EventResponseContainer
    {
        public string Descriptor;

        public int OptionIndex;

        public HashSet<string> RequiredFlags;

        public HashSet<string> AppliedFlags;

        public HashSet<string> RemovedFlags;

        public JObject AdditionalData;
    }

    private List<EventVariants> EventVariantList = new List<EventVariants>();

    public EventLoader(string resourcePath)
    {
        foreach (string path in Directory.GetFiles(resourcePath, "*.json"))
        {
            JObject topLevelData = JObject.Parse(File.ReadAllText(path));

            int eventID = topLevelData.GetValue("EventID").ToObject<int>();
            string dataFolder = topLevelData.GetValue("DataFolder").ToObject<string>();
            string eventDescriptor = topLevelData.GetValue("Descriptor").ToObject<string>();
            string mapSource = topLevelData.GetValue("MapSource").ToObject<string>();
            string focusDescriptor = topLevelData.GetValue("FocusSelectedDescriptor").ToObject<string>();

            EventVariants variantData = new EventVariants()
            {
                Descriptor = eventDescriptor,
                FocusSelectedDescriptor = focusDescriptor,
                EventID = eventID,
                Responses = new List<EventResponseContainer>()
            };

            IEnumerable<JObject> optionData = Directory.GetFiles(resourcePath + "\\" + dataFolder, "*.json").Select(x => JObject.Parse(File.ReadAllText(x)));
            foreach(JObject variant in optionData)
            {
                string descriptor = variant.GetValue("Descriptor").ToObject<string>();
                int index = variant.GetValue("OptionIndex").ToObject<int>();
                HashSet<string> requiredFlags = variant.GetValue("RequiredFlags").ToObject<HashSet<string>>();
                HashSet<string> appliedFlags = variant.GetValue("AppliedFlags").ToObject<HashSet<string>>();
                HashSet<string> removedFlags = variant.GetValue("RemovedFlags").ToObject<HashSet<string>>();
                JObject additionalData = variant.GetValue("AdditionalData").ToObject<JObject>();

                EventResponseContainer container = new EventResponseContainer()
                {
                    Descriptor = descriptor,
                    OptionIndex = index,
                    RequiredFlags = requiredFlags,
                    AppliedFlags = appliedFlags,
                    RemovedFlags = removedFlags,
                    AdditionalData = additionalData
                };

                variantData.Responses.Add(container);
            }

            EventVariantList.Add(variantData);
        }
    }
}

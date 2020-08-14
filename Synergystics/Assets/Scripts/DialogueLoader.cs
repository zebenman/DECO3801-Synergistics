using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class DialogueLoader
{
    // A containing class for possible dialogue variants
    public class DialogueVariants
    {
        // Associated event
        public int EventID;
        
        // Flag describing the use of the players focus selection state
        public bool FocusState;
        
        // What advisor this is for (if 'DEFAULT' assume any can use it)
        public AdvisorType ForAdvisor;

        // List of option containers that can be trait matched
        public List<DialogueOptionContainer> Variants;       
    }

    // A container for trait matching dialogue
    public class DialogueOptionContainer
    {
        // Trait weightings for the variant (if empty, assume traits are irrelevant)
        public Advisor.AdvisorTraits TraitWeightings;

        // A set of required flags for this option to be considered (as an option to select)
        // Explicitly used to determine if the DIALOGUE OPTION can be considered (not the response)
        // If this class is in the Response list then this should be empty
        public HashSet<string> RequiredFlags;

        // A set of flags to be added to the game (TODO)
        // Explicitly added when the RESPONSE is used (applied when response is read)
        // If this class is in the Dialogue list then this should be empty
        public HashSet<string> BackgroundFlags;

        // A JObject containing any additional data this container might have (Don't know what it might be yet, hence...)
        public JObject AdditionalData;

        // The variant text
        public string Variant;

        // Returns true if the advisor is valid for this option
        // Game must match the required flags
        // Advisor traits must roll correctly against TraitWeightings? (TODO)
        public bool IsValid(Advisor advisor)
        {
            // All flags must be met to trigger this option
            if(!RequiredFlags.All(x => GameController.Instance.BackgroundFlags.Contains(x)))
            {
                return false;
            }

            // If this has empty weightings, it is the 'default' option
            if(TraitWeightings.Empty())
            {
                return true;
            }

            // TODO
            return true;
        }
    }

    // List of all spoken variants
    private List<DialogueVariants> AllDialogueVariants = new List<DialogueVariants>();

    // List of all response variants
    private List<DialogueVariants> AllResponseVariants = new List<DialogueVariants>();

    public DialogueLoader(string resourcePath)
    {
        // Get and read all the top level files (i.e. BugsInTheField.json), this tells us the data folder location and the pre and post focus prefix for files
        // For each of these top level files, read through their data folder and sift the information into both the DialogueVariant & DialogueOptionContainer class
        // Using the information gathered, determine whether to add it to the dialogue or response variant lists

        foreach (string path in Directory.GetFiles(resourcePath, "*.json"))
        {
            // Get top level data
            JObject topLevelData = JObject.Parse(File.ReadAllText(path));

            // Get all the top-level information (can be made safer by using 'TryGet', but thats a job for later)
            int eventID = topLevelData.GetValue("EventID").ToObject<int>();
            string dataFolder = topLevelData.GetValue("DataFolder").ToObject<string>();
            string prePrefix = topLevelData.GetValue("PreFocusPrefix").ToObject<string>();
            string postPrefix = topLevelData.GetValue("PostFocusPrefix").ToObject<string>();

            // Some one liners to grab all the matching pre/post prefixed files and convert them to json objects
            string[] paths0 = Directory.GetFiles(resourcePath + "\\" + dataFolder, prePrefix + "_*.json");
            string[] paths1 = Directory.GetFiles(resourcePath + "\\" + dataFolder, postPrefix + "_*.json");

            IEnumerable<JObject> preFocus = Directory.GetFiles(resourcePath + "\\" + dataFolder, prePrefix + "_*.json").Select(x => JObject.Parse(File.ReadAllText(x)));
            IEnumerable<JObject> postFocus = Directory.GetFiles(resourcePath + "\\" + dataFolder, postPrefix + "_*.json").Select(x => JObject.Parse(File.ReadAllText(x)));

            // Dictionaries containing variant information mapped against advisor type
            Dictionary<AdvisorType, DialogueVariants> PRE_DialogueVariantData = new Dictionary<AdvisorType, DialogueVariants>();
            Dictionary<AdvisorType, DialogueVariants> PRE_ResponseVariantData = new Dictionary<AdvisorType, DialogueVariants>();

            // Adding pre/post as a bit of a kludge to fix pre/post focus variants from 'sticking' together which caused some trouble when selecting which version to use
            Dictionary<AdvisorType, DialogueVariants> POST_DialogueVariantData = new Dictionary<AdvisorType, DialogueVariants>();
            Dictionary<AdvisorType, DialogueVariants> POST_ResponseVariantData = new Dictionary<AdvisorType, DialogueVariants>();


            // Map data for pre-focus
            foreach (JObject prefoc in preFocus)
            {
                MapDialogueData(prefoc, eventID, PRE_DialogueVariantData, PRE_ResponseVariantData, false);
            }

            // Map data for post-focus
            foreach(JObject posfoc in postFocus)
            {
                MapDialogueData(posfoc, eventID, POST_DialogueVariantData, POST_ResponseVariantData, true);
            }

            // Add info to master lists
            AllDialogueVariants.AddRange(PRE_DialogueVariantData.Values);
            AllResponseVariants.AddRange(PRE_ResponseVariantData.Values);
            AllDialogueVariants.AddRange(POST_DialogueVariantData.Values);
            AllResponseVariants.AddRange(POST_ResponseVariantData.Values);
        }
    }

    // Please forgive this unholy mess
    private void MapDialogueData(JObject pfoc, int eventID, Dictionary<AdvisorType, DialogueVariants> DialogueVariantData, Dictionary<AdvisorType, DialogueVariants> ResponseVariantData, bool focusFlag)
    {
        // Get stuff unsafely
        AdvisorType aType = (AdvisorType)Enum.Parse(typeof(AdvisorType), pfoc.GetValue("AdvisorType").ToObject<string>(), true);
        string question = pfoc.GetValue("Question").ToObject<string>();
        string response = pfoc.GetValue("Response").ToObject<string>();
        Dictionary<string, float> traitWeightings = pfoc.GetValue("TraitWeightings").ToObject<Dictionary<string, float>>();
        HashSet<string> requiredFlags = pfoc.GetValue("RequiredFlags").ToObject<HashSet<string>>();
        HashSet<string> backgroundFlags = pfoc.GetValue("BackgroundFlags").ToObject<HashSet<string>>();
        JObject additionalData = pfoc.GetValue("AdditionalData").ToObject<JObject>();

        // Create dialogue
        DialogueOptionContainer dialogueContainer = new DialogueOptionContainer()
        {
            Variant = question,
            BackgroundFlags = new HashSet<string>(),
            RequiredFlags = requiredFlags,
            TraitWeightings = Advisor.AdvisorTraits.MapFrom(traitWeightings),
            AdditionalData = additionalData
        };

        // Create response
        DialogueOptionContainer responseContainer = new DialogueOptionContainer()
        {
            Variant = response,
            BackgroundFlags = backgroundFlags,
            RequiredFlags = new HashSet<string>(),
            TraitWeightings = Advisor.AdvisorTraits.MapFrom(traitWeightings),
            AdditionalData = additionalData
        };

        // Add to dialogue map
        DialogueVariants foundDialogueVariant = null;
        if (!DialogueVariantData.TryGetValue(aType, out foundDialogueVariant))
        {
            foundDialogueVariant = new DialogueVariants()
            {
                EventID = eventID,
                ForAdvisor = aType,
                FocusState = focusFlag,
                Variants = new List<DialogueOptionContainer>()
            };
            DialogueVariantData.Add(aType, foundDialogueVariant);
        }
        foundDialogueVariant.Variants.Add(dialogueContainer);
        DialogueVariantData[aType] = foundDialogueVariant;

        // Add to response map
        DialogueVariants foundResponseVariant = null;
        if (!ResponseVariantData.TryGetValue(aType, out foundResponseVariant))
        {
            foundResponseVariant = new DialogueVariants()
            {
                EventID = eventID,
                ForAdvisor = aType,
                FocusState = focusFlag,
                Variants = new List<DialogueOptionContainer>()
            };
            ResponseVariantData.Add(aType, foundResponseVariant);
        }
        foundResponseVariant.Variants.Add(responseContainer);
        ResponseVariantData[aType] = foundResponseVariant;
    }

    public string FindDialogue(GameEvent relatedEvent, Advisor advisor, bool isFocusSelected, bool isResponse)
    {
        // Get the list containing what selection we want
        List<DialogueVariants> selection = isResponse ? AllResponseVariants : AllDialogueVariants;

        // Find the variant that matches our criteria
        DialogueVariants variant = selection.Find(x => x.EventID == relatedEvent.EventID && x.FocusState == isFocusSelected && (x.ForAdvisor == AdvisorType.DEFAULT || x.ForAdvisor == advisor.GetAdvisorType()));

        // Return an error if we can't find one 
        if(variant == null)
        {
            return "An error occurred while finding a matching variant";
        }
        
        // Get a 'list' of any matches (i.e. legitimate matching options & default variants)
        IEnumerable<DialogueOptionContainer> finalSelection = variant.Variants.Where(x => x.IsValid(advisor)); 

        // Return an error if there were no matches
        if(finalSelection == null || finalSelection.Count() == 0)
        {
            return "An error occurred while making the final variant selection";
        }

        DialogueOptionContainer endingSelection = null;
        
        if(finalSelection.Count() == 1)
        {
            // If there is only one selection then use it
            endingSelection = finalSelection.First();
        } else
        {
            // If there are multiple selections (i.e. a default & at least one other legitimate trait match), return the trait match
            IEnumerable<DialogueOptionContainer> traitMatchContainers = finalSelection.Where(x => !x.TraitWeightings.Empty());
            
            if(traitMatchContainers.Count() == 0)
            {
                // If we couldn't find a trait match, then use the first default variant
                endingSelection = finalSelection.First();
            } else
            {
                // Use the first trait match variant regardless of how many others there were (may be subject to change?)
                endingSelection = traitMatchContainers.First();
            }
        }

        // Apply the correct flags
        foreach(string flg in endingSelection.BackgroundFlags)
        {
            GameController.Instance.BackgroundFlags.Add(flg);
        }

        // Perform textual substitution
        string sub = endingSelection.Variant;
        foreach(KeyValuePair<string, string> replacements in GameController.Instance.ReplacementMap)
        {
            sub = Regex.Replace(sub, $"\\[{replacements.Key}\\]", replacements.Value);
        }

        // Return finalized string
        return sub;
    }
}

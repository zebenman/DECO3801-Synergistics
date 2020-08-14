using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public enum AdvisorType
{
    DEFAULT, MILITARY, AGRICULTURAL, SCHOLAR
}

public enum AdvisorGender
{
    MALE, FEMALE, OTHER
}

public class Advisor : MonoBehaviour
{
    public class AdvisorTraits
    {
        public static readonly float EmptyValue = -2;

        public readonly float Loyalty;
        public readonly float Ambition;
        public readonly float Stubornness;
        public readonly float Aggressivness;

        public AdvisorTraits(float loyalty, float ambition, float stubornness, float aggressivness)
        {
            Loyalty = loyalty;
            Ambition = ambition;
            Stubornness = stubornness;
            Aggressivness = aggressivness;
        }

        public AdvisorTraits()
        {
            Loyalty = EmptyValue;
            Ambition = EmptyValue;
            Stubornness = EmptyValue;
            Aggressivness = EmptyValue;
        }

        // Generate traits for an advisor type
        public static AdvisorTraits GenerateFor(AdvisorType type)
        {
            // Select random values
            switch(type)
            {
                case AdvisorType.AGRICULTURAL:
                    return GameController.Instance.GameConfig.GetRandom(GameController.Instance.GameConfig.AgriAdvisorMinStats, GameController.Instance.GameConfig.AgriAdvisorMaxStats);
                case AdvisorType.MILITARY:
                    return GameController.Instance.GameConfig.GetRandom(GameController.Instance.GameConfig.MilitaryAdvisorMinStats, GameController.Instance.GameConfig.MilitaryAdvisorMaxStats);
                case AdvisorType.SCHOLAR:
                    return GameController.Instance.GameConfig.GetRandom(GameController.Instance.GameConfig.ScholarAdvisorMinStats, GameController.Instance.GameConfig.ScholarAdvisorMaxStats);
            }

            // Default return value
            return new AdvisorTraits(EmptyValue, EmptyValue, EmptyValue, EmptyValue);
        }

        // Map a dictionary of traits to an actual trait object
        public static AdvisorTraits MapFrom(Dictionary<string, float> traitMap)
        {
            // Get fields
            List<FieldInfo> fields = typeof(AdvisorTraits).GetFields().ToList();

            // Create an object to edit
            AdvisorTraits rObject = new AdvisorTraits();

            foreach(KeyValuePair<string, float> entry in traitMap)
            {
                FieldInfo matchingField = fields.Find(x => x.Name.Equals(entry.Key, StringComparison.InvariantCultureIgnoreCase));
                if(matchingField == null)
                {
                    Debug.LogWarning($"Could not map trait [{entry.Key}] to any existing trait");
                } else
                {
                    if(!IsValidRange(entry.Value))
                    {
                        Debug.LogWarning($"Trait [{entry.Key}] was not mapped because value was out of range [{entry.Value}]");
                    } else
                    {
                        // Set trait value
                        matchingField.SetValue(rObject, entry.Value);
                    }
                }
            }

            return rObject;
        }

        // Returns true if the value is in the valid range (-1 <= value <= 1 or value == -2)
        public static bool IsValidRange(float value)
        {
            return value == -2 || (value >= -1 && value <= 1);
        }

        // Checks each field in the struct to check if its 'empty' (i.e. all values are -2) 
        // Returns true if all values are 'empty', false otherwise
        public bool Empty()
        {
            foreach(FieldInfo field in typeof(AdvisorTraits).GetFields())
            {
                if((float)field.GetValue(this) != EmptyValue)
                {
                    return false;
                } 
            }

            return true;
        }
    }    

    [SerializeField]
    protected AdvisorType advisorType;

    [SerializeField]
    protected string advisorName;

    [SerializeField]
    protected AdvisorGender advisorGender = AdvisorGender.OTHER;

    public AdvisorTraits Traits { get; private set; }

    public AdvisorType GetAdvisorType()
    {
        return advisorType;
    }

    public string GetAdvisorName()
    {
        return advisorName;
    }

    public AdvisorGender GetAdvisorGender()
    {
        return advisorGender;
    }

    public string GetAdvisorGenderedReferal()
    {
        switch(GetAdvisorGender())
        {
            case AdvisorGender.OTHER:
                return "they";
            case AdvisorGender.FEMALE:
                return "she";
            case AdvisorGender.MALE:
                return "he";
        }

        return "Error, gender unspecified";
    }

    // Return a list of dialogue options based on the current game state
    public List<string> GetDialogueOptions()
    {
        List<GameEvent> selectionList = GameController.Instance.HasSelectedFocus ? GameController.Instance.GetFocusedEvents() : GameController.Instance.GetPossibleEvents();
        List<string> lines = new List<string>();
        if(selectionList != null)
        {
            foreach (GameEvent gEvent in selectionList)
            {
                lines.Add(GameController.Instance.DialogugeLoader.FindDialogue(gEvent, this, GameController.Instance.HasSelectedFocus, false));
            }
        } else
        {
            lines.Add("Dialogue Selection List was NULL!");
        }       
        return lines;
    }

    // Return a dialogue response based on what the player has selected
    public string GetDialogueResponse()
    {
        return GameController.Instance.DialogugeLoader.FindDialogue(GameController.Instance.LastSelectedEvent, this, GameController.Instance.HasSelectedFocus, true);
    }

    private void Awake()
    {
        // Register advisor name & type with replacement map
        GameController.Instance.ReplacementMap.Add(advisorType.ToString(), advisorName);

        // Register gendered referal
        GameController.Instance.ReplacementMap.Add($"{advisorType}_GENDERED", GetAdvisorGenderedReferal());

        // Create traits
        Traits = AdvisorTraits.GenerateFor(advisorType);
    }
}

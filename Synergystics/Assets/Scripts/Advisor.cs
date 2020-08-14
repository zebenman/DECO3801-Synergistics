using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum AdvisorType
{
    DEFAULT, MILITARY, AGRICULTURAL, SCHOLAR
}

public class Advisor : MonoBehaviour
{
    public struct AdvisorTraits
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

        public static AdvisorTraits GenerateFor(AdvisorType type)
        {
            // TODO
            return new AdvisorTraits(EmptyValue, EmptyValue, EmptyValue, EmptyValue);
        }

        public static AdvisorTraits MapFrom(Dictionary<string, float> traitMap)
        {
            // TODO
            return new AdvisorTraits(EmptyValue, EmptyValue, EmptyValue, EmptyValue);
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

    public AdvisorTraits Traits { get; private set; }

    public AdvisorType GetAdvisorType()
    {
        return advisorType;
    }

    public string GetAdvisorName()
    {
        return advisorName;
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
        // Create traits
        Traits = AdvisorTraits.GenerateFor(advisorType);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public enum AdvisorType
{
    DEFAULT, MILITARY, AGRICULTURAL, SCHOLAR, ECONOMICS, INTELLIGENCE, FOREIGN
}

public enum AdvisorGender
{
    MALE, FEMALE, OTHER
}

public enum AdvisorTrait
{
    DEFAULT, DIPLOMATIC, WAR_MONGERING, PRESTIGIOUS, CONNIVING, GREEDY, DOWN_TO_EARTH, PERCEPTIVE, SENSIBLE, CONSCIENTIOUS, GULLIBLE, SUBTLE, UPFRONT
}

public class Advisor : MonoBehaviour
{   
    [SerializeField]
    protected AdvisorType advisorType;

    [SerializeField]
    protected string advisorName;

    [SerializeField]
    protected AdvisorGender advisorGender = AdvisorGender.OTHER;

    [SerializeField]
    protected AdvisorTrait advisorTrait = AdvisorTrait.DEFAULT;

    public Sprite AdvisorSprite { get; protected set; } = null;

    public void ProgressTurn()
    {
        
    }

    public AdvisorType GetAdvisorType()
    {
        return advisorType;
    }

    public string GetAdvisorTypeFancy()
    {
        string t = advisorType.ToString().ToLower();
        return $"{t[0].ToString().ToUpper()}{t.Substring(1)} Advisor";
    }

    public string GetAdvisorName()
    {
        return advisorName;
    }

    public AdvisorGender GetAdvisorGender()
    {
        return advisorGender;
    }

    public AdvisorTrait GetTrait()
    {
        return advisorTrait;
    }

    public string GetTraitFancy()
    {
        StringBuilder builder = new StringBuilder();

        string[] split = advisorTrait.ToString().ToLower().Split('_');
        foreach(string s in split)
        {
            builder.Append(s[0].ToString().ToUpper()).Append(s.Substring(1)).Append(' ');
        }

        return builder.ToString();
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

    private void Awake()
    {
        (advisorName, advisorTrait, advisorGender, AdvisorSprite) = GameController.Instance.AdvisorGenerator.GetRandomAdvisorData(advisorType);

        // Register advisor name & type with replacement map
        GameController.Instance.ReplacementMap.Add(advisorType.ToString(), advisorName);

        // Register gendered referal
        GameController.Instance.ReplacementMap.Add($"{advisorType}_GENDERED", GetAdvisorGenderedReferal());      

        // Immediately roll
        ProgressTurn();
    }
}

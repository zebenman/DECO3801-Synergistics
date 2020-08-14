using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class GameConfig
{
    public static GameConfig LoadFrom(string resourcePath, bool replace = false)
    {
        GameConfig config = null;

        // If the file does not exist, or we want to replace it; generate a new config and write it to disk
        if(!File.Exists(resourcePath) || replace)
        {
            config = new GameConfig();
            File.WriteAllText(resourcePath, JsonConvert.SerializeObject(config, Formatting.Indented));            
        } else
        {
            // Load the config if it exists
            config = JsonConvert.DeserializeObject<GameConfig>(File.ReadAllText(resourcePath));
        }

        return config;
    }

    // Gets a random trait based on the min and max
    // This method should never have to be changed even if there are more traits
    public Advisor.AdvisorTraits GetRandom(Advisor.AdvisorTraits min, Advisor.AdvisorTraits max)
    {
        // Create return object
        Advisor.AdvisorTraits rTraits = new Advisor.AdvisorTraits();

        // Go through each field and do stuff
        FieldInfo[] fields = typeof(Advisor.AdvisorTraits).GetFields();
        foreach (FieldInfo fInfo in fields)
        {
            // Get the min range
            float minRange = (float)fInfo.GetValue(min);

            // Get the max range
            float maxRange = (float)fInfo.GetValue(max);

            // Get the random value in that range and set the return objects value
            float randomRange = UnityEngine.Random.Range(minRange, maxRange);
            fInfo.SetValue(rTraits, randomRange);
        }

        // Return traits
        return rTraits;
    }

    // Military Advisor min/max traits
    public Advisor.AdvisorTraits MilitaryAdvisorMinStats = new Advisor.AdvisorTraits(0, 0, 0.2f, -0.1f);
    public Advisor.AdvisorTraits MilitaryAdvisorMaxStats = new Advisor.AdvisorTraits(1, 1, 0.9f, 1);

    // Agricultural Advisor min/max traits
    public Advisor.AdvisorTraits AgriAdvisorMinStats = new Advisor.AdvisorTraits(0, -0.5f, -1, -1);
    public Advisor.AdvisorTraits AgriAdvisorMaxStats = new Advisor.AdvisorTraits(1, 0.5f, 0.2f, 0.2f);

    // Scholar min/max traits
    public Advisor.AdvisorTraits ScholarAdvisorMinStats = new Advisor.AdvisorTraits(-0.2f, 0.1f, 0.3f, -0.1f);
    public Advisor.AdvisorTraits ScholarAdvisorMaxStats = new Advisor.AdvisorTraits(1, 1, 1, 0.5f);
}
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdvisorDataGenerator
{
    // List of names and images
    private List<string> FemaleNames = new List<string>();
    private List<string> MaleNames = new List<string>();
    private List<AdvisorPicture> AdvisorImages;

    // Setup generator
    public AdvisorDataGenerator(string maleNames, string femaleNames, List<AdvisorPicture> images)
    {
        FemaleNames.AddRange(File.ReadAllLines(femaleNames));
        MaleNames.AddRange(File.ReadLines(maleNames));
        AdvisorImages = images;
    }

    public (string name, AdvisorTrait trait, AdvisorGender gender, Sprite image) GetRandomAdvisorData(AdvisorType type)
    {
        // Pick a gender with a 50/50 split
        AdvisorGender gender = Random.Range(0, 2) == 0 ? AdvisorGender.MALE : AdvisorGender.FEMALE;

        // Select a random name that matches the gender and remove it from the list
        List<string> nameList = gender == AdvisorGender.MALE ? MaleNames : FemaleNames;
        string name = nameList[Random.Range(0, nameList.Count)];
        nameList.Remove(name);

        // Get both traits, select them 50/50
        (AdvisorTrait a, AdvisorTrait b) = type.GetTraitByType();
        AdvisorTrait trait = Random.Range(0, 2) == 0 ? a : b;

        // Select an image based on gender
        List<Sprite> possibleImages = AdvisorImages.Where(x => x.AdvisorType == type && (x.Gender == AdvisorGender.OTHER || x.Gender == gender)).Select(x => x.AdvisorImage).ToList();
        Sprite image = possibleImages[Random.Range(0, possibleImages.Count)];

        // Return data
        return (name, trait, gender, image);
    }
}
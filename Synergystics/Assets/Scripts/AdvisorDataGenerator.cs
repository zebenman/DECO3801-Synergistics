using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AdvisorPicture", menuName = "Scriptable/AdvisorPicture", order = 0)]
public class AdvisorPicture : ScriptableObject
{
    public Sprite AdvisorImage;
    public AdvisorType AdvisorType;
    public AdvisorGender Gender;
}

public class AdvisorDataGenerator
{
    private List<string> FemaleNames = new List<string>();
    private List<string> MaleNames = new List<string>();
    private List<AdvisorPicture> AdvisorImages;

    public AdvisorDataGenerator(string maleNames, string femaleNames, List<AdvisorPicture> images)
    {
        FemaleNames.AddRange(File.ReadAllLines(femaleNames));
        MaleNames.AddRange(File.ReadLines(maleNames));
        AdvisorImages = images;
    }

    public (string name, AdvisorTrait trait, AdvisorGender gender, Sprite image) GetRandomAdvisorData(AdvisorType type)
    {
        AdvisorGender gender = Random.Range(0, 2) == 0 ? AdvisorGender.MALE : AdvisorGender.FEMALE;

        List<string> nameList = gender == AdvisorGender.MALE ? MaleNames : FemaleNames;
        string name = nameList[Random.Range(0, nameList.Count)];
        nameList.Remove(name);

        (AdvisorTrait a, AdvisorTrait b) = type.GetTraitByType();
        AdvisorTrait trait = Random.Range(0, 2) == 0 ? a : b;

        List<Sprite> possibleImages = AdvisorImages.Where(x => x.AdvisorType == type && (x.Gender == AdvisorGender.OTHER || x.Gender == gender)).Select(x => x.AdvisorImage).ToList();
        Sprite image = possibleImages[Random.Range(0, possibleImages.Count)];

        return (name, trait, gender, image);
    }
}
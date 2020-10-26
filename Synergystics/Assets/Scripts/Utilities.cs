using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities
{
    public static readonly Color DarkGrey = new Color(66f / 255f, 66f / 255f, 66f / 255f);
    public static readonly string[] MapSources = { "FARM", "TOWN_MARKET", "MONASTERY", "TAVERN", "BARRACKS", "DUNGEON" };
    public static void SetWidgetColorRecursive(GameObject root, Color color, params GameObject[] ignore)
    {
        if (ignore.Contains(root))
        {
            return;
        }

        SetWidgetColor(root, color);

        for (int i = 0; i < root.transform.childCount; i++)
        {
            SetWidgetColorRecursive(root.transform.GetChild(i).gameObject, color, ignore);
        }
    }

    public static void SetWidgetColor(GameObject root, Color color)
    {
        Image rootImage = root.GetComponent<Image>();
        TextMeshProUGUI rootText = root.GetComponent<TextMeshProUGUI>();
        if (rootImage != null)
        {
            rootImage.color = color;
        }

        if (rootText != null)
        {
            rootText.color = color;
        }
    }

    public static IEnumerable<(int index, T value)> IndexedForeach<T>(this IEnumerable<T> self)
    {
        for(int i = 0; i < self.Count(); i++)
        {
            yield return (i, self.ElementAt(i));
        }
    }

    public static IEnumerable<T> SelectRandom<T>(this IEnumerable<T> self, int count)
    {
        if (count <= 0)
            return null;
        List<int> indices = new List<int>();
        count = Mathf.Min(count, self.Count());
        for(int i = 0; i < count; i++)
        {
            int nextValue;
            do
            {
                nextValue = UnityEngine.Random.Range(0, self.Count());
            } while (indices.Contains(nextValue));
            indices.Add(nextValue);
        }
        return indices.Select(x => self.ElementAt(x));
    }

    public static (AdvisorTrait a, AdvisorTrait b) GetTraitByType(this AdvisorType type)
    {
        switch(type)
        {
            case AdvisorType.MILITARY:
                return (AdvisorTrait.DIPLOMATIC, AdvisorTrait.WAR_MONGERING);
            case AdvisorType.SCHOLAR:
                return (AdvisorTrait.PRESTIGIOUS, AdvisorTrait.CONNIVING);
            case AdvisorType.AGRICULTURAL:
                return (AdvisorTrait.GREEDY, AdvisorTrait.DOWN_TO_EARTH);
            case AdvisorType.ECONOMICS:
                return (AdvisorTrait.PERCEPTIVE, AdvisorTrait.SENSIBLE);
            case AdvisorType.INTELLIGENCE:
                return (AdvisorTrait.CONSCIENTIOUS, AdvisorTrait.GULLIBLE);
            case AdvisorType.FOREIGN:
                return (AdvisorTrait.SUBTLE, AdvisorTrait.UPFRONT);                
        }

        throw new ArgumentException("Illegal advisor type");
    }
}

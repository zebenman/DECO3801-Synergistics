using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities
{
    public static readonly Color DarkGrey = new Color(66f / 255f, 66f / 255f, 66f / 255f);
    public static readonly string[] MapSources = { "FARM", "TOWN_HALL", "TOWN_SQUARE", "MONASTERY" };
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
}

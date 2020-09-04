using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities
{
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
}

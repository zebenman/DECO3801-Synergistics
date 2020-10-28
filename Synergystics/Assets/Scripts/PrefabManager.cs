using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class to hold prefabs to be used
public sealed class PrefabManager : MonoBehaviour
{
    public GameObject AreYouSurePrefab;

    // Class stuff

    public static PrefabManager Instance;
    public void Awake()
    {
        Instance = this;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

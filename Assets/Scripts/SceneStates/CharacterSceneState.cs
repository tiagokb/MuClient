using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSceneState : MonoBehaviour
{
    public static CharacterSceneState Instance { get; private set; }

    public Dictionary<string, CharacterList> CharacterList = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
}
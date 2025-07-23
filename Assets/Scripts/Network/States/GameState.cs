using System;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public HandshakeInfo CurrentHandshake { get; set; }

    public List<ServerInfo> AvailableServers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
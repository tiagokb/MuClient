using System;

public static class GameEvents
{
    public static Action<GameServerInfo> OnConnectionInfoReceived;
    public static Action OnGameServerEntered;
}
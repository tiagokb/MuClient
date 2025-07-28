using System;

public static class GameEvents
{
    public static Action<GameServerInfo> OnConnectionInfoReceived;
    public static Action OnGameServerEntered;
    public static Action OnLoginAttempt;
    public static Action<LoginAttemptResponseEnum> OnLoginAttemptResponse;
    public static Action OnCharacterListLoaded;
}
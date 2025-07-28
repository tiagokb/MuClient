using System;
using System.Collections.Concurrent;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher _instance;

    private static readonly ConcurrentQueue<Action> ExecutionQueue = new();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Enqueue(Action action)
    {
        ExecutionQueue.Enqueue(action);
    }

    void Update()
    {
        while (ExecutionQueue.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }
}
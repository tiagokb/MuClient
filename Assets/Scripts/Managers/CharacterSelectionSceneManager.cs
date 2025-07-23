using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectionSceneManager : MonoBehaviour
{
    public static CharacterSelectionSceneManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        var definition =
            PacketDefinitionsRegistry.Instance.GetDefinition(0xF3, 0x00,
                PacketDefinitionsRegistry.PacketType.ClientToServer);

        var builder = new PacketBuilder(definition);

        builder.SetField("Language", 0);
        PacketSender.SendAsync(builder);
    }
}
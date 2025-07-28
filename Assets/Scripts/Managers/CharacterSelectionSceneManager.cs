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
        CheckCharacterList();
    }

    public void CheckCharacterList()
    {
        var definition =
            PacketDefinitionsRegistry.Instance.GetDefinition(0xF3, 0x00,
                PacketDefinitionsRegistry.PacketType.ClientToServer);

        var builder = new PacketBuilder(definition);
        builder.SetField("Language", 0);
        PacketSender.Send(builder);
    }

    public void OnDeleteCharacterClicked()
    {
        CharacterSelectionUiRoot.Instance.OpenDeleteCharacterModel();
    }

    public void OnCancelDeleteCharacter()
    {
        CharacterSelectionUiRoot.Instance.CloseDeleteCharacterModal();
    }

    public void OnDeleteCharacterConfirmed(string securityCode)
    {
        if (SelectedCharacterHolder.Instance.SelectedCharacter != null)
        {
            Debug.Log(
                $"[OnDeleteCharacterClicked] deleting: {SelectedCharacterHolder.Instance.SelectedCharacter?.Name}");

            DeleteCharacter deleteCharacterSender =
                new(SelectedCharacterHolder.Instance.SelectedCharacter?.Name, securityCode);

            _ = LoadingScreen.Instance.ShowStep("Verificando exclusão do personagem...");

            deleteCharacterSender.SendPacket();
        }
        else
        {
            Debug.Log(
                $"[OnDeleteCharacterClicked] Trying to delete a character with no selected character");
        }
    }
}
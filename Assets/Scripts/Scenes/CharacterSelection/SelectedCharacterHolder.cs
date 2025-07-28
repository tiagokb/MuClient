using System;
using TMPro;
using UnityEngine;

public class SelectedCharacterHolder : MonoBehaviour
{
    public static SelectedCharacterHolder Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI nameText;
    public CharacterList? SelectedCharacter { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SelectCharacter(CharacterList character)
    {
        SelectedCharacter = character;
        SetCharacterInfo(character.Name, character.Level);
        CharacterSelectionEvents.OnCharacterSelected?.Invoke(character);
    }

    public void DeselectCharacter()
    {
        SelectedCharacter = null;
        SetCharacterInfo("", 0);
    }

    private void SetCharacterInfo(string characterName, uint level)
    {
        nameText.text = $"Lv. {level} - {characterName}";
    }
}
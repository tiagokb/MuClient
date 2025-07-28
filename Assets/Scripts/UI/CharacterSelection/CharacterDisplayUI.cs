using TMPro;
using UnityEngine;

public class CharacterDisplayUI : MonoBehaviour
{
    private int _slotIndex;
    private string _name;
    private uint _level;
    private CharacterStatus _status;
    private bool _isItemBlockActive;
    private byte[] _appearance;
    private GuildMemberRole _guildPosition;

    private CharacterList _character;

    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI textLevel;
    [SerializeField] private TextMeshProUGUI textStatus;
    [SerializeField] private TextMeshProUGUI textGuildPosition;

    public void OnClick()
    {
        SelectedCharacterHolder.Instance.SelectCharacter(_character);
    }

    public void Setup(CharacterList character) // Recebe um unico character, isso n é uma lista
    {
        _character = character;

        textName.text = character.Name;
        textLevel.text = character.Level.ToString();
        textStatus.text = character.Status.ToString();
        textGuildPosition.text = character.GuildPosition.ToString();
    }
}
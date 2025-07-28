using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCharacterModal : MonoBehaviour
{
    [SerializeField] private TMP_InputField securityCodeInputField;

    [SerializeField] private Button deleteButton;

    private void OnEnable()
    {
        securityCodeInputField.text = string.Empty;
        deleteButton.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (securityCodeInputField.text == string.Empty || securityCodeInputField.text.Length < 1)
        {
            return;
        }

        CharacterSelectionSceneManager.Instance.OnDeleteCharacterConfirmed(securityCodeInputField.text);
    }
}
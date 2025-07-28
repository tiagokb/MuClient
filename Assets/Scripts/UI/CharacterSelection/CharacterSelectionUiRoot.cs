using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionUiRoot : MonoBehaviour
{
    public static CharacterSelectionUiRoot Instance { get; private set; }

    [SerializeField] private GameObject characterListGameObject;

    [SerializeField] private Button deleteButton;
    [SerializeField] private Button createButton;
    [SerializeField] private Button enterWorldButton;

    [SerializeField] private GameObject characterDisplayPrefab;

    [SerializeField] private GameObject deleteCharacterModal;

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
        if (deleteButton)
            deleteButton.interactable = false;

        if (deleteButton)
            createButton.interactable = false;

        if (deleteButton)
            enterWorldButton.interactable = false;

        if (deleteCharacterModal)
            deleteCharacterModal.SetActive(false);

        LoadingScreen.Instance.Hide();
    }

    private void OnEnable()
    {
        GameEvents.OnCharacterListLoaded += CharacterListLoadedHandler;
        CharacterSelectionEvents.OnCharacterSelected += CharacterSelectedHandler;
        CharacterSelectionEvents.OnCharacterDeleted += OnCharacterDeletedHandler;
        CharacterSelectionEvents.OnCharacterDeleteStatusReturn += OnCharacterDeleteStatusReturnHandler;
    }

    private void OnDisable()
    {
        GameEvents.OnCharacterListLoaded -= CharacterListLoadedHandler;
        CharacterSelectionEvents.OnCharacterSelected -= CharacterSelectedHandler;
        CharacterSelectionEvents.OnCharacterDeleted -= OnCharacterDeletedHandler;
        CharacterSelectionEvents.OnCharacterDeleteStatusReturn -= OnCharacterDeleteStatusReturnHandler;
    }

    private void OnCharacterDeleteStatusReturnHandler(CharacterDeleteResult characterDeleteResult)
    {
        switch (characterDeleteResult)
        {
            case CharacterDeleteResult.Successful:
                CharacterSelectionSceneManager.Instance.CheckCharacterList();
                StartCoroutine(ShowMessageAndHide("Personagem Excluído com sucesso!", 3f));
                break;
            case CharacterDeleteResult.Unsuccessful:
                CloseDeleteCharacterModal();
                StartCoroutine(ShowMessageAndHide("Não foi possível excluir o personagem!", 3f));
                break;
            case CharacterDeleteResult.WrongSecurityCode:
                StartCoroutine(ShowMessageAndHide("Código de segurança incorreto!", 3f));
                break;
        }
    }

    private IEnumerator ShowMessageAndHide(string message, float delaySeconds)
    {
        _ = LoadingScreen.Instance.ShowStep(message);
        yield return new WaitForSeconds(delaySeconds);
        LoadingScreen.Instance.Hide();
    }

    private void CharacterListLoadedHandler()
    {
        foreach (Transform child in characterListGameObject.transform)
            Destroy(child.gameObject); // limpa antes de recriar

        foreach (var entry in CharacterSceneState.Instance.CharacterList)
        {
            var character = entry.Value;

            var go = Instantiate(characterDisplayPrefab, characterListGameObject.transform);
            // Aqui você pode ter um componente tipo "CharacterDisplayUI"
            // que recebe o struct e exibe os dados (nome, nível, etc)
            go.GetComponent<CharacterDisplayUI>().Setup(character);

            Debug.Log($"[CharacterSelectionUiRoot] Instanciado personagem: {character.Name}");
        }

        if (CharacterSceneState.Instance.CharacterList.Count < 5)
        {
            createButton.interactable = true;
        }
        else
        {
            createButton.interactable = false;
        }
    }

    private void CharacterSelectedHandler(CharacterList character)
    {
        deleteButton.interactable = true;

        if (character.Status != CharacterStatus.Banned)
        {
            enterWorldButton.interactable = true;
        }
        else
        {
            enterWorldButton.interactable = false;
        }
    }

    private void OnCharacterDeletedHandler()
    {
        deleteButton.interactable = false;
    }

    public void OpenDeleteCharacterModel()
    {
        deleteCharacterModal.SetActive(true);
    }

    public void CloseDeleteCharacterModal()
    {
        deleteCharacterModal.SetActive(false);
    }
}
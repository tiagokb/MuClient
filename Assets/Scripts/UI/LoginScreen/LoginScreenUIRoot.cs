using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScreenUIRoot : MonoBehaviour
{
    public static LoginScreenUIRoot Instance { get; private set; }

    [SerializeField] private GameObject serverListPanel;
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private TextMeshProUGUI errorsText;

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
        GameEvents.OnGameServerEntered += HandleGameServerEntered;
        GameEvents.OnLoginAttempt += HandleLoginAttemptSend;
        GameEvents.OnLoginAttemptResponse += HandleLoginAttemptResponse;
        HideUI();
        ShowServerList();
    }

    private void OnDestroy()
    {
        GameEvents.OnGameServerEntered -= HandleGameServerEntered;
    }

    private void HandleLoginAttemptSend()
    {
        HideUI();
        _ = LoadingScreen.Instance.ShowStep("Autenticando...");
    }

    private void HandleLoginAttemptResponse(LoginAttemptResponseEnum responseCode)
    {
        if (responseCode == LoginAttemptResponseEnum.ConnectionClosed3Fails)
        {
            NetworkConnection.Instance.QuitGame();
            return;
        }

        if (responseCode != LoginAttemptResponseEnum.Okay)
        {
            ShowLoginForm(responseCode.ToString());
            return;
        }

        _ = LoadingScreen.Instance.ShowStep("Login Efetuado com sucesso!");
        SceneManager.LoadScene("CharacterSelection");
    }

    private void HandleGameServerEntered()
    {
        LoadingScreen.Instance.Hide();
        ShowLoginForm();
    }

    public void ShowServerList()
    {
        serverListPanel.SetActive(true);
        loginPanel.SetActive(false);
    }

    public void ShowLoginForm(string withErrors = null)
    {
        serverListPanel.SetActive(false);
        loginPanel.SetActive(true);

        if (withErrors != null)
        {
            errorsText.gameObject.SetActive(true);
            errorsText.text = withErrors;
        }
        else
        {
            if (errorsText.gameObject)
            {
                errorsText.gameObject.SetActive(false);
            }
        }
    }

    public void HideUI()
    {
        if (serverListPanel)
        {
            serverListPanel.SetActive(false);
        }

        if (loginPanel)
        {
            loginPanel.SetActive(false);
        }
    }
}
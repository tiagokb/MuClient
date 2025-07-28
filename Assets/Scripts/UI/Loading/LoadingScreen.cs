using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }

    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI messageText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            loadingPanel.SetActive(false);
        }
    }

    public async Task ShowStep(string message, float progress = -1)
    {
        loadingPanel.SetActive(true);
        messageText.text = message;
        if (progress >= 0)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = progress;
        }
        else
        {
            progressBar.gameObject.SetActive(false);
        }

        await Task.Delay(10);
    }

    public void Hide()
    {
        loadingPanel.SetActive(false);
    }
}
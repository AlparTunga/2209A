using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeToMenu : MonoBehaviour
{
    public string mainMenuSceneName = "Menu";
    public GameObject infoPanel;

    private bool isPanelActive = false;

    void Start()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPanelActive)
            {
                ShowInfoPanel();
            }
            else
            {
                HideInfoPanel();
            }
        }
    }

    public void ShowInfoPanel()
    {
        infoPanel.SetActive(true);
        isPanelActive = true;
    }

    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);
        isPanelActive = false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

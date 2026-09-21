using UnityEngine;
using UnityEngine.UI;

// The pause button in the HUD and the pause panel with Resume and Main Menu. The Escape key is handled by
// GameManager, so both ways of pausing end up in the same place.
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _menuButton;

    private void OnEnable()
    {
        GameManager.GamePaused += ShowPanel;
        GameManager.GameResumed += HidePanel;
        _pauseButton.onClick.AddListener(Pause);
        _resumeButton.onClick.AddListener(Resume);
        _menuButton.onClick.AddListener(ReturnToMenu);
    }

    private void OnDisable()
    {
        GameManager.GamePaused -= ShowPanel;
        GameManager.GameResumed -= HidePanel;
        _pauseButton.onClick.RemoveListener(Pause);
        _resumeButton.onClick.RemoveListener(Resume);
        _menuButton.onClick.RemoveListener(ReturnToMenu);
    }

    private void Start()
    {
        _pausePanel.SetActive(false);
    }

    private void ShowPanel()
    {
        _pausePanel.SetActive(true);
    }

    private void HidePanel()
    {
        _pausePanel.SetActive(false);
    }

    private void Pause()
    {
        GameManager.Instance.Pause();
    }

    private void Resume()
    {
        GameManager.Instance.Resume();
    }

    private void ReturnToMenu()
    {
        GameManager.Instance.ReturnToMenu();
    }
}

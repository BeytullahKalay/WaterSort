using UnityEngine;

public class CanvasController : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.LevelCompleted += OpenLevelCompletePanel;
        EventManager.LoadNextLevel += CloseLevelCompetePanel;
    }

    private void OnDisable()
    {
        EventManager.LevelCompleted -= OpenLevelCompletePanel;
        EventManager.LoadNextLevel -= CloseLevelCompetePanel;
    }


    [Header("Panels")]
    [SerializeField] private GameObject levelCompletedPanel;
    
    private void OpenLevelCompletePanel()
    {
        print("LevelCompleted");
        levelCompletedPanel.SetActive(true);
    }

    private void CloseLevelCompetePanel()
    {
        levelCompletedPanel.SetActive(false);
    }

    public void NextLevelButtonAction()
    {
        EventManager.LoadNextLevel();
    }
}

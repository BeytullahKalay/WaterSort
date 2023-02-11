using UnityEngine;

namespace UI
{
    public class CanvasController : MonoBehaviour
    {
        [Header("Panels")] [SerializeField] private GameObject levelCompletedPanel;
        [SerializeField] private GameObject inGamePanel;

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

        private void OpenLevelCompletePanel()
        {
            print("LevelCompleted");
            levelCompletedPanel.SetActive(true);
            inGamePanel.SetActive(false);
        }

        private void CloseLevelCompetePanel()
        {
            levelCompletedPanel.SetActive(false);
            inGamePanel.SetActive(true);
        }
    }
}
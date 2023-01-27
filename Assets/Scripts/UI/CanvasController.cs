using TMPro;
using UnityEngine;

namespace UI
{
    public class CanvasController : MonoBehaviour
    {
        [Header("Panels")] [SerializeField] private GameObject levelCompletedPanel;
        [SerializeField] private GameObject inGamePanel;


        [Header("Texts")] [SerializeField] private TMP_Text remainingUndoText;
        [SerializeField] private TMP_Text LevelText;

        private GameManager _gm;

        private void OnEnable()
        {
            EventManager.LevelCompleted += OpenLevelCompletePanel;
            EventManager.LoadNextLevel += CloseLevelCompetePanel;
            EventManager.UpdateRemainingUndo += UpdateRemainingUndo;
            EventManager.UpdateLevelText += UpdateLevelText;

        }

        private void OnDisable()
        {
            EventManager.LevelCompleted -= OpenLevelCompletePanel;
            EventManager.LoadNextLevel -= CloseLevelCompetePanel;
            EventManager.UpdateRemainingUndo -= UpdateRemainingUndo;
            EventManager.UpdateLevelText -= UpdateLevelText;

        }

        private void Start()
        {
            _gm = GameManager.Instance;
            UpdateLevelText();
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

        private void UpdateRemainingUndo(int remainingUndo)
        {
            remainingUndoText.text = remainingUndo.ToString();
        }

        private void UpdateLevelText()
        {
            LevelText.text = "Level " + (PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex) + 1).ToString();
        }

        // using by button actions
        public void NextLevelButtonAction()
        {
            // delete last level and create new level prototype
            EventManager.CreateNewLevelForJson?.Invoke();

            // load next level
            EventManager.LoadNextLevel?.Invoke();

            // update level text
            EventManager.UpdateLevelText?.Invoke();
        }

        // using by button actions
        public void UndoLastMove()
        {
            if (_gm.InActionBottleList.Count == 0)
            {
                EventManager.UndoLastMove?.Invoke();
            }
        }

        // using by button actions
        public void RestartLevel()
        {
            Debug.Log("Restart");
            EventManager.RestartLevel?.Invoke();
        }

        // using by button actions
        public void AddOneMoreBottle()
        {
            if (GameManager.Instance.InActionBottleList.Count == 0)
            {
                Debug.Log("Add one more bottle");
                EventManager.AddExtraEmptyBottle?.Invoke();
            }
            else
            {
                Debug.Log("In action bottle list more than 0");
            }
        }

        // using by button actions
        public void OpenMenuTab()
        {
            Debug.Log("Open menu tab");
        }
    }
}
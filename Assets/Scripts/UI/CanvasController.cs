using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.LevelCompleted += OpenLevelCompletePanel;
        EventManager.LoadNextLevel += CloseLevelCompetePanel;
        EventManager.UpdateRemainingUndo += UpdateRemainingUndo;
    }

    private void OnDisable()
    {
        EventManager.LevelCompleted -= OpenLevelCompletePanel;
        EventManager.LoadNextLevel -= CloseLevelCompetePanel;
        EventManager.UpdateRemainingUndo -= UpdateRemainingUndo;
    }


    [Header("Panels")]
    [SerializeField] private GameObject levelCompletedPanel;
    [SerializeField] private GameObject inGamePanel;


    [Header("Texts")] [SerializeField] private TMP_Text remainingUndoText;

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

    public void NextLevelButtonAction()
    {
        EventManager.LoadNextLevel();
    }

    // using by button actions
    public void UndoLastMove()
    {
        EventManager.UndoLastMove();
    }

    private void UpdateRemainingUndo(int remainingUndo)
    {
        remainingUndoText.text = remainingUndo.ToString();
    }
}

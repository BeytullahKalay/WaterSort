using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [Header("Panels")] [SerializeField] private GameObject levelCompletedPanel;
    [SerializeField] private GameObject inGamePanel;


    [Header("Texts")] [SerializeField] private TMP_Text remainingUndoText;

    private GameManager _gm;

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

    private void Start()
    {
        _gm = GameManager.Instance;
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

    public void NextLevelButtonAction()
    {
        EventManager.LoadNextLevel();
    }

    // using by button actions
    public void UndoLastMove()
    {
        if (_gm.InActionBottleList.Count == 0)
            EventManager.UndoLastMove();
    }

    private void UpdateRemainingUndo(int remainingUndo)
    {
        remainingUndoText.text = remainingUndo.ToString();
    }
}
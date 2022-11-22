using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [Header("Panels")] [SerializeField] private GameObject levelCompletedPanel;
    [SerializeField] private GameObject inGamePanel;


    [Header("Texts")] [SerializeField] private TMP_Text remainingUndoText;
    [SerializeField] private TMP_Text LevelText;

    [Header("One More Bottle Button")] [SerializeField]
    private GameObject buttonGameObject;

    [SerializeField] private Color notIntractableColor;

    private GameManager _gm;

    private void OnEnable()
    {
        EventManager.LevelCompleted += OpenLevelCompletePanel;
        EventManager.LoadNextLevel += CloseLevelCompetePanel;
        EventManager.UpdateRemainingUndo += UpdateRemainingUndo;
        EventManager.UpdateLevelText += UpdateLevelText;
        EventManager.AddExtraEmptyBottle += MakeAddExtraBottleButtonNotIntractable;
        EventManager.LoadNextLevel += MakeAddExtraBottleButtonIntractable;
        EventManager.RestartLevel += MakeAddExtraBottleButtonIntractable;
    }

    private void OnDisable()
    {
        EventManager.LevelCompleted -= OpenLevelCompletePanel;
        EventManager.LoadNextLevel -= CloseLevelCompetePanel;
        EventManager.UpdateRemainingUndo -= UpdateRemainingUndo;
        EventManager.UpdateLevelText -= UpdateLevelText;
        EventManager.AddExtraEmptyBottle -= MakeAddExtraBottleButtonNotIntractable;
        EventManager.LoadNextLevel -= MakeAddExtraBottleButtonIntractable;
        EventManager.RestartLevel -= MakeAddExtraBottleButtonIntractable;
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
        LevelText.text = "Level " + (PlayerPrefs.GetInt("LevelIndex") + 1).ToString();
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
        Debug.Log("Add one more bottle");
        EventManager.AddExtraEmptyBottle?.Invoke();
    }

    // using by button actions
    public void OpenMenuTab()
    {
        Debug.Log("Open menu tab");
    }

    private void MakeAddExtraBottleButtonNotIntractable()
    {
        var button = buttonGameObject.GetComponent<Button>();
        button.image.color = notIntractableColor;
        button.enabled = false;
    }

    private void MakeAddExtraBottleButtonIntractable()
    {
        var button = buttonGameObject.GetComponent<Button>();
        button.image.color = Color.white;
        button.enabled = true;
    }
}
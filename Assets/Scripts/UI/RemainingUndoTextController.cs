using TMPro;
using UnityEngine;

namespace UI
{
    public class RemainingUndoTextController : MonoBehaviour
    {
        [SerializeField] private TMP_Text remainingUndoText;

        private void OnEnable()
        {
            EventManager.UpdateRemainingUndo += UpdateRemainingUndo;
        }

        private void OnDisable()
        {
            EventManager.UpdateRemainingUndo -= UpdateRemainingUndo;
        }

        private void UpdateRemainingUndo(int remainingUndo)
        {
            remainingUndoText.text = remainingUndo.ToString();
        }
    }
}
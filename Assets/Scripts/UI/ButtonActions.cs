using UnityEngine;

namespace UI
{
    public class ButtonActions : MonoBehaviour
    {
        private GameManager _gm;

        private void Start() => _gm = GameManager.Instance;
        
        
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
                EventManager.UndoLastMove?.Invoke();
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
                Debug.Log("In action bottle list more than 0");
        }

        // using by button actions
        public void OpenMenuTab()
        {
            Debug.Log("Open menu tab");
        }
    }
}
using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LevelTextController : MonoBehaviour
    {
        [SerializeField] private TMP_Text LevelText;

        private void OnEnable()
        {
            EventManager.UpdateLevelText += UpdateLevelText;
        }

        private void OnDisable()
        {
            EventManager.UpdateLevelText -= UpdateLevelText;
        }

        private void Start()
        {
            UpdateLevelText();
        }

        private void UpdateLevelText()
        {
            LevelText.text = "Level " + (PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex) + 1).ToString();
        }
        
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OneMoreBottleButtonController : MonoSingleton<OneMoreBottleButtonController>
    {
        [Header("One More Bottle Button")] [SerializeField]
        private GameObject oneMoreBottleButton;
        [SerializeField] private Color notIntractableColor;
    

        private void OnEnable()
        {
            EventManager.AddExtraEmptyBottle += MakeAddExtraBottleButtonNotIntractable;
            EventManager.LoadNextLevel += MakeAddExtraBottleButtonIntractable;
            EventManager.RestartLevel += MakeAddExtraBottleButtonIntractable;
        }

        private void OnDisable()
        {
            EventManager.AddExtraEmptyBottle -= MakeAddExtraBottleButtonNotIntractable;
            EventManager.LoadNextLevel -= MakeAddExtraBottleButtonIntractable;
            EventManager.RestartLevel -= MakeAddExtraBottleButtonIntractable;
        }

        public void MakeAddExtraBottleButtonNotIntractable()
        {
            var button = oneMoreBottleButton.GetComponent<Button>();
            button.image.color = notIntractableColor;
            button.enabled = false;
        }

        public void MakeAddExtraBottleButtonIntractable()
        {
            var button = oneMoreBottleButton.GetComponent<Button>();
            button.image.color = Color.white;
            button.enabled = true;
        }
    
    }
}

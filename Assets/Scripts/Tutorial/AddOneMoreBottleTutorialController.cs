using UI;
using UnityEngine;

namespace Tutorial
{
    public class AddOneMoreBottleTutorialController : MonoBehaviour
    {
        private void Start()
        {
            OneMoreBottleButtonController.Instance.MakeAddExtraBottleButtonNotIntractable();
        }
    }
}
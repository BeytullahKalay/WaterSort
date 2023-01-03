using UnityEngine;

namespace BottleCodes
{
    public class BottleActionListController : MonoBehaviour
    {
        private GameManager _gm;

        private void Awake()
        {
            _gm = GameManager.Instance;
        }

        public void AddBottleToActionBottleList(BottleController bottle)
        {
            _gm.InActionBottleList.Add(bottle);
        }

        public void RemoveBottleFromActionBottleList(BottleController bottle)
        {
            _gm.InActionBottleList.Remove(bottle);
        }
    }
}

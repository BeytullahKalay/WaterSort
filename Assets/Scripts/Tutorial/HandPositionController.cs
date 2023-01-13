using UnityEngine;

namespace Tutorial
{
    public class HandPositionController : MonoBehaviour
    {
        [SerializeField] private Transform transferTransform;

        [SerializeField] private ClickController leftBottleClicker;
        [SerializeField] private ClickController rightBottleClicker;
        
        private void OnEnable()
        {
            leftBottleClicker.OnClicked += MoveRight;

            leftBottleClicker.OnClicked += leftBottleClicker.CallCloseCollider;

            leftBottleClicker.OnClicked += rightBottleClicker.CallOpenCollider;

            rightBottleClicker.OnClicked += DestroyHand;

        }

        private void OnDisable()
        {
            leftBottleClicker.OnClicked -= MoveRight;
            
            leftBottleClicker.OnClicked -= leftBottleClicker.CallCloseCollider;
            
            leftBottleClicker.OnClicked -= rightBottleClicker.CallOpenCollider;
            
            rightBottleClicker.OnClicked -= DestroyHand;
        }


        private void MoveRight()
        {
            var pos = gameObject.transform.position;
            pos.x = transferTransform.position.x;
            gameObject.transform.position = pos;
        }

        private void DestroyHand()
        {
            Destroy(gameObject);
        }
    }
}

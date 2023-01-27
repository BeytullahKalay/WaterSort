using System;
using UnityEngine;

namespace Tutorial
{
    [RequireComponent(typeof(ColliderController))]
    public class ClickController : MonoBehaviour
    {

        public Action OnClicked;

        private ColliderController _colliderController;

        private void Awake()
        {
            _colliderController = GetComponent<ColliderController>();
        }

        private void OnMouseDown()
        {
            OnClicked?.Invoke();
        }

        public void CallCloseCollider()
        {
            _colliderController.CloseColliderAfter(.1f);
        }

        public void CallOpenCollider()
        {
            _colliderController.OpenColliderAfter(.1f);
        }
    }
}

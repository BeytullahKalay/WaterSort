using System;
using UnityEngine;

namespace Tutorial
{
    public class ClickController : MonoBehaviour
    {

        public Action OnClicked;
        
        private BoxCollider2D _boxCollider2D;

        private void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        private void OnMouseDown()
        {
            OnClicked?.Invoke();
        }

        public void CallCloseCollider()
        {
            Invoke(nameof(CloseCollider),.2f);
        }

        public void CallOpenCollider()
        {
            Invoke(nameof(OpenCollider),.2f);
        }

        private void CloseCollider()
        {
            _boxCollider2D.enabled = false;
        }

        private void OpenCollider()
        {
            _boxCollider2D.enabled = true;
        }
        
        
    }
}

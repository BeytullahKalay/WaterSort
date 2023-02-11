using UnityEngine;

namespace Tutorial
{
    public class ColliderController : MonoBehaviour
    {
        private BoxCollider2D _boxCollider2D;

        private void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }
    
        private void CloseCollider()
        {
            _boxCollider2D.enabled = false;
        }

        private void OpenCollider()
        {
            _boxCollider2D.enabled = true;
        }

        public void CloseColliderAfter(float seconds)
        {
            Invoke(nameof(CloseCollider),seconds);
        }
        
        public void OpenColliderAfter(float seconds)
        {
            Invoke(nameof(OpenCollider),seconds);
        }
    }
}

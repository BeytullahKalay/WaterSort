using DG.Tweening;
using UnityEngine;

namespace BottleCodes.Animation
{
    [RequireComponent(typeof(BottleValueHolder))]
    public class SelectionAnimations : MonoBehaviour
    {
        private BottleValueHolder _bottleValueHolder;

        private void Awake()
        {
            _bottleValueHolder = GetComponent<BottleValueHolder>();
        }

        public void OnSelected()
        {
            _bottleValueHolder.SelectedTween?.Kill();
            _bottleValueHolder.SelectedTween = transform.DOMoveY(_bottleValueHolder.OriginalPosition.y + .5f, .25f);
        }

        public void OnSelectionCanceled()
        {
            _bottleValueHolder.SelectedTween?.Kill();
            _bottleValueHolder.SelectedTween = transform.DOMove(_bottleValueHolder.OriginalPosition, .25f);
        }
    }
}
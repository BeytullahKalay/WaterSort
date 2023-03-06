using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BottleCodes.Animation
{
    [RequireComponent(typeof(BottleFindRotationPointAndDirection))]
    [RequireComponent(typeof(SelectionAnimations))]
    [RequireComponent(typeof(MoveAnimation))]
    [RequireComponent(typeof(PreRotateAnimation))]
    [RequireComponent(typeof(BottleValueHolder))]
    public class BottleAnimationController : MonoBehaviour
    {
        
        [Header("Locker Values")] public bool BottleIsLocked;

        private SelectionAnimations _selectionAnimations;
        private MoveAnimation _moveAnimation;
        private PreRotateAnimation _preRotateAnimation;
        private BottleValueHolder _bottleValueHolder;
        private BoxCollider2D _boxCollider2D;

        
        private void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _selectionAnimations = GetComponent<SelectionAnimations>();
            _moveAnimation = GetComponent<MoveAnimation>();
            _preRotateAnimation = GetComponent<PreRotateAnimation>();
            _bottleValueHolder = GetComponent<BottleValueHolder>();
        }

        private void Start()
        {
            SetOriginalPosition(transform.position);
        }

        public void OnSelected()
        {
            _selectionAnimations.OnSelected();
        }
        
        public void OnSelectionCanceled()
        {
            _selectionAnimations.OnSelectionCanceled();
        }


        public void DisableCollider()
        {
            _boxCollider2D.enabled = false;
        }
        
        public void PlayPreRotateTween()
        {
            _preRotateAnimation.PlayPreRotateTween();
        }

        public void StartAnimationChain()
        {
            _moveAnimation.PlayMoveTween();
        }

        public void SetOriginalPosition(Vector3 newPosition)
        {
            _bottleValueHolder.OriginalPosition = newPosition;
        }
    }
}
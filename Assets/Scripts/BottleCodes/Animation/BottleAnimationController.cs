using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BottleCodes.Animation
{
    [RequireComponent(typeof(BottleFindRotationPointAndDirection))]
    [RequireComponent(typeof(SelectionAnimations))]
    [RequireComponent(typeof(MoveAnimation))]
    public class BottleAnimationController : MonoBehaviour
    {
        [Header("Animation Curves")] public AnimationCurve ScaleAndRotationMultiplierCurve;
        public AnimationCurve FillAmountCurve;

        [Header("Animation Values")]
        public float PreRotateDuration = .25f;
        public float PreRotateAmount = 15f;

        [Header("Locker Values")] public bool BottleIsLocked;


        [Header("Tweens")] private Tween _selectedTween;
        private Tween _moveTween;
        private Tween _rotateBottle;
        private Tween _preRotate;

        private BottleFindRotationPointAndDirection _bottleFindRotationPointAndDirection;
        private SelectionAnimations _selectionAnimations;
        private MoveAnimation _moveAnimation;

        public Vector3 OriginalPosition { get; set; }

        private BoxCollider2D _boxCollider2D;

        
        private void Awake()
        {
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _bottleFindRotationPointAndDirection = GetComponent<BottleFindRotationPointAndDirection>();
            _selectionAnimations = GetComponent<SelectionAnimations>();
            _moveAnimation = GetComponent<MoveAnimation>();
        }

        private void Start()
        {
            OriginalPosition = transform.position;
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
        

        #region Pre Rotation

        public void PlayPreRotateTween(BottleColorController bottleColorController,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, FillAndRotationValues fillAndRotationValues,
            BottleData bottleData)
        {
            float angle = 0;
            float lastAngleValue = 0;
            var desRot = Vector3.forward * (_bottleFindRotationPointAndDirection.DirectionMultiplier * PreRotateAmount);

            _preRotate = DOTween.To(() => angle, x => angle = x, desRot.z, PreRotateDuration)
                .SetEase(Ease.OutQuart).SetUpdate(UpdateType.Fixed, true).OnUpdate(() =>
                {
                    bottleAnimationSpeedUp.CheckSpeedUp(_preRotate);

                    transform.RotateAround(_bottleFindRotationPointAndDirection.ChosenRotationPoint.position,
                        Vector3.forward, angle - lastAngleValue);

                    if (fillAndRotationValues.GetFillCurrentAmount(bottleData) > FillAmountCurve.Evaluate(angle))
                    {
                        bottleColorController.SetFillAmount(FillAmountCurve.Evaluate(angle));
                    }

                    bottleColorController.SetSARM(ScaleAndRotationMultiplierCurve.Evaluate(angle));

                    lastAngleValue = angle;
                });
        }

        #endregion
        
        public void StartAnimationChain()
        {
            _moveAnimation.PlayMoveTween();
        }
        
        public List<Tween> GetAnimationTweens()
        {
            return new List<Tween>() { _moveTween, _preRotate, _rotateBottle };
        }
    }
}
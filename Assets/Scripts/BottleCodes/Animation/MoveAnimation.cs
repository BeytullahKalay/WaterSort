using DG.Tweening;
using UnityEngine;

namespace BottleCodes.Animation
{
    [RequireComponent(typeof(BottleValueHolder))]
    [RequireComponent(typeof(RotateAnimation))]
    public class MoveAnimation : MonoBehaviour
    {
        [SerializeField] private float MoveBottleDuration = 5f;

        private BottleValueHolder _bottleValueHolder;
        private RotateAnimation _rotateAnimation;

        private Tween _moveTween;
        
        private void Awake()
        {
            _bottleValueHolder = GetComponent<BottleValueHolder>();
            _rotateAnimation = GetComponent<RotateAnimation>();
        }

        

        public void PlayMoveTween()
        {
            _bottleValueHolder.BottleLineRendererController.InitializeLineRenderer(_bottleValueHolder.BottleData);

            var bottleTransferController = _bottleValueHolder.BottleTransferController;
            var beforePourAmount = bottleTransferController.BottleControllerRef.BottleData.NumberOfColorsInBottle;

            // increase number of colors in second bottle
            bottleTransferController.BottleControllerRef.BottleData.IncreaseNumberOfColorsInBottle(
                bottleTransferController.NumberOfColorsToTransfer);

            // update second bottle top color
            bottleTransferController.BottleControllerRef.BottleColorController.UpdateTopColorValues(
                bottleTransferController.BottleControllerRef.BottleData);
            

            _moveTween = transform.DOMove(_bottleValueHolder.BottleFindRotationPointAndDirection.MovePosition,
                    MoveBottleDuration)
                .OnStart(() =>
                {
                    _bottleValueHolder.SelectedTween?.Kill();
                    bottleTransferController.BottleColorController.UpdateTopColorValues(bottleTransferController
                        .BottleData);

                    _bottleValueHolder.BottleData.UpdatePreviousTopColor();
                }).SetUpdate(UpdateType.Fixed, true).OnUpdate(() =>
                {
                    _bottleValueHolder.BottleAnimationSpeedUp.CheckSpeedUp(_moveTween);
                    bottleTransferController.BottleControllerRef.BottleAnimationController.BottleIsLocked = true;
                }).OnComplete(() =>
                {
                    _rotateAnimation.RotateBottle(beforePourAmount);

                });
        }
    }
}
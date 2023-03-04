using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BottleCodes.Animation
{
    [RequireComponent(typeof(BottleLineRendererController))]
    [RequireComponent(typeof(BottleFindRotationPointAndDirection))]
    public class BottleAnimationController : MonoBehaviour
    {
        [Header("Animation Curves")] public AnimationCurve ScaleAndRotationMultiplierCurve;
        public AnimationCurve FillAmountCurve;

        [Header("Animation Values")] public float LineRendererPouringDistance = 1f;
        public float MoveBottleDuration = 5f;
        public float RotateBottleDuration = 1f;
        public float PreRotateDuration = .25f;
        public float PreRotateAmount = 15f;

        [Header("Locker Values")] public bool BottleIsLocked;


        [Header("Tweens")] private Tween _selectedTween;
        private Tween _moveTween;
        private Tween _rotateBottle;
        private Tween _preRotate;

        private BottleLineRendererController _bottleLineRendererController;
        private BottleFindRotationPointAndDirection _bottleFindRotationPointAndDirection;

        public Vector3 OriginalPosition { get; set; }

        private BoxCollider2D _boxCollider2D;

        private GameManager _gm;

        private FillAndRotationValues _fillAndRotationValues;
        
        private void Awake()
        {
            _bottleLineRendererController = GetComponent<BottleLineRendererController>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _fillAndRotationValues = FillAndRotationValues.Instance;
            _bottleFindRotationPointAndDirection = GetComponent<BottleFindRotationPointAndDirection>();
        }

        private void Start()
        {
            _gm = GameManager.Instance;
            OriginalPosition = transform.position;
        }

        public void OnSelected()
        {
            _selectedTween?.Kill();
            _selectedTween = transform.DOMoveY(OriginalPosition.y + .5f, .25f);
        }

        public void OnSelectionCanceled()
        {
            _selectedTween?.Kill();
            _selectedTween = transform.DOMove(OriginalPosition, .25f);
        }


        public void DisableCollider()
        {
            _boxCollider2D.enabled = false;
        }


        public void PlayMoveTween(BottleTransferController bottleTransferController, BottleData bottleData,
            BottleColorController bottleColorController,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, BottleController bottleController)
        {
            _bottleLineRendererController.InitializeLineRenderer(bottleData);

            var beforePourAmount = bottleTransferController.BottleControllerRef.BottleData.NumberOfColorsInBottle;

            // increase number of colors in second bottle
            bottleTransferController.BottleControllerRef.BottleData.IncreaseNumberOfColorsInBottle(
                bottleTransferController.NumberOfColorsToTransfer);

            // update second bottle top color
            bottleTransferController.BottleControllerRef.BottleColorController.UpdateTopColorValues(
                bottleTransferController.BottleControllerRef.BottleData);

            #region Move

            _moveTween = transform.DOMove(_bottleFindRotationPointAndDirection.MovePosition, MoveBottleDuration)
                .OnStart(() =>
                {
                    _selectedTween?.Kill();
                    bottleTransferController.BottleColorController.UpdateTopColorValues(bottleTransferController
                        .BottleData);

                    bottleData.UpdatePreviousTopColor();
                }).SetUpdate(UpdateType.Fixed, true).OnUpdate(() =>
                {
                    bottleAnimationSpeedUp.CheckSpeedUp(_moveTween);
                    bottleTransferController.BottleControllerRef.BottleAnimationController.BottleIsLocked = true;
                }).OnComplete(() =>
                {
                    RotateBottle(bottleTransferController, bottleData,
                        bottleColorController, bottleAnimationSpeedUp, bottleController, beforePourAmount);
                });

            #endregion
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

        #region RotateBottle

        private void RotateBottle(BottleTransferController bottleTransferController,
            BottleData bottleData, BottleColorController bottleColorController,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, BottleController bottleController, int beforePourAmount)
        {
            var startAngle = transform.eulerAngles.z;
            var angle = WrapAngle(startAngle);
            var lastAngleValue = WrapAngle(startAngle);
            var bottleControllerRef = bottleTransferController.BottleControllerRef;
            var numberOfEmptySpacesInSecondBottle = 4 - beforePourAmount;
            var rotateValue = _fillAndRotationValues.GetRotationValue(bottleData, numberOfEmptySpacesInSecondBottle);
            var desRot = _bottleFindRotationPointAndDirection.DirectionMultiplier * rotateValue;
            var rotationPoint = _fillAndRotationValues.GetFillCurrentAmount(bottleData);
            var lastTransferAmount = bottleTransferController.NumberOfColorsToTransfer;


            _rotateBottle = DOTween.To(() => angle, x => angle = x, desRot, RotateBottleDuration)
                .SetUpdate(UpdateType.Fixed, true).OnStart(() =>
                {
                    // decrease number of colors in first bottle
                    bottleData.DecreaseNumberOfColorsInBottle(bottleTransferController.NumberOfColorsToTransfer);

                    // set bottle color scale to 1
                    bottleControllerRef.BottleColorController.SetSARM(1);
                }).OnUpdate(() =>
                {
                    bottleAnimationSpeedUp.CheckSpeedUp(_rotateBottle);


                    transform.RotateAround(_bottleFindRotationPointAndDirection.ChosenRotationPoint.position,
                        Vector3.forward, angle - lastAngleValue);


                    if (rotationPoint > FillAmountCurve.Evaluate(Mathf.Abs(angle)))
                    {
                        _bottleLineRendererController.SetLineRenderer(
                            _bottleFindRotationPointAndDirection.ChosenRotationPoint,
                            LineRendererPouringDistance);

                        bottleColorController.SetFillAmount(FillAmountCurve.Evaluate(Mathf.Abs(angle)));

                        bottleControllerRef.BottleColorController.FillUp(
                            FillAmountCurve.Evaluate(Mathf.Abs(lastAngleValue)) -
                            FillAmountCurve.Evaluate(Mathf.Abs(angle)));


                        var fillAmounts = bottleControllerRef.FillAndRotationValues.FillAmounts;

                        bottleControllerRef.BottleColorController.ClampFillAmount(
                            fillAmounts[0], fillAmounts[bottleControllerRef.BottleData.NumberOfColorsInBottle]);
                    }

                    bottleColorController.SetSARM(ScaleAndRotationMultiplierCurve.Evaluate(angle));

                    lastAngleValue = angle;
                }).OnComplete(() =>
                {
                    bottleColorController.UpdateTopColorValues(bottleData);

                    CheckBottlesAreSorted(bottleController);

                    _bottleLineRendererController.ReleaseLineRenderer();

                    RotateBottleBackAndMoveOriginalPosition(bottleData, bottleColorController,
                        bottleController, bottleTransferController, bottleAnimationSpeedUp, lastTransferAmount);
                });
        }

        #endregion

        #region Rotate Bottle Back and Move Original Position

        private void RotateBottleBackAndMoveOriginalPosition(BottleData bottleData,
            BottleColorController bottleColorController,
            BottleController bottleController, BottleTransferController bottleTransferController,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, int lastTransferAmount)
        {
            transform.DOMove(OriginalPosition, MoveBottleDuration).OnStart(() =>
            {
                bottleAnimationSpeedUp.OnSpeedUp = false;

                var bottleRef = bottleTransferController.BottleControllerRef;
                var bottleRefData = bottleRef.BottleData;

                bottleRefData.ActionBottles.Remove(bottleController);

                if (bottleRefData.ActionBottles.Count <= 0)
                    bottleRef.BottleAnimationController.BottleIsLocked = false;

                if (bottleRefData.BottleSorted && bottleRefData.ActionBottles.Count <= 0)
                    bottleRef.BottleColorController.PlayParticleFX();
            }).OnComplete(() =>
            {
                _boxCollider2D.enabled = true;

                EventManager.AddMoveToList?.Invoke(bottleController, bottleTransferController.BottleControllerRef,
                    lastTransferAmount, bottleData.PreviousTopColor);
            });


            var noColorInBottle = bottleData.NumberOfTopColorLayers <= 0;
            var startAngle = transform.eulerAngles.z;
            var angle = WrapAngle(startAngle);
            var lastAngleValue = WrapAngle(startAngle);

            DOTween.To(() => angle, x => angle = x, 0, MoveBottleDuration)
                .SetUpdate(UpdateType.Fixed, true).OnStart(() =>
                {
                    if (noColorInBottle) bottleColorController.SetSARM(2);
                }).OnUpdate(() =>
                {
                    transform.RotateAround(_bottleFindRotationPointAndDirection.ChosenRotationPoint.position, Vector3.forward, angle - lastAngleValue);

                    lastAngleValue = angle;

                    if (noColorInBottle) return;

                    bottleColorController.SetSARM(ScaleAndRotationMultiplierCurve.Evaluate(angle));
                }).OnComplete(() =>
                {
                    RemoveBottleFromInActionBottleList(bottleController);

                    bottleController.BottleSpriteRendererOrderController.ResetSortingOrder(
                        bottleController.BottleSpriteRenderer, bottleController.BottleMaskSR);
                });
        }

        #endregion


        private void CheckBottlesAreSorted(BottleController bottleController)
        {
            bottleController.BottleColorController.CheckIsBottleSorted(bottleController.BottleData);

            var bottleRef = bottleController.BottleTransferController.BottleControllerRef;
            bottleRef.BottleColorController.CheckIsBottleSorted(bottleRef.BottleData);
        }


        private void RemoveBottleFromInActionBottleList(BottleController bottleController)
        {
            _gm.InActionBottleList.Remove(bottleController);
        }

        public List<Tween> GetAnimationTweens()
        {
            return new List<Tween>() { _moveTween, _preRotate, _rotateBottle };
        }


        private float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }
    }
}
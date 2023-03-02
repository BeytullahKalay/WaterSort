using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BottleCodes.Animation
{
    [RequireComponent(typeof(BottleLineRendererController))]
    public class BottleAnimationController : MonoBehaviour
    {
        [Header("Animation Curves")] public AnimationCurve ScaleAndRotationMultiplierCurve;
        public AnimationCurve FillAmountCurve;

        [Header("Animation Values")] public float LineRendererPouringDistance = 1f;
        public float MoveBottleDuration = 5f;
        public float RotateBottleDuration = 1f;
        public float PreRotateDuration = .25f;
        public float PreRotateAmount = 15f;
        public float BottlePouringDistanceIncreasor = .25f;

        [Header("Rotation Points")] public Transform LeftRotationPoint;
        public Transform RightRotationPoint;
        private Transform _chosenRotationPoint;

        [Header("Locker Values")] public bool BottleIsLocked;


        [Header("Tweens")] private Tween _selectedTween;
        private Tween _moveTween;
        private Tween _rotateBottle;
        private Tween _preRotate;

        private BottleLineRendererController _bottleLineRendererController;

        private float _directionMultiplier = 1;

        public Vector3 OriginalPosition { get; set; }
        private Vector3 _movePosition;

        private Camera _camera;

        private BoxCollider2D _boxCollider2D;

        private GameManager _gm;

        private FillAndRotationValues _fillAndRotationValues;


        private void Awake()
        {
            _bottleLineRendererController = GetComponent<BottleLineRendererController>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _camera = Camera.main;
            _fillAndRotationValues = FillAndRotationValues.Instance;
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

        public void ChoseRotationPointAndDirection(BottleController bottleControllerRef)
        {
            var minBottleDistanceToCorner = 1f;

            var leftOfScreen = _camera.ViewportToWorldPoint(Vector3.zero).x;
            var rightOfScreen = _camera.ViewportToWorldPoint(Vector3.one).x;

            var bottleRefPosition = bottleControllerRef.transform.position;
            var distanceToLeft = Mathf.Abs(bottleRefPosition.x - leftOfScreen);
            var distanceToRight = Mathf.Abs(bottleRefPosition.x - rightOfScreen);

            if (transform.position.x > bottleRefPosition.x)
            {
                if (minBottleDistanceToCorner >= distanceToRight)
                {
                    _chosenRotationPoint = RightRotationPoint;
                    _directionMultiplier = -1;
                }
                else
                {
                    _chosenRotationPoint = LeftRotationPoint;
                    _directionMultiplier = 1;
                }
            }
            else
            {
                if (minBottleDistanceToCorner >= distanceToLeft)
                {
                    _chosenRotationPoint = LeftRotationPoint;
                    _directionMultiplier = 1;
                }
                else
                {
                    _chosenRotationPoint = RightRotationPoint;
                    _directionMultiplier = -1;
                }
            }
        }

        public void DisableCollider()
        {
            _boxCollider2D.enabled = false;
        }

        public void ChoseMovePosition(BottleTransferController bottleTransferController)
        {
            var bottleRef = bottleTransferController.BottleControllerRef;

            // if chosen position is left go right
            if (_chosenRotationPoint == LeftRotationPoint)
            {
                _movePosition = bottleRef.BottleAnimationController.RightRotationPoint.position;
                _movePosition.x += BottlePouringDistanceIncreasor;
            }
            else // if chose position is right go left
            {
                _movePosition = bottleRef.BottleAnimationController.LeftRotationPoint.position;
                _movePosition.x -= BottlePouringDistanceIncreasor;
            }
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


            _moveTween = transform.DOMove(_movePosition, MoveBottleDuration).OnStart(() =>
            {
                _selectedTween?.Kill();
                bottleTransferController.BottleColorController.UpdateTopColorValues(bottleTransferController
                    .BottleData);
            }).SetUpdate(UpdateType.Fixed, true).OnUpdate(() =>
            {
                bottleAnimationSpeedUp.CheckSpeedUp(_moveTween);
                bottleTransferController.BottleControllerRef.BottleAnimationController.BottleIsLocked = true;
            }).OnComplete(() =>
            {
                RotateBottle(bottleTransferController, bottleData,
                    bottleColorController, bottleAnimationSpeedUp, bottleController, beforePourAmount);
            });
        }

        #region Pre Rotation

        public void PlayPreRotateTween(BottleColorController bottleColorController,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, FillAndRotationValues fillAndRotationValues,
            BottleData bottleData)
        {
            float angle = 0;
            float lastAngleValue = 0;
            var desRot = Vector3.forward * (_directionMultiplier * PreRotateAmount);

            _preRotate = DOTween.To(() => angle, x => angle = x, desRot.z, PreRotateDuration)
                .SetEase(Ease.OutQuart).SetUpdate(UpdateType.Fixed, true).OnUpdate(() =>
                {
                    bottleAnimationSpeedUp.CheckSpeedUp(_preRotate);

                    transform.RotateAround(_chosenRotationPoint.position, Vector3.forward, angle - lastAngleValue);

                    if (fillAndRotationValues.GetFillCurrentAmount(bottleData) > FillAmountCurve.Evaluate(angle))
                    {
                        bottleColorController.SetFillAmount(FillAmountCurve.Evaluate(angle));
                    }

                    bottleColorController.SetSARM(ScaleAndRotationMultiplierCurve.Evaluate(angle));

                    lastAngleValue = angle;
                });
        }

        #endregion


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
            var desRot = _directionMultiplier * rotateValue;


            var rotationPoint = _fillAndRotationValues.GetFillCurrentAmount(bottleData);


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

                    
                    transform.RotateAround(_chosenRotationPoint.position, Vector3.forward, angle - lastAngleValue);


                    if (rotationPoint > FillAmountCurve.Evaluate(Mathf.Abs(angle)))
                    {
                        _bottleLineRendererController.SetLineRenderer(_chosenRotationPoint,
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
                        bottleController, bottleTransferController,bottleAnimationSpeedUp);
                });
        }

        private void CheckBottlesAreSorted(BottleController bottleController)
        {
            bottleController.BottleColorController.CheckIsBottleSorted(bottleController.BottleData);
            
            var bottleRef = bottleController.BottleTransferController.BottleControllerRef;
            bottleRef.BottleColorController.CheckIsBottleSorted(bottleRef.BottleData);
        }

        private void RotateBottleBackAndMoveOriginalPosition(BottleData bottleData, BottleColorController bottleColorController,
            BottleController bottleController, BottleTransferController bottleTransferController,BottleAnimationSpeedUp bottleAnimationSpeedUp)
        {
            transform.DOMove(OriginalPosition, MoveBottleDuration).OnStart(() =>
            {
                bottleAnimationSpeedUp.OnSpeedUp = false;
            }).OnComplete(() =>
            {
                _boxCollider2D.enabled = true;

                var bottleRef = bottleTransferController.BottleControllerRef;
                bottleRef.BottleData.ActionBottles.Remove(bottleController);

                if (bottleRef.BottleData.ActionBottles.Count <= 0)
                    bottleRef.BottleAnimationController.BottleIsLocked = true;
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
                    transform.RotateAround(_chosenRotationPoint.position, Vector3.forward, angle - lastAngleValue);

                    lastAngleValue = angle;

                    if (noColorInBottle) return;

                    bottleColorController.SetSARM(ScaleAndRotationMultiplierCurve.Evaluate(angle));
                }).OnComplete(() => { RemoveBottleFromInActionBottleList(bottleController); });
        }

        private void RemoveBottleFromInActionBottleList(BottleController bottleController)
        {
            _gm.InActionBottleList.Remove(bottleController);
        }

        public List<Tween> GetAnimationTweens()
        {
            return new List<Tween>() { _moveTween, _preRotate, _rotateBottle};
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
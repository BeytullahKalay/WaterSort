using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BottleCodes
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
        public float BottlePouringHeightIncreasor = 1f;

        [Header("Rotation Points")] public Transform LeftRotationPoint;
        public Transform RightRotationPoint;
        private Transform _chosenRotationPoint;

        [Header("Tweens")] private Tween _selectedTween;
        private Tween _moveTween;
        private Tween _rotateBottle;
        private Tween _preRotate;
        private Tween _rotateBottleBack;


        private BottleLineRendererController _bottleLineRendererController;
        

        private float _directionMultiplier = 1;

        public Vector3 OriginalPosition { get; set; }
        private Vector3 _movePosition;

        private Camera _camera;

        private BoxCollider2D _boxCollider2D;


        private GameManager _gm;


        private void Awake()
        {
            _bottleLineRendererController = GetComponent<BottleLineRendererController>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _camera = Camera.main;
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

            var position = bottleControllerRef.transform.position;
            var distanceToLeft = Mathf.Abs(position.x - leftOfScreen);
            var distanceToRight = Mathf.Abs(position.x - rightOfScreen);

            if (transform.position.x > bottleControllerRef.transform.position.x)
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
                _movePosition = bottleRef.RightRotationPoint.position;
                _movePosition.x += BottlePouringDistanceIncreasor;
            }
            else // if chose position is right go left
            {
                _movePosition = bottleRef.LeftRotationPoint.position;
                _movePosition.x -= BottlePouringDistanceIncreasor;
            }

            _movePosition.y += BottlePouringHeightIncreasor;
        }


        public void PlayMoveTween(BottleTransferController bottleTransferController, BottleData bottleData,
            FillAndRotationValues fillAndRotationValues, BottleColorController bottleColorController,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, BottleController bottleController)
        {
            //InitializeLineRenderer(bottleData);
            _bottleLineRendererController.InitializeLineRenderer(bottleData);


            _moveTween = transform.DOMove(_movePosition, MoveBottleDuration);

            _moveTween.OnStart(() =>
                {
                    _selectedTween?.Kill();

                    bottleTransferController.BottleColorController.UpdateTopColorValues(bottleTransferController
                        .BottleData);
                }).SetUpdate(UpdateType.Fixed, true)
                .OnUpdate(() =>
                {
                    bottleTransferController.BottleControllerRef.BottleIsLocked = true;
                    bottleAnimationSpeedUp.CheckSpeedUp(_moveTween);
                })
                .OnComplete(() =>
                {
                    RotateBottle(fillAndRotationValues, bottleTransferController, bottleData,
                        bottleColorController, bottleAnimationSpeedUp, bottleController);
                });
        }

        // private void InitializeLineRenderer(BottleData bottleData)
        // {
        //     _lineRenderer = _gm.GetLineRenderer();
        //     _lineRenderer.startColor = bottleData.TopColor;
        //     _lineRenderer.endColor = bottleData.TopColor;
        // }

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

        private void RotateBottle(FillAndRotationValues fillAndRotationValues,
            BottleTransferController bottleTransferController,
            BottleData bottleData, BottleColorController bottleColorController,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, BottleController bottleController)
        {
            var startAngle = transform.eulerAngles.z;
            var angle = WrapAngle(startAngle);
            var lastAngleValue = WrapAngle(startAngle);

            var bottleControllerRef = bottleTransferController.BottleControllerRef;
            var numberOfEmptySpacesInSecondBottle = 4 - bottleControllerRef.BottleData.NumberOfColorsInBottle;
            var rotateValue = fillAndRotationValues.GetRotationValue(bottleData, numberOfEmptySpacesInSecondBottle);
            var desRot = _directionMultiplier * rotateValue;


            var rotationPoint = fillAndRotationValues.GetFillCurrentAmount(bottleData);


            _rotateBottle = DOTween.To(() => angle, x => angle = x, desRot, RotateBottleDuration)
                .SetUpdate(UpdateType.Fixed, true).OnStart(() =>
                {
                    // decrease number of colors in first bottle
                    bottleData.DecreaseNumberOfColorsInBottle(bottleTransferController.NumberOfColorsToTransfer);

                    // increase number of colors in seconds bottle
                    bottleTransferController.BottleControllerRef.BottleData.IncreaseNumberOfColorsInBottle(
                        bottleTransferController.NumberOfColorsToTransfer);
                }).OnUpdate(() =>
                {
                    bottleAnimationSpeedUp.CheckSpeedUp(_rotateBottle);
                    transform.RotateAround(_chosenRotationPoint.position, Vector3.forward, angle - lastAngleValue);



                    if (rotationPoint > FillAmountCurve.Evaluate(Mathf.Abs(angle)))
                    {
                        _bottleLineRendererController.SetLineRenderer(_chosenRotationPoint,LineRendererPouringDistance);
                        
                        bottleColorController.SetFillAmount(FillAmountCurve.Evaluate(Mathf.Abs(angle)));
                        
                        bottleControllerRef.BottleColorController.FillUp(
                            FillAmountCurve.Evaluate(Mathf.Abs(lastAngleValue)) -
                            FillAmountCurve.Evaluate(Mathf.Abs(angle)));
                    }

                    bottleColorController.SetSARM(ScaleAndRotationMultiplierCurve.Evaluate(angle));

                    lastAngleValue = angle;
                }).OnComplete(() =>
                {
                    UpdateColorsAfterPouring(bottleColorController, bottleTransferController, bottleData,
                        bottleAnimationSpeedUp, bottleController);
                });
        }

        private void UpdateColorsAfterPouring(BottleColorController bottleColorController,
            BottleTransferController bottleTransferController, BottleData bottleData,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, BottleController bottleController)
        {
            bottleColorController.UpdateTopColorValues(bottleData);
            
            bottleTransferController.BottleControllerRef.BottleColorController.UpdateTopColorValues(bottleTransferController.BottleControllerRef.BottleData);
            
            RotateBottleBackAndMoveOriginalPosition(bottleData, bottleAnimationSpeedUp, bottleColorController, bottleController);
        }

        private void RotateBottleBackAndMoveOriginalPosition(BottleData bottleData,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, BottleColorController bottleColorController,
            BottleController bottleController)
        {
            transform.DOMove(OriginalPosition, MoveBottleDuration);

            //_gm.ReleaseLineRenderer(_lineRenderer);
            _bottleLineRendererController.ReleaseLineRenderer();

            var noColorInBottle = bottleData.NumberOfTopColorLayers <= 0;
            _rotateBottleBack = transform.DORotate(Vector3.zero, RotateBottleDuration).OnStart(() =>
            {
                if (noColorInBottle) bottleColorController.SetSARM(1);
            }).OnUpdate(() =>
            {
                bottleAnimationSpeedUp.CheckSpeedUp(_rotateBottleBack);

                if (noColorInBottle) return;

                var angle = transform.rotation.eulerAngles.z;

                bottleColorController.SetSARM(ScaleAndRotationMultiplierCurve.Evaluate(angle));
            }).OnComplete(() =>
            {
                RemoveBottleFromInActionBottleList(bottleController);
                bottleColorController.CheckIsBottleSorted(bottleData);
            });
        }

        // private void SetLineRenderer()
        // {
        //     if (_lineRenderer.enabled) return;
        //
        //     // set line position
        //     var position = _chosenRotationPoint.position;
        //     _lineRenderer.SetPosition(0, position);
        //     _lineRenderer.SetPosition(1, position - Vector3.up * LineRendererPouringDistance);
        //
        //     // enable line renderer
        //     _lineRenderer.enabled = true;
        // }

        private void RemoveBottleFromInActionBottleList(BottleController bottleController)
        {
            _gm.InActionBottleList.Remove(bottleController);
        }

        public List<Tween> GetAnimationTweens()
        {
            return new List<Tween>() { _moveTween, _preRotate, _rotateBottle, _rotateBottleBack };
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
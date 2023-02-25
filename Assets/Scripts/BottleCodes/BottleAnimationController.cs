using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace BottleCodes
{
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

        [Header("Tweens")] private Tween _selectedTween;
        private Tween _moveTween;
        private Tween _rotateBottle;
        private Tween _preRotate;
        private Tween _rotateBottleBack;

        
        private float _directionMultiplier = 1;

        public Vector3 OriginalPosition { get; set; }

        private Camera _camera;

        private BoxCollider2D _boxCollider2D;

        private LineRenderer _lineRenderer;

        private GameManager _gm;


        private void Awake()
        {
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
                    _directionMultiplier = 1;
                }
                else
                {
                    _chosenRotationPoint = LeftRotationPoint;
                    _directionMultiplier = -1;
                }
            }
            else
            {
                if (minBottleDistanceToCorner >= distanceToLeft)
                {
                    _chosenRotationPoint = LeftRotationPoint;
                    _directionMultiplier = -1;
                }
                else
                {
                    _chosenRotationPoint = RightRotationPoint;
                    _directionMultiplier = 1;
                }
            }
        }

        public void DisableCollider()
        {
            _boxCollider2D.enabled = false;
        }

        public void ChoseMovePosition(BottleTransferController bottleTransferController)
        {
            var bottleRef = bottleTransferController.BottleTransferControllerRef;

            // if chosen position is left go right
            if (_chosenRotationPoint == LeftRotationPoint)
            {
                Vector3 movePos = bottleRef.RightRotationPoint.position;
                movePos.x += BottlePouringDistanceIncreasor;
                _moveTween = transform.DOMove(movePos, MoveBottleDuration);
            }
            else // if chose position is right go left
            {
                Vector3 movePos = bottleRef.LeftRotationPoint.position;
                movePos.x -= BottlePouringDistanceIncreasor;
                _moveTween = transform.DOMove(movePos, MoveBottleDuration);
            }
        }


        public void PlayMoveTween(BottleTransferController bottleTransferController, BottleData bottleData,
            FillAndRotationValues fillAndRotationValues, BottleColorController bottleColorController,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, BottleController bottleController)
        {
            InitializeLineRenderer(bottleData);

            _moveTween.OnStart(() =>
                {
                    _selectedTween?.Kill();

                    bottleTransferController.BottleColorController.UpdateTopColorValues(bottleTransferController
                        .BottleData);
                }).SetUpdate(UpdateType.Fixed, true)
                .OnUpdate(() =>
                {
                    bottleTransferController.BottleTransferControllerRef.BottleIsLocked = true;
                    bottleAnimationSpeedUp.CheckSpeedUp(_moveTween);
                })
                .OnComplete(() =>
                {
                    RotateBottle(fillAndRotationValues, bottleTransferController, bottleData,
                        bottleColorController, bottleAnimationSpeedUp, bottleController);
                });
        }

        private void InitializeLineRenderer(BottleData bottleData)
        {
            _lineRenderer = _gm.GetLineRenderer();
            _lineRenderer.startColor = bottleData.TopColor;
            _lineRenderer.endColor = bottleData.TopColor;
        }

        public void PlayPreRotateTween(BottleColorController bottleColorController,
            BottleAnimationSpeedUp bottleAnimationSpeedUp)
        {
            float lastAngeValue = 0;
            var desRot = Vector3.forward * (_directionMultiplier * PreRotateAmount);

            _preRotate = transform.DORotate(desRot, PreRotateDuration).SetEase(Ease.OutQuart)
                .SetUpdate(UpdateType.Fixed, true).OnUpdate(() =>
                {
                    bottleAnimationSpeedUp.CheckSpeedUp(_preRotate);

                    var angle = transform.rotation.eulerAngles.z;

                    transform.RotateAround(_chosenRotationPoint.position, Vector3.forward, lastAngeValue - angle);
                    lastAngeValue = angle;

                    bottleColorController.SetFillAmount(FillAmountCurve.Evaluate(angle));
                    bottleColorController.SetSARM(ScaleAndRotationMultiplierCurve.Evaluate(angle));
                });
        }

        private void RotateBottle(FillAndRotationValues fillAndRotationValues,
            BottleTransferController bottleTransferController,
            BottleData bottleData, BottleColorController bottleColorController,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, BottleController bottleController)
        {
            float lastAngleValue = 0;

            var numberOfEmptySpacesInSecondBottle = 4 - bottleTransferController.BottleData.NumberOfColorsInBottle;
            var rotateValue = fillAndRotationValues.GetRotationValue(bottleData, numberOfEmptySpacesInSecondBottle);
            var desRot = Vector3.forward * (_directionMultiplier * rotateValue);

            _rotateBottle = transform.DORotate(desRot, RotateBottleDuration).SetUpdate(UpdateType.Fixed, true).OnStart(
                () =>
                {
                    var angle = transform.rotation.eulerAngles.z;

                    if (fillAndRotationValues.GetFillCurrentAmount(bottleData) > FillAmountCurve.Evaluate(angle))
                    {
                        SetLineRenderer();
                        bottleTransferController.BottleColorController.SetFillAmount(angle);
                        bottleTransferController.BottleColorController.FillUp(FillAmountCurve.Evaluate(lastAngleValue) -
                                                                              FillAmountCurve.Evaluate(angle));
                    }

                    bottleTransferController.BottleColorController.SetSARM(
                        ScaleAndRotationMultiplierCurve.Evaluate(angle));

                    // if (Mathf.Abs(WrapAngle(transform.rotation.eulerAngles.z)) <=
                    //     FillAndRotationValues.RotationValues[_rotationIndex])
                    //     transform.RotateAround(_chosenRotationPoint.position, Vector3.forward,
                    //         lastAngleValue - angle);

                    lastAngleValue = angle;
                }).OnUpdate(() => { bottleAnimationSpeedUp.CheckSpeedUp(_rotateBottle); }).OnComplete(() =>
            {
                UpdateColorsAfterPouring(bottleColorController, bottleTransferController, bottleData,
                    bottleAnimationSpeedUp, bottleController);
            });
        }

        private void UpdateColorsAfterPouring(BottleColorController bottleColorController,
            BottleTransferController bottleTransferController, BottleData bottleData,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, BottleController bottleController)
        {
            bottleData.DecreaseNumberOfColorsInBottle(bottleTransferController.NumberOfColorsToTransfer);


            bottleTransferController.BottleTransferControllerRef.BottleData.IncreaseNumberOfColorsInBottle(
                bottleTransferController.NumberOfColorsToTransfer);


            bottleColorController.UpdateTopColorValues(bottleData);
            RotateBottleBackAndMoveOriginalPosition(bottleData, bottleAnimationSpeedUp, bottleColorController,
                bottleController);
        }

        private void RotateBottleBackAndMoveOriginalPosition(BottleData bottleData,
            BottleAnimationSpeedUp bottleAnimationSpeedUp, BottleColorController bottleColorController,
            BottleController bottleController)
        {
            transform.DOMove(OriginalPosition, MoveBottleDuration);

            _gm.ReleaseLineRenderer(_lineRenderer);

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
            }).OnComplete(() => { RemoveBottleFromInActionBottleList(bottleController); });
        }

        private void SetLineRenderer()
        {
            if (_lineRenderer.enabled) return;

            // set line position
            var position = _chosenRotationPoint.position;
            _lineRenderer.SetPosition(0, position);
            _lineRenderer.SetPosition(1, position - Vector3.up * LineRendererPouringDistance);

            // enable line renderer
            _lineRenderer.enabled = true;
        }

        private void RemoveBottleFromInActionBottleList(BottleController bottleController)
        {
            _gm.InActionBottleList.Remove(bottleController);
        }

        public List<Tween> GetAnimationTweens()
        {
            return new List<Tween>() { _moveTween, _preRotate, _rotateBottle, _rotateBottleBack };
        }


        // private float WrapAngle(float angle)
        // {
        //     angle %= 360;
        //     if (angle > 180)
        //         return angle - 360;
        //
        //     return angle;
        // }
    }
}
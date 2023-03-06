using DG.Tweening;
using UnityEngine;

namespace BottleCodes.Animation
{
    [RequireComponent(typeof(RotateAndMoveBackAnimation))]
    public class RotateAnimation : MonoBehaviour
    {
        [SerializeField] private float RotateBottleDuration = 1f;

        private Tween _rotateBottle;

        private BottleValueHolder _bottleValueHolder;

        private FillAndRotationValues _fillAndRotationValues;

        private AnimationCurve _fillAmountCurve;
        private AnimationCurve _scaleAndRotationMultiplierCurve;

        private RotateAndMoveBackAnimation _rotateAndMoveBackAnimation;


        private void Awake()
        {
            _bottleValueHolder = GetComponent<BottleValueHolder>();
            _fillAndRotationValues = FillAndRotationValues.Instance;
            _rotateAndMoveBackAnimation = GetComponent<RotateAndMoveBackAnimation>();
            _fillAmountCurve = _fillAndRotationValues.FillAmountCurve;
            _scaleAndRotationMultiplierCurve = _fillAndRotationValues.ScaleAndRotationMultiplierCurve;
        }

        public void RotateBottle(int beforePourAmount)
        {
            var bottleTransferController = _bottleValueHolder.BottleTransferController;
            var bottleAnimationSpeedUp = _bottleValueHolder.BottleAnimationSpeedUp;
            var bottleColorController = _bottleValueHolder.BottleColorController;
            var bottleData = _bottleValueHolder.BottleData;
            var startAngle = transform.eulerAngles.z;
            var angle = WrapAngle(startAngle);
            var lastAngleValue = WrapAngle(startAngle);
            var bottleControllerRef = bottleTransferController.BottleControllerRef;
            var numberOfEmptySpacesInSecondBottle = 4 - beforePourAmount;
            var rotateValue = _fillAndRotationValues.GetRotationValue(bottleData, numberOfEmptySpacesInSecondBottle);
            var desRot = _bottleValueHolder.BottleFindRotationPointAndDirection.DirectionMultiplier * rotateValue;
            var rotationPoint = _fillAndRotationValues.GetFillCurrentAmount(bottleData);
            var lastTransferAmount = bottleTransferController.NumberOfColorsToTransfer;
            var bottleFindRotationPointAndDirection = _bottleValueHolder.BottleFindRotationPointAndDirection;

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


                    transform.RotateAround(bottleFindRotationPointAndDirection.ChosenRotationPoint.position,
                        Vector3.forward, angle - lastAngleValue);


                    if (rotationPoint > _fillAmountCurve.Evaluate(Mathf.Abs(angle)))
                    {
                        var bottleLineRendererController = _bottleValueHolder.BottleLineRendererController;

                        bottleLineRendererController.SetLineRenderer(
                            bottleFindRotationPointAndDirection.ChosenRotationPoint,
                            bottleLineRendererController.LineRendererPouringDistance);

                        bottleColorController.SetFillAmount(_fillAmountCurve.Evaluate(Mathf.Abs(angle)));

                        bottleControllerRef.BottleColorController.FillUp(
                            _fillAmountCurve.Evaluate(Mathf.Abs(lastAngleValue)) -
                            _fillAmountCurve.Evaluate(Mathf.Abs(angle)));


                        var fillAmounts = bottleControllerRef.FillAndRotationValues.FillAmounts;

                        bottleControllerRef.BottleColorController.ClampFillAmount(
                            fillAmounts[0], fillAmounts[bottleControllerRef.BottleData.NumberOfColorsInBottle]);
                    }

                    bottleColorController.SetSARM(_scaleAndRotationMultiplierCurve.Evaluate(angle));

                    lastAngleValue = angle;
                }).OnComplete(() =>
                {
                    bottleColorController.UpdateTopColorValues(bottleData);

                    CheckBottlesAreSorted(_bottleValueHolder);

                    _bottleValueHolder.BottleLineRendererController.ReleaseLineRenderer();

                    _rotateAndMoveBackAnimation.RotateBottleBackAndMoveOriginalPosition(lastTransferAmount);

                });
        }

        private float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }

        private void CheckBottlesAreSorted(BottleValueHolder bottleValueHolder)
        {
            bottleValueHolder.BottleColorController.CheckIsBottleSorted(bottleValueHolder.BottleData);

            var bottleRef = bottleValueHolder.BottleTransferController.BottleControllerRef;
            bottleRef.BottleColorController.CheckIsBottleSorted(bottleRef.BottleData);
        }
    }
}
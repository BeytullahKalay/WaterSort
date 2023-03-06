using DG.Tweening;
using UnityEngine;

namespace BottleCodes.Animation
{
    public class PreRotateAnimation : MonoBehaviour
    {
        [SerializeField] private float _preRotateAmount;
        [SerializeField] private float _preRotateDuration;

        private BottleValueHolder _bottleValueHolder;

        private Tween _preRotate;

        private FillAndRotationValues _fillAndRotationValues;


        private AnimationCurve _fillAmountCurve;
        private AnimationCurve _scaleAndRotationMultiplierCurve;

        private void Awake()
        {
            _fillAndRotationValues = FillAndRotationValues.Instance;
            _bottleValueHolder = GetComponent<BottleValueHolder>();
            _fillAmountCurve = _fillAndRotationValues.FillAmountCurve;
            _scaleAndRotationMultiplierCurve = _fillAndRotationValues.ScaleAndRotationMultiplierCurve;
        }

        public void PlayPreRotateTween()
        {
            float angle = 0;
            float lastAngleValue = 0;
            var desRot = Vector3.forward * (_bottleValueHolder.BottleFindRotationPointAndDirection.DirectionMultiplier *
                                            _preRotateAmount);

            _preRotate = DOTween.To(() => angle, x => angle = x, desRot.z, _preRotateDuration)
                .SetEase(Ease.OutQuart).SetUpdate(UpdateType.Fixed, true).OnUpdate(() =>
                {
                    _bottleValueHolder.BottleAnimationSpeedUp.CheckSpeedUp(_preRotate);
                    
                    transform.RotateAround(
                        _bottleValueHolder.BottleFindRotationPointAndDirection.ChosenRotationPoint.position,
                        Vector3.forward, angle - lastAngleValue);

                    if (_fillAndRotationValues.GetFillCurrentAmount(_bottleValueHolder.BottleData) >
                        _fillAmountCurve.Evaluate(angle))
                    {
                        _bottleValueHolder.BottleColorController.SetFillAmount(_fillAmountCurve.Evaluate(angle));
                    }

                    _bottleValueHolder.BottleColorController.SetSARM(_scaleAndRotationMultiplierCurve.Evaluate(angle));

                    lastAngleValue = angle;
                });
        }
    }
}
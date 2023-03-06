using DG.Tweening;
using UnityEngine;

namespace BottleCodes.Animation
{
    public class RotateAndMoveBackAnimation : MonoBehaviour
    {
        [SerializeField] private float moveBottleDuration;

        private BottleValueHolder _bottleValueHolder;

        private BoxCollider2D _boxCollider2D;

        private FillAndRotationValues _fillAndRotationValues;

        private GameManager _gm;
        
        private Vector3 _startPosition;


        private void Awake()
        {
            _gm = GameManager.Instance;
            _fillAndRotationValues = FillAndRotationValues.Instance;
            _bottleValueHolder = GetComponent<BottleValueHolder>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            _startPosition = transform.position;
        }

        public void RotateBottleBackAndMoveOriginalPosition(int lastTransferAmount)
        {
            transform.DOMove(_startPosition, moveBottleDuration).OnStart(() =>
            {
                _bottleValueHolder.BottleAnimationSpeedUp.OnSpeedUp = false;

                var bottleRef = _bottleValueHolder.BottleTransferController.BottleControllerRef;
                var bottleRefData = bottleRef.BottleData;


                bottleRefData.ActionBottles.Remove(_bottleValueHolder.BottleController);

                if (bottleRefData.ActionBottles.Count <= 0)
                    bottleRef.BottleAnimationController.BottleIsLocked = false;

                if (bottleRefData.BottleSorted && bottleRefData.ActionBottles.Count <= 0)
                    bottleRef.BottleColorController.PlayParticleFX();
            }).OnComplete(() =>
            {
                _boxCollider2D.enabled = true;

                EventManager.AddMoveToList?.Invoke(_bottleValueHolder.BottleController,
                    _bottleValueHolder.BottleTransferController.BottleControllerRef,
                    lastTransferAmount, _bottleValueHolder.BottleData.PreviousTopColor);
            });


            var noColorInBottle = _bottleValueHolder.BottleData.NumberOfTopColorLayers <= 0;
            var startAngle = transform.eulerAngles.z;
            var angle = WrapAngle(startAngle);
            var lastAngleValue = WrapAngle(startAngle);

            DOTween.To(() => angle, x => angle = x, 0, moveBottleDuration)
                .SetUpdate(UpdateType.Fixed, true).OnStart(() =>
                {
                    if (noColorInBottle) _bottleValueHolder.BottleColorController.SetSARM(2);
                }).OnUpdate(() =>
                {
                    transform.RotateAround(
                        _bottleValueHolder.BottleFindRotationPointAndDirection.ChosenRotationPoint.position,
                        Vector3.forward, angle - lastAngleValue);

                    lastAngleValue = angle;

                    if (noColorInBottle) return;

                    _bottleValueHolder.BottleColorController.SetSARM(
                        _fillAndRotationValues.ScaleAndRotationMultiplierCurve.Evaluate(angle));
                }).OnComplete(() =>
                {
                    var bottleController = _bottleValueHolder.BottleController;

                    RemoveBottleFromInActionBottleList(bottleController);

                    bottleController.BottleSpriteRendererOrderController.ResetSortingOrder(
                        bottleController.BottleSpriteRenderer, bottleController.BottleMaskSR);
                });
        }

        private float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }

        private void RemoveBottleFromInActionBottleList(BottleController bottleController)
        {
            _gm.InActionBottleList.Remove(bottleController);
        }
    }
}
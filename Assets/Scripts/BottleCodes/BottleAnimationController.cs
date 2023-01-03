using System;
using DG.Tweening;
using UnityEngine;

namespace BottleCodes
{
    public class BottleAnimationController : MonoBehaviour
    {
        [Header("Animation Curves")] public AnimationCurve ScaleAndRotationMultiplierCurve;
        public AnimationCurve FillAmountCurve;


        [Header("Rotate Axis Values")]
        private Vector3 _originalPosition;
        private float _directionMultiplier = 1;
    
        [Header("Animation Values")] [SerializeField]
        private float lineRendererPouringDistance = 1f;

        public float MoveBottleDuration = 5f;
        public float RotateBottleDuration = 1f;
        public float PreRotateDuration = .25f;
        public float PreRotateAmount = 15f;
        public float BottlePouringDistanceIncreasor = .25f;
       
        [Header("Speed Up Values")]
        [SerializeField] private float speedMultiplier = 10f;
        public bool OnSpeedUp;

        private Tween _moveTween;
        private Tween _rotateBottle;
        private Tween _preRotate;
        private Tween _rotateBottleBack;

        private BoxCollider2D _collider2D;
        
        private LineRenderer _lineRenderer;

        private GameManager _gm;

        private BottleManager _thisBottleManager;


        private void Awake()
        {
            _collider2D = GetComponent<BoxCollider2D>();
            _thisBottleManager = GetComponent<BottleManager>();
            _gm = GameManager.Instance;
        }

        public void MoveBottle(ChosenPoint chosenPoint,BottleManager bottleManagerRef)
        {
            _collider2D.enabled = false;
            
            // if chosen position is left go right
            if (chosenPoint == ChosenPoint.Left)
            {
                Vector3 movePos = bottleManagerRef.BottleRotationPointChooser.RightRotationPoint.position;
                movePos.x += BottlePouringDistanceIncreasor;
                _moveTween = transform.DOMove(movePos, MoveBottleDuration);
            }
            else // if chose position is right go left
            {
                Vector3 movePos = bottleManagerRef.BottleRotationPointChooser.LeftRotationPoint.position;
                movePos.x -= BottlePouringDistanceIncreasor;
                _moveTween = transform.DOMove(movePos, MoveBottleDuration);
            }

            // set line renderer start and end color
            _lineRenderer = _gm.GetLineRenderer();
            _lineRenderer.startColor =_thisBottleManager.BottleColorManager.TopColor;
            _lineRenderer.endColor = _thisBottleManager.BottleColorManager.TopColor;

            // decrease number of colors in first bottle
            _thisBottleManager.BottleColorManager.BottleColorsAmount -= _thisBottleManager.GetNumberOfColorsToTransfer();

            // increase number of colors in seconds bottle
            bottleManagerRef.BottleColorManager.BottleColorsAmount += _thisBottleManager.GetNumberOfColorsToTransfer();

            // lock seconds bottle while action and on completed call rotate bottle
            _moveTween.OnStart(() =>
                {
                    CheckSpeedUp(_moveTween);

                    bottleManagerRef.BottleColorManager.UpdateTopColorValues();
                }).SetUpdate(UpdateType.Fixed, true)
                .OnUpdate(() => { bottleManagerRef.BottleIsLocked = true; }).OnComplete(RotateBottle);
        }

        private void RotateBottle()
        {
            // TODO: rotate bottle is not defined
            Debug.Log("Rotate bottle");
        }
        
        private void CheckSpeedUp(Tween comingTween)
        {
            if (OnSpeedUp)
                comingTween.timeScale = speedMultiplier;
        }
    }
}

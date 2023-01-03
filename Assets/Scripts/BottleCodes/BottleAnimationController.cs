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
    
    
        private Tween _moveTween;
        private Tween _rotateBottle;
        private Tween _preRotate;
        private Tween _rotateBottleBack;

    }
}

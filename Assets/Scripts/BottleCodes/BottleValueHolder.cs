using BottleCodes.Animation;
using DG.Tweening;
using UnityEngine;

namespace BottleCodes
{
    [RequireComponent(typeof(BottleData))]
    [RequireComponent(typeof(BottleController))]
    [RequireComponent(typeof(BottleColorController))]
    [RequireComponent(typeof(BottleFindRotationPointAndDirection))]
    [RequireComponent(typeof(BottleAnimationSpeedUp))]
    [RequireComponent(typeof(BottleLineRendererController))]
    [RequireComponent(typeof(BottleTransferController))]
    [RequireComponent(typeof(BottleSpriteRendererOrderController))]
    [RequireComponent(typeof(BottleLineRendererController))]

    public class BottleValueHolder : MonoBehaviour
    {
        public BottleData BottleData { get; private set; }
        public BottleController BottleController { get; private set; }
        public BottleColorController BottleColorController { get; private set; }
        public BottleFindRotationPointAndDirection BottleFindRotationPointAndDirection { get; private set; }
        public BottleAnimationSpeedUp BottleAnimationSpeedUp { get; private set; }
        public BottleLineRendererController BottleLineRendererController { get; private set; }
        public BottleTransferController BottleTransferController { get; private set; }
        public BottleSpriteRendererOrderController BottleSpriteRendererOrderController { get; private set; }
        public Tween SelectedTween { get; set; }
        public Vector3 OriginalPosition { get; private set; }
        
        private void Awake()
        {
            BottleData = GetComponent<BottleData>();
            BottleController = GetComponent<BottleController>();
            BottleColorController = GetComponent<BottleColorController>();
            BottleFindRotationPointAndDirection = GetComponent<BottleFindRotationPointAndDirection>();
            BottleAnimationSpeedUp = GetComponent<BottleAnimationSpeedUp>();
            BottleLineRendererController = GetComponent<BottleLineRendererController>();
            BottleTransferController = GetComponent<BottleTransferController>();
            BottleSpriteRendererOrderController = GetComponent<BottleSpriteRendererOrderController>();
        }

        private void Start()
        {
            OriginalPosition = transform.position;
        }
    }
}

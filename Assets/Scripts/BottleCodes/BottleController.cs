using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace BottleCodes
{
    [RequireComponent(typeof(BottleData))]
    [RequireComponent(typeof(FillAndRotationValues))]
    [RequireComponent(typeof(BottleColorController))]
    [RequireComponent(typeof(BottleAnimationController))]
    [RequireComponent(typeof(BottleTransferController))]
    [RequireComponent(typeof(BottleAnimationSpeedUp))]
    public class BottleController : MonoBehaviour
    {
        public BottleData BottleData { get; private set; }
        public FillAndRotationValues FillAndRotationValues { get; private set; }
        public BottleColorController BottleColorController { get; private set; }
        public BottleAnimationController BottleAnimationController { get; private set; }
        public BottleTransferController BottleTransferController { get; private set; }
        public BottleAnimationSpeedUp BottleAnimationSpeedUp { get; private set; }


        [Header("Bottle Sprite Renderer")] public SpriteRenderer BottleMaskSR;


        //[Header("Transfer Values")] public BottleController BottleControllerRef;
        public Transform LeftRotationPoint;
        public Transform RightRotationPoint;


        //private LineRenderer _lineRenderer;
        private int _numberOfColorsToTransfer = 0;

        [Header("Locker Values")] public bool BottleIsLocked;


        // Game manager
        private GameManager _gm;

        private SpriteRenderer _bottleSpriteRenderer;

        [Header("Bottle Helper")] [SerializeField]
        public Bottle HelperBottle;

        private void Awake()
        {
            BottleData = GetComponent<BottleData>();
            FillAndRotationValues = GetComponent<FillAndRotationValues>();
            BottleColorController = GetComponent<BottleColorController>();
            BottleAnimationController = GetComponent<BottleAnimationController>();
            BottleTransferController = GetComponent<BottleTransferController>();
            BottleAnimationSpeedUp = GetComponent<BottleAnimationSpeedUp>();

            _bottleSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            _gm = GameManager.Instance;
            BottleMaskSR.material = _gm.Mat;

            BottleColorController.SetFillAmount(FillAndRotationValues.GetFillCurrentAmount(BottleData));

            BottleColorController.UpdateColorsOnShader(BottleData);

            BottleColorController.UpdateTopColorValues(BottleData);
        }

        public void UpdateAfterUndo()
        {
            BottleColorController.SetFillAmount(FillAndRotationValues.GetFillCurrentAmount(BottleData));
            BottleColorController.UpdateColorsOnShader(BottleData);
            BottleData.TopColor = BottleData.PreviousTopColor;
            BottleColorController.UpdateTopColorValues(BottleData);
        }

        public bool IsBottleEmpty()
        {
            return BottleData.NumberOfColorsInBottle <= 0;
        }

        public void StartColorTransfer()
        {
            AddActionBottleToActionBottleList();

            // chose rotation point and direction
            BottleAnimationController.ChoseRotationPointAndDirection(this);

            var bottleControllerRef = BottleTransferController.BottleTransferControllerRef;
            var bottleRefData = bottleControllerRef.BottleData;

            // get how many water color will pour
            _numberOfColorsToTransfer = Mathf.Min(BottleData.NumberOfTopColorLayers,
                4 - bottleRefData.NumberOfColorsInBottle);

            // setting array color values to pouring water color
            for (var i = 0; i < BottleTransferController.NumberOfColorsToTransfer; i++)
            {
                bottleRefData.BottleColors[bottleRefData.NumberOfColorsInBottle + i] = BottleData.TopColor;
            }

            // updating colors on shader
            bottleControllerRef.BottleColorController.UpdateColorsOnShader(bottleRefData);

            // setting render order
            SetSpriteRendererSortingOrders();

            // call move bottle
            MoveBottle();

            // call pre rotate bottle
            PreRotateBottle();
        }

        private void SetSpriteRendererSortingOrders()
        {
            _bottleSpriteRenderer.sortingOrder += 2; // default bottle renderer sorting order
            BottleMaskSR.sortingOrder += 2; // liquid sprite renderer order
        }

        private void AddActionBottleToActionBottleList()
        {
            _gm.InActionBottleList.Add(this);
        }

        private void MoveBottle()
        {
            BottleAnimationController.DisableCollider();
            BottleAnimationController.ChoseMovePosition(BottleTransferController);

            // decrease number of colors in first bottle
            BottleData.NumberOfColorsInBottle -= _numberOfColorsToTransfer;

            // increase number of colors in seconds bottle
            BottleTransferController.BottleData.NumberOfColorsInBottle += _numberOfColorsToTransfer;

            // lock seconds bottle while action and on completed call rotate bottle
            BottleAnimationController.PlayMoveTween(BottleTransferController, BottleData, FillAndRotationValues,
                BottleColorController, BottleAnimationSpeedUp, this);
        }

        private void PreRotateBottle()
        {
            BottleAnimationController.PlayPreRotateTween(BottleColorController, BottleAnimationSpeedUp);
        }


        private float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }

        public void SetOriginalPositionTo(Vector3 position)
        {
            BottleAnimationController.OriginalPosition = position;
        }
    }
}
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace BottleCodes
{
    [RequireComponent(typeof(BottleData))]
    [RequireComponent(typeof(FillAndRotationValues))]
    [RequireComponent(typeof(BottleColorController))]
    [RequireComponent(typeof(BottleAnimationController))]
    public class BottleController : MonoBehaviour
    {
        public BottleData BottleData { get; private set; }
        public FillAndRotationValues FillAndRotationValues { get; private set; }
        public BottleColorController BottleColorController { get; private set; }
        public BottleAnimationController BottleAnimationController { get; private set; }
        
        
        
        [Header("Bottle Sprite Renderer")] public SpriteRenderer BottleMaskSR;

        private int _rotationIndex = 0;


        [Header("Rotate Axis Values")] private Vector3 _originalPosition;
        private float _directionMultiplier = 1;


        [Header("Transfer Values")] public BottleController BottleControllerRef;
        public Transform LeftRotationPoint;
        public Transform RightRotationPoint;
        private Transform _chosenRotationPoint;
        private LineRenderer _lineRenderer;
        private int _numberOfColorsToTransfer = 0;

        [Header("Locker Values")] public bool BottleIsLocked;

        [Header("Animation Values")] [SerializeField]
        private float lineRendererPouringDistance = 1f;

        public float MoveBottleDuration = 5f;
        public float RotateBottleDuration = 1f;
        public float PreRotateDuration = .25f;
        public float PreRotateAmount = 15f;
        public float BottlePouringDistanceIncreasor = .25f;

        [Header("Speed Up Values")] [SerializeField]
        private float speedMultiplier = 10f;

        private Tween _selectedTween;
        private Tween _moveTween;
        private Tween _rotateBottle;
        private Tween _preRotate;
        private Tween _rotateBottleBack;
        public bool OnSpeedUp;

        // Game manager
        private GameManager _gm;

        private Camera _camera;

        [Header("Bottle Helper")] [SerializeField]
        public Bottle HelperBottle;

        private void Awake()
        {
            BottleData = GetComponent<BottleData>();
            FillAndRotationValues = GetComponent<FillAndRotationValues>();
            BottleColorController = GetComponent<BottleColorController>();
            BottleAnimationController = GetComponent<BottleAnimationController>();
            
            _camera = Camera.main;
        }

        private void Start()
        {
            _gm = GameManager.Instance;
            BottleMaskSR.material = _gm.Mat;
            

            BottleColorController.SetFillAmount(FillAndRotationValues.GetFillCurrentAmount(BottleData));

            _originalPosition = transform.position;
            
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

        public void OnSelected()
        {
            _selectedTween?.Kill();
            _selectedTween = transform.DOMoveY(_originalPosition.y + .5f, .25f);
        }

        public void OnSelectionCanceled()
        {
            _selectedTween?.Kill();
            _selectedTween = transform.DOMove(_originalPosition, .25f);
        }

        public bool IsBottleEmpty()
        {
            return BottleData.NumberOfColorsInBottle <= 0;
        }

        public bool FillBottleCheck(Color colorToCheck)
        {
            if (BottleData.NumberOfColorsInBottle == 0)
            {
                return true;
            }
            else
            {
                if (BottleData.NumberOfColorsInBottle == 4)
                {
                    return false;
                }
                else
                {
                    if (BottleData.TopColor.Equals(colorToCheck))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public void StartColorTransfer()
        {
            AddActionBottleToActionBottleList();

            // chose rotation point and direction
            ChoseRotationPointAndDirection();

            // get how many water color will pour
            _numberOfColorsToTransfer = Mathf.Min(BottleData.NumberOfTopColorLayers, 4 - BottleControllerRef.BottleData.NumberOfColorsInBottle);

            // setting array color values to pouring water color
            for (int i = 0; i < _numberOfColorsToTransfer; i++)
            {
                BottleControllerRef.BottleData.BottleColors[BottleControllerRef.BottleData.NumberOfColorsInBottle + i] = BottleData.TopColor;
            }

            // updating colors on shader
            BottleControllerRef.BottleColorController.UpdateTopColorValues(BottleControllerRef.BottleData);


            // calculating rotation index 
            CalculateRotationIndex(4 - BottleControllerRef.BottleData.NumberOfColorsInBottle);

            // setting render order
            transform.GetComponent<SpriteRenderer>().sortingOrder += 2; // default bottle renderer sorting order
            BottleMaskSR.sortingOrder += 2; // liquid sprite renderer order

            // call move bottle
            MoveBottle();

            // call pre rotate bottle
            PreRotateBottle();
        }

        private void AddActionBottleToActionBottleList()
        {
            _gm.InActionBottleList.Add(this);
        }

        private void ChoseRotationPointAndDirection()
        {
            var minBottleDistanceToCorner = 1f;
        
            var leftOfScreen = _camera.ViewportToWorldPoint(Vector3.zero).x;
            var rightOfScreen = _camera.ViewportToWorldPoint(Vector3.one).x;

            var distanceToLeft = Mathf.Abs(BottleControllerRef.transform.position.x - leftOfScreen);
            var distanceToRight = Mathf.Abs(BottleControllerRef.transform.position.x - rightOfScreen);
        
            if (transform.position.x > BottleControllerRef.transform.position.x)
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

        private void MoveBottle()
        {
            GetComponent<BoxCollider2D>().enabled = false;

            // if chosen position is left go right
            if (_chosenRotationPoint == LeftRotationPoint)
            {
                Vector3 movePos = BottleControllerRef.RightRotationPoint.position;
                movePos.x += BottlePouringDistanceIncreasor;
                _moveTween = transform.DOMove(movePos, MoveBottleDuration);
            }
            else // if chose position is right go left
            {
                Vector3 movePos = BottleControllerRef.LeftRotationPoint.position;
                movePos.x -= BottlePouringDistanceIncreasor;
                _moveTween = transform.DOMove(movePos, MoveBottleDuration);
            }

            // set line renderer start and end color
            _lineRenderer = _gm.GetLineRenderer();
            _lineRenderer.startColor = BottleData.TopColor;
            _lineRenderer.endColor = BottleData.TopColor;

            // decrease number of colors in first bottle
            BottleData.NumberOfColorsInBottle -= _numberOfColorsToTransfer;

            // increase number of colors in seconds bottle
            BottleControllerRef.BottleData.NumberOfColorsInBottle += _numberOfColorsToTransfer;

            // lock seconds bottle while action and on completed call rotate bottle
            _moveTween.OnStart(() =>
                {
                    _selectedTween?.Kill();
                    CheckSpeedUp(_moveTween);

                    //BottleControllerRef.UpdateTopColorValues();
                    BottleControllerRef.BottleColorController.UpdateTopColorValues(BottleControllerRef.BottleData);

                }).SetUpdate(UpdateType.Fixed, true)
                .OnUpdate(() => { BottleControllerRef.BottleIsLocked = true; }).OnComplete(RotateBottle);
        }

        private void CheckSpeedUp(Tween comingTween)
        {
            if (OnSpeedUp)
                comingTween.timeScale = speedMultiplier;
        }

        private void PreRotateBottle() // Do pre-rotation on direction
        {
            float angle = 0;
            float lastAngeValue = 0;

            _preRotate = DOTween.To(() => angle, x => angle = x, _directionMultiplier * PreRotateAmount, PreRotateDuration)
                .SetEase(Ease.OutQuart).SetUpdate(UpdateType.Fixed, true).OnUpdate(() =>
                {
                    transform.RotateAround(_chosenRotationPoint.position, Vector3.forward, lastAngeValue - angle);
                    lastAngeValue = angle;
                });
        }

        private void RotateBottle()
        {
            float angle = 0;
            float lastAngeValue = 0;

            _rotateBottle = DOTween.To(() => angle, x => angle = x, _directionMultiplier * FillAndRotationValues.RotationValues[_rotationIndex],
                RotateBottleDuration).SetUpdate(UpdateType.Fixed, true).OnStart(() => CheckSpeedUp(_rotateBottle)).OnUpdate(
                () =>
                {
                    if (FillAndRotationValues.FillAmounts[BottleData.NumberOfColorsInBottle + _numberOfColorsToTransfer] >
                        BottleAnimationController.FillAmountCurve.Evaluate(angle) + 0.005f)
                    {
                        if (!_lineRenderer.enabled)
                        {
                            // set line position
                            _lineRenderer.SetPosition(0, _chosenRotationPoint.position);
                            _lineRenderer.SetPosition(1,
                                _chosenRotationPoint.position - Vector3.up * lineRendererPouringDistance);

                            // enable line renderer
                            _lineRenderer.enabled = true;
                        }

                        BottleMaskSR.material.SetFloat("_FillAmount",  BottleAnimationController.FillAmountCurve.Evaluate(angle));
                        BottleMaskSR.material.SetFloat("_SARM", BottleAnimationController.ScaleAndRotationMultiplierCurve.Evaluate(angle));


                        BottleControllerRef.FillUp(
                            BottleAnimationController.FillAmountCurve.Evaluate(lastAngeValue) - BottleAnimationController.FillAmountCurve.Evaluate(angle),
                            FillAndRotationValues.FillAmounts[BottleControllerRef.BottleData.NumberOfColorsInBottle]);
                    }


                    if (Mathf.Abs(WrapAngle(transform.rotation.eulerAngles.z)) <= FillAndRotationValues.RotationValues[_rotationIndex])
                        transform.RotateAround(_chosenRotationPoint.position, Vector3.forward, lastAngeValue - angle);


                    lastAngeValue = angle;

                    BottleControllerRef.BottleIsLocked = true;
                }).OnComplete(() =>
            {
                angle = _directionMultiplier * FillAndRotationValues.RotationValues[_rotationIndex];
                BottleMaskSR.material.SetFloat("_SARM", BottleAnimationController.ScaleAndRotationMultiplierCurve.Evaluate(angle));
                BottleMaskSR.material.SetFloat("_FillAmount", BottleAnimationController.FillAmountCurve.Evaluate(angle));

                _lineRenderer.enabled = false;
                //UpdateTopColorValues();
                BottleColorController.UpdateTopColorValues(BottleData);
                RotateBottleBackAndMoveOriginalPosition();
            });
        }

        private float WrapAngle(float angle)
        {
            angle %= 360;
            if (angle > 180)
                return angle - 360;

            return angle;
        }

        private void RotateBottleBackAndMoveOriginalPosition()
        {
            transform.DOMove(_originalPosition, MoveBottleDuration);
            transform.DORotate(Vector3.zero, RotateBottleDuration);
            _gm.ReleaseLineRenderer(_lineRenderer);

            var angle = 0;
            _rotateBottleBack = DOTween.To(() => angle, x => angle = x, 1, RotateBottleDuration)
                .OnStart(() => { CheckSpeedUp(_rotateBottleBack); }).OnUpdate(() =>
                {
                    BottleMaskSR.material.SetFloat("_SARM", BottleAnimationController.ScaleAndRotationMultiplierCurve.Evaluate(angle));
                }).OnComplete(() =>
                {
                    GetComponent<BoxCollider2D>().enabled = true;
                    OnSpeedUp = false;
                    EventManager.AddMoveToList?.Invoke(this, BottleControllerRef, _numberOfColorsToTransfer,
                        BottleData.PreviousTopColor);
                    RemoveBottleFromInActionBottleList();
                });

            transform.GetComponent<SpriteRenderer>().sortingOrder -= 2;
            BottleMaskSR.sortingOrder -= 2;
            BottleControllerRef.BottleIsLocked = false;
            BottleControllerRef.BottleData.ActionBottles.Remove(this);
            BottleControllerRef.BottleColorController.CheckIsBottleSorted(BottleControllerRef.BottleData);

            // change top water color for not glitching on fast selection situations
            BottleData.BottleColors[BottleData.NumberOfColorsInBottle] = BottleData.TopColor;
        }

        private void RemoveBottleFromInActionBottleList()
        {
            _gm.InActionBottleList.Remove(this);
        }

        private void FillUp(float fillAmountToAdd, float desFillAmount)
        {
            if (BottleMaskSR.material.GetFloat("_FillAmount") >= desFillAmount) return;

            float currentFillAmount = BottleMaskSR.material.GetFloat("_FillAmount");
            currentFillAmount += fillAmountToAdd;

            if (currentFillAmount > desFillAmount)
            {
                BottleMaskSR.material.SetFloat("_FillAmount", desFillAmount);
            }
            else
            {
                BottleMaskSR.material.SetFloat("_FillAmount",
                    BottleMaskSR.material.GetFloat("_FillAmount") + fillAmountToAdd);
            }
        }

        private void CalculateRotationIndex(int numberOfEmptySpacesInSecondBottle)
        {
            _rotationIndex = 3 - (BottleData.NumberOfColorsInBottle -
                                  Mathf.Min(numberOfEmptySpacesInSecondBottle,BottleData.NumberOfTopColorLayers));
        }

        public async Task SpeedUpActions()
        {
            var ActionBottles = BottleData.ActionBottles;
            
            var tasks = new Task[ActionBottles.Count];

            for (int i = 0; i < ActionBottles.Count; i++)
            {
                tasks[i] = ActionBottles[i].SpeedUp();
            }

            await Task.WhenAll(tasks);

            SetSpeedToNormalSpeed();

            // TODO: Fazlaliklar icin duzeltiyor!
            BottleMaskSR.material.SetFloat("_FillAmount", FillAndRotationValues.FillAmounts[BottleData.NumberOfColorsInBottle]);
        }

        private async Task SpeedUp()
        {
            OnSpeedUp = true;

            while (OnSpeedUp)
            {
                if (_preRotate != null)
                    _preRotate.timeScale = speedMultiplier;

                if (_moveTween != null)
                    _moveTween.timeScale = speedMultiplier;

                if (_rotateBottle != null)
                    _rotateBottle.timeScale = speedMultiplier;

                if (_rotateBottleBack != null)
                    _rotateBottleBack.timeScale = speedMultiplier;

                await Task.Yield();
            }
        }

        private void SetSpeedToNormalSpeed()
        {
            if (_preRotate != null)
                _preRotate.timeScale = 1f;

            if (_moveTween != null)
                _moveTween.timeScale = 1f;

            if (_rotateBottle != null)
                _rotateBottle.timeScale = 1f;

            if (_rotateBottleBack != null)
                _rotateBottleBack.timeScale = 1f;
        }

        public void SetOriginalPositionTo(Vector3 position)
        {
            _originalPosition = position;
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    [Header("Bottle Sprite Renderer")] public SpriteRenderer BottleMaskSR;

    [Header("Bottle Values")] [Range(0, 4)]
    public int NumberOfColorsInBottle = 4;

    public Color[] BottleColors;
    public Color TopColor;

    public float[] FillAmounts;
    public float[] RotationValues;

    public int NumberOfTopColorLayers = 0;
    private int _rotationIndex = 0;

    public bool BottleSorted;

    // Undo values
    public Color _previousTopColor;


    [Header("Animation Curves")] public AnimationCurve ScaleAndRotationMultiplierCurve;
    public AnimationCurve FillAmountCurve;


    [Header("Rotate Axis Values")] private Vector3 _originalPosition;
    private float _directionMultiplier = 1;


    [Header("Transfer Values")] public BottleController BottleControllerRef;
    public Transform LeftRotationPoint;
    public Transform RightRotationPoint;
    private Transform _chosenRotationPoint;
    private LineRenderer _lineRenderer;
    private int _numberOfColorsToTransfer = 0;

    [Header("Locker Values")] public bool BottleIsLocked;
    private IEnumerator _coroutine;

    [Header("Animation Values")] [SerializeField]
    private float lineRendererPouringDistance = 1f;

    public float MoveBottleDuration = 5f;
    public float RotateBottleDuration = 1f;
    public float PreRotateDuration = .25f;
    public float PreRotateAmount = 15f;
    public float BottlePouringDistanceIncreasor = .25f;

    [Header("Speed Up Values")] [SerializeField]
    private float speedMultiplier = 10f;

    public List<BottleController> ActionBottles = new List<BottleController>();
    private Tween _moveTween;
    private Tween _rotateBottle;
    private Tween _preRotate;
    private Tween _rotateBottleBack;
    public bool OnSpeedUp;

    // Game manager
    private GameManager _gm;

    [Header("Bottle Helper")] [SerializeField]
    public Bottle HelperBottle;

    private void Start()
    {
        _gm = GameManager.Instance;
        BottleMaskSR.material = _gm.Mat;
        _coroutine = CheckIsBottleSorted_Co();
        BottleMaskSR.material.SetFloat("_FillAmount", FillAmounts[NumberOfColorsInBottle]);
        _originalPosition = transform.position;
        UpdateColorsOnShader(); // setting colors
        UpdateTopColorValues(); // setting Top Color values
    }

    public void UpdateAfterUndo()
    {
        BottleMaskSR.material.SetFloat("_FillAmount", FillAmounts[NumberOfColorsInBottle]);
        UpdateColorsOnShader(); // setting colors
        TopColor = _previousTopColor;
        UpdateTopColorValues(); // setting Top Color values
    }

    private void UpdateColorsOnShader()
    {
        BottleMaskSR.material.SetColor("_C1", BottleColors[0]);
        BottleMaskSR.material.SetColor("_C2", BottleColors[1]);
        BottleMaskSR.material.SetColor("_C3", BottleColors[2]);
        BottleMaskSR.material.SetColor("_C4", BottleColors[3]);
    }

    private void UpdateTopColorValues()
    {
        _previousTopColor = TopColor;
        BottleSorted = false;
        if (NumberOfColorsInBottle != 0)
        {
            NumberOfTopColorLayers = 1;

            if (NumberOfColorsInBottle == 4)
            {
                if (BottleColors[3].Equals(BottleColors[2]))
                {
                    NumberOfTopColorLayers = 2;
                    if (BottleColors[2].Equals(BottleColors[1]))
                    {
                        NumberOfTopColorLayers = 3;
                        if (BottleColors[1].Equals(BottleColors[0]))
                        {
                            NumberOfTopColorLayers = 4;
                            print("Bottle Sorted!");
                            BottleSorted = true;
                            StartCoroutine(_coroutine);
                        }
                    }
                }
            }

            else if (NumberOfColorsInBottle == 3)
            {
                if (BottleColors[2].Equals(BottleColors[1]))
                {
                    NumberOfTopColorLayers = 2;
                    if (BottleColors[1].Equals(BottleColors[0]))
                    {
                        NumberOfTopColorLayers = 3;
                    }
                }
            }

            else if (NumberOfColorsInBottle == 2)
            {
                if (BottleColors[1].Equals(BottleColors[0]))
                {
                    NumberOfTopColorLayers = 2;
                }
            }


            // _rotationIndex = 3 - (NumberOfColorsInBottle - NumberOfTopColorLayers);
            // Debug.Log("In UpdateTopColorValues: " + _rotationIndex);


            TopColor = BottleColors[NumberOfColorsInBottle - 1];
        }
    }

    public void OnSelected()
    {
        transform.DOMoveY(_originalPosition.y + .5f, .25f);
    }

    public void OnSelectionCanceled()
    {
        transform.DOMove(_originalPosition, .25f);
    }

    public bool IsBottleEmpty()
    {
        return NumberOfColorsInBottle <= 0;
    }

    public bool FillBottleCheck(Color colorToCheck)
    {
        if (NumberOfColorsInBottle == 0)
        {
            return true;
        }
        else
        {
            if (NumberOfColorsInBottle == 4)
            {
                return false;
            }
            else
            {
                if (TopColor.Equals(colorToCheck))
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
        _numberOfColorsToTransfer = Mathf.Min(NumberOfTopColorLayers, 4 - BottleControllerRef.NumberOfColorsInBottle);

        // setting array color values to pouring water color
        for (int i = 0; i < _numberOfColorsToTransfer; i++)
        {
            BottleControllerRef.BottleColors[BottleControllerRef.NumberOfColorsInBottle + i] = TopColor;
        }

        // updating colors on shader
        BottleControllerRef.UpdateColorsOnShader();

        // calculating rotation index 
        CalculateRotationIndex(4 - BottleControllerRef.NumberOfColorsInBottle);

        // setting render order
        transform.GetComponent<SpriteRenderer>().sortingOrder += 2;
        BottleMaskSR.sortingOrder += 2;

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
        if (transform.position.x > BottleControllerRef.transform.position.x)
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
        _lineRenderer.startColor = TopColor;
        _lineRenderer.endColor = TopColor;

        // decrease number of colors in first bottle
        NumberOfColorsInBottle -= _numberOfColorsToTransfer;

        // increase number of colors in seconds bottle
        BottleControllerRef.NumberOfColorsInBottle += _numberOfColorsToTransfer;

        // lock seconds bottle while action and on completed call rotate bottle
        _moveTween.OnStart(() =>
            {
                CheckSpeedUp(_moveTween);

                BottleControllerRef.UpdateTopColorValues();
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

        _rotateBottle = DOTween.To(() => angle, x => angle = x, _directionMultiplier * RotationValues[_rotationIndex],
            RotateBottleDuration).SetUpdate(UpdateType.Fixed, true).OnStart(() => CheckSpeedUp(_rotateBottle)).OnUpdate(
            () =>
            {
                if (FillAmounts[NumberOfColorsInBottle + _numberOfColorsToTransfer] >
                    FillAmountCurve.Evaluate(angle) + 0.005f)
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

                    BottleMaskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angle));
                    BottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angle));


                    BottleControllerRef.FillUp(
                        FillAmountCurve.Evaluate(lastAngeValue) - FillAmountCurve.Evaluate(angle),
                        FillAmounts[BottleControllerRef.NumberOfColorsInBottle]);
                }


                if (Mathf.Abs(WrapAngle(transform.rotation.eulerAngles.z)) <= RotationValues[_rotationIndex])
                    transform.RotateAround(_chosenRotationPoint.position, Vector3.forward, lastAngeValue - angle);


                lastAngeValue = angle;

                BottleControllerRef.BottleIsLocked = true;
            }).OnComplete(() =>
        {
            angle = _directionMultiplier * RotationValues[_rotationIndex];
            BottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angle));
            BottleMaskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angle));

            _lineRenderer.enabled = false;
            UpdateTopColorValues();
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
                BottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angle));
            }).OnComplete(() =>
            {
                GetComponent<BoxCollider2D>().enabled = true;
                OnSpeedUp = false;
                EventManager.AddMoveToList?.Invoke(this, BottleControllerRef, _numberOfColorsToTransfer, _previousTopColor);
                RemoveBottleFromInActionBottleList();
            });

        transform.GetComponent<SpriteRenderer>().sortingOrder -= 2;
        BottleMaskSR.sortingOrder -= 2;
        BottleControllerRef.BottleIsLocked = false;
        BottleControllerRef.ActionBottles.Remove(this);

        // change top water color for not glitching on fast selection situations
        BottleColors[NumberOfColorsInBottle] = TopColor;
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
        _rotationIndex = 3 - (NumberOfColorsInBottle -
                              Mathf.Min(numberOfEmptySpacesInSecondBottle, NumberOfTopColorLayers));
    }

    public async Task SpeedUpActions()
    {
        var tasks = new Task[ActionBottles.Count];

        for (int i = 0; i < ActionBottles.Count; i++)
        {
            tasks[i] = ActionBottles[i].SpeedUp();
        }

        await Task.WhenAll(tasks);

        SetSpeedToNormalSpeed();

        // TODO: Fazlaliklar icin duzeltiyor!
        BottleMaskSR.material.SetFloat("_FillAmount", FillAmounts[NumberOfColorsInBottle]);
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

    private IEnumerator CheckIsBottleSorted_Co()
    {
        while (true)
        {
            if (ActionBottles.Count == 0)
            {
                GameObject particleFX = Instantiate(_gm.ConfettiParticle,
                    transform.position + new Vector3(0, .25f, -1),
                    _gm.ConfettiParticle.transform.rotation);
                Destroy(particleFX, 3);
                EventManager.CheckIsLevelCompleted?.Invoke();
                StopCoroutine(_coroutine);
            }

            yield return null;
        }
    }

    public void SetOriginalPositionTo(Vector3 position)
    {
        _originalPosition = position;
    }
}
using DG.Tweening;
using UnityEngine;

public class MyBottleControllerTest : MonoBehaviour
{
    [Header("Bottle Sprite Renderer")] public SpriteRenderer BottleMaskSR;


    [Header("Bottle Values")] [Range(0, 4)]
    public int NumberOfColorsInBottle = 4;

    public Color[] BottleColors;
    public Color TopColor;
    public float[] FillAmounts;
    public float[] RotationValues;
    public int NumberOfTopColorLayers = 0;
    private int rotationIndex = 0;


    [Header("Animation Curves")] public AnimationCurve ScaleAndRotationMultiplierCurve;
    public AnimationCurve FillAmountCurve;


    [Header("Rotate Axis Values")] private Vector3 originalPosition;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float directionMultiplier = 1;


    [Header("Transfer Values")] public MyBottleControllerTest BottleControllerRef;
    public Transform LeftRotationPoint;
    public Transform RightRotationPoint;
    private Transform _chosenRotationPoint;
    private float _direcitonMultiplier = 1;
    public LineRenderer LineRenderer;
    private int _numberOfColorsToTransfer = 0;


    [Header("Locker Values")] public bool BottleIsLocked;

    [Header("Animation Values")] public float MoveBottleDuration = 5f;
    public float RotateBottleDuration = 1f;
    public float PreRotateDuration = .25f;
    public float PreRotateAmount = 15f;
    public float BottlePouringDistanceIncreasor = .25f;

    [Space(10)] public bool IsBottleInAction = false;


    private void Start()
    {
        BottleMaskSR.material.SetFloat("_FillAmount", FillAmounts[NumberOfColorsInBottle]);
        originalPosition = transform.position;
        UpdateColorsOnShader(); // setting colors
        UpdateTopColorValues(); // setting Top Color values
    }

    private void UpdateColorsOnShader()
    {
        BottleMaskSR.material.SetColor("_C1", BottleColors[0]);
        BottleMaskSR.material.SetColor("_C2", BottleColors[1]);
        BottleMaskSR.material.SetColor("_C3", BottleColors[2]);
        BottleMaskSR.material.SetColor("_C4", BottleColors[3]);
    }

    public void UpdateTopColorValues()
    {
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

            rotationIndex = 3 - (NumberOfColorsInBottle - NumberOfTopColorLayers);

            TopColor = BottleColors[NumberOfColorsInBottle - 1];
        }
    }

    public void OnSelected()
    {
        transform.DOMoveY(originalPosition.y + .5f, .25f);
    }

    public void OnSelectionCanceled()
    {
        transform.DOMove(originalPosition, .25f);
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
    
    private void ChoseRotationPointAndDirection()
    {
        if (transform.position.x > BottleControllerRef.transform.position.x)
        {
            _chosenRotationPoint = LeftRotationPoint;
            directionMultiplier = -1;
        }
        else
        {
            _chosenRotationPoint = RightRotationPoint;
            directionMultiplier = 1;
        }
    }


    private void MoveBottle()
    {
        IsBottleInAction = true;
        Tween moveTween;

        // if chosen position is left go right
        if (_chosenRotationPoint == LeftRotationPoint)
        {
            Vector3 movePos = BottleControllerRef.RightRotationPoint.position;
            movePos.x += BottlePouringDistanceIncreasor;
            moveTween = transform.DOMove(movePos, MoveBottleDuration);
        }
        else // if chose position is right go left
        {
            Vector3 movePos = BottleControllerRef.LeftRotationPoint.position;
            movePos.x -= BottlePouringDistanceIncreasor;
            moveTween = transform.DOMove(movePos, MoveBottleDuration);
        }

        // set line renderer start and end color
        LineRenderer.startColor = TopColor;
        LineRenderer.endColor = TopColor;
        
        // decrease number of colors in first bottle
        NumberOfColorsInBottle -= _numberOfColorsToTransfer;
        
        // increase number of colors in seconds bottle
        BottleControllerRef.NumberOfColorsInBottle += _numberOfColorsToTransfer;

        // lock seconds bottle while action and on completed call rotate bottle
        moveTween.OnStart(BottleControllerRef.UpdateTopColorValues).OnUpdate(() => { BottleControllerRef.BottleIsLocked = true; }).OnComplete(RotateBottle);
    }

    private void PreRotateBottle() // Do pre-rotation on direction
    {
        float angle = 0;
        float lastAngeValue = 0;

        DOTween.To(() => angle, x => angle = x, directionMultiplier * PreRotateAmount, PreRotateDuration).SetEase(Ease.OutQuart).OnUpdate(() =>
        {
            
            //TODO: puring value is 0 for now
            
            // BottleMaskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angle));
            // BottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angle));
            

            transform.RotateAround(_chosenRotationPoint.position, Vector3.forward, lastAngeValue - angle);

            lastAngeValue = angle;
        });
    }

    private void RotateBottle()
    {
        float angle = PreRotateAmount;
        float lastAngeValue = 0;
            
        DOTween.To(() => angle, x => angle = x, directionMultiplier * RotationValues[rotationIndex],
            RotateBottleDuration).OnUpdate(() =>
        {
            if (FillAmounts[NumberOfColorsInBottle + _numberOfColorsToTransfer] >
                FillAmountCurve.Evaluate(angle) + 0.005f)
            {
                if (!LineRenderer.enabled)
                {
                    // set line position
                    LineRenderer.SetPosition(0, _chosenRotationPoint.position);
                    LineRenderer.SetPosition(1, _chosenRotationPoint.position - Vector3.up * 1.35f);

                    // enable line renderer
                    LineRenderer.enabled = true;
                }

                BottleMaskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angle));
                BottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angle));


                BottleControllerRef.FillUp(FillAmountCurve.Evaluate(lastAngeValue) - FillAmountCurve.Evaluate(angle));
            }

            transform.RotateAround(_chosenRotationPoint.position, Vector3.forward, lastAngeValue - angle);
            lastAngeValue = angle;
            
            BottleControllerRef.BottleIsLocked = true;
        }).OnComplete(() =>
        {
            angle = directionMultiplier * RotationValues[rotationIndex];
            BottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angle));
            BottleMaskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angle));

            LineRenderer.enabled = false;

            UpdateColorsOnShader();
            UpdateTopColorValues();

            print("Start RotateBottle Back and position");
            RotateBottleBackAndMoveOriginalPosition();

        });
    }

    private void RotateBottleBackAndMoveOriginalPosition()
    {
        transform.DOMove(originalPosition, MoveBottleDuration).OnComplete(() => { IsBottleInAction = false;});
        transform.DORotate(Vector3.zero, RotateBottleDuration);

        var angle = 0;
        DOTween.To(() =>angle, x => angle = x, 1, RotateBottleDuration).OnUpdate(() =>
        {
            BottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angle));
        });

        

        transform.GetComponent<SpriteRenderer>().sortingOrder -= 2;
        BottleMaskSR.sortingOrder -= 2;
        BottleControllerRef.BottleIsLocked = false;
    }

    private void FillUp(float fillAmountToAdd)
    {
        BottleMaskSR.material.SetFloat("_FillAmount", BottleMaskSR.material.GetFloat("_FillAmount") + fillAmountToAdd);
    }

    private void CalculateRotationIndex(int numberOfEmptySpacesInSecondBottle)
    {
        rotationIndex = 3 - (NumberOfColorsInBottle -
                             Mathf.Min(numberOfEmptySpacesInSecondBottle, NumberOfTopColorLayers));
        
    }
}
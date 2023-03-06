using BottleCodes;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    public BottleController FirstBottle;
    public BottleController SecondBottle;

    private Camera _camera;

    private void OnEnable()
    {
        EventManager.UndoLastMove += CancelSelection;
    }

    private void OnDisable()
    {
        EventManager.UndoLastMove -= CancelSelection;
    }

    private void Start()
    {
        _camera = Camera.main;
    }

    private void CancelSelection()
    {
        if (FirstBottle == null) return;
        FirstBottle.BottleAnimationController.OnSelectionCanceled();
        FirstBottle = null;
    }

    private async void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var mousePos2D = GetMousePos2D();

        var hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider == null) return;

        if (!hit.collider.TryGetComponent(out BottleController bottleController)) return;

        if (FirstBottle == null)
        {
            if (bottleController.IsBottleEmpty()) return;
            
            if (bottleController.BottleAnimationController.BottleIsLocked)
                await bottleController.BottleAnimationSpeedUp.SpeedUpActions(bottleController.BottleData);

            FirstBottle = bottleController;
            FirstBottle.BottleAnimationController.OnSelected();
        }
        else
        {
            var isClickedSameBottleAgain = FirstBottle == bottleController;

            if (isClickedSameBottleAgain)
            {
                FirstBottle.BottleAnimationController.OnSelectionCanceled();
                FirstBottle = null;
            }
            else
            {
                var maxAmountOfBottleCanTake = 4;
                var isBottleFull = bottleController.BottleData.NumberOfColorsInBottle >= maxAmountOfBottleCanTake;

                if (isBottleFull)
                {
                    FirstBottle.BottleAnimationController.OnSelectionCanceled();
                    FirstBottle = null;
                    SecondBottle = null;

                    print("second bottle full!");
                    return;
                }

                var isTopColorsNotMatch = bottleController.BottleData.TopColor != FirstBottle.BottleData.TopColor &&
                                          bottleController.BottleData.NumberOfColorsInBottle > 0;

                if (isTopColorsNotMatch)
                {
                    FirstBottle.BottleAnimationController.OnSelectionCanceled();
                    FirstBottle = null;
                    SecondBottle = null;

                    print("top colors not matching!");
                    return;
                }

                SecondBottle = bottleController;

                FirstBottle.BottleTransferController.BottleControllerRef = SecondBottle;
                SecondBottle.BottleTransferController.BottleControllerRef = FirstBottle;
                SecondBottle.BottleData.ActionBottles.Add(FirstBottle);

                if (SecondBottle.BottleTransferController.FillBottleCheck(FirstBottle.BottleData.TopColor))
                {
                    FirstBottle.StartColorTransfer();
                    FirstBottle = null;
                    SecondBottle = null;
                }
                else
                {
                    FirstBottle = null;
                    SecondBottle = null;
                }
            }
        }
    }

    private Vector2 GetMousePos2D()
    {
        var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        var mousePos2D = new Vector2(mousePos.x, mousePos.y);
        return mousePos2D;
    }
}
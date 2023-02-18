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
        FirstBottle.OnSelectionCanceled();
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

            if (bottleController.BottleIsLocked) await bottleController.SpeedUpActions();

            FirstBottle = bottleController;
            FirstBottle.OnSelected();
        }
        else
        {
            var isClickedSameBottleAgain = FirstBottle == bottleController;
            
            if (isClickedSameBottleAgain)
            {
                FirstBottle.OnSelectionCanceled();
                FirstBottle = null;
            }
            else
            {
                var maxAmountOfBottleCanTake = 4;
                var isBottleFull = bottleController.NumberOfColorsInBottle >= maxAmountOfBottleCanTake;

                if (isBottleFull)
                {
                    FirstBottle.OnSelectionCanceled();
                    FirstBottle = null;
                    SecondBottle = null;

                    print("second bottle full!");
                    return;
                }

                var isTopColorsNotMatch = bottleController.TopColor != FirstBottle.TopColor &&
                                          bottleController.NumberOfColorsInBottle > 0;
                
                if (isTopColorsNotMatch)
                {
                    Debug.Log("second bottle top color: " + bottleController.TopColor.GetHashCode());
                    Debug.Log("first bottle top color: " + FirstBottle.TopColor.GetHashCode());

                    FirstBottle.OnSelectionCanceled();
                    FirstBottle = null;
                    SecondBottle = null;

                    print("top colors not matching!");
                    return;
                }

                SecondBottle = bottleController;

                FirstBottle.BottleControllerRef = SecondBottle;
                SecondBottle.BottleControllerRef = FirstBottle;
                SecondBottle.ActionBottles.Add(FirstBottle);

                if (SecondBottle.FillBottleCheck(FirstBottle.TopColor))
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;

public class MyGameController : MonoBehaviour
{
    public MyBottleController FirstBottle;
    public MyBottleController SecondBottle;

    private async void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider == null) return;

            if (hit.collider.GetComponent<MyBottleController>() == null) return;

            var bottleController = hit.collider.GetComponent<MyBottleController>();
            

            if (FirstBottle == null)
            {
                if (bottleController.IsBottleEmpty()) return;

                if (bottleController.BottleIsLocked)
                {
                    print("icinde");

                    //bottleController.OnSpeedUp = true;

                    await bottleController.SpeedUpActions();
                    
                    print("disinda");
                }

                FirstBottle = bottleController;
                FirstBottle.OnSelected();
            }
            else
            {
                if (FirstBottle == bottleController)
                {
                    FirstBottle.OnSelectionCanceled();
                    FirstBottle = null;
                }
                else
                {
                    if (bottleController.NumberOfColorsInBottle >= 4)
                    {
                        FirstBottle.OnSelectionCanceled();
                        FirstBottle = null;
                        SecondBottle = null;

                        print("second bottle full!");
                        return;
                    }

                    if (bottleController.TopColor != FirstBottle.TopColor &&
                        bottleController.NumberOfColorsInBottle > 0)
                    {
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
    }
}
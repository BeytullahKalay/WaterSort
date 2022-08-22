using UnityEngine;

public class MyGameControllerTest : MonoBehaviour
{
    public MyBottleControllerTest FirstBottle;
    public MyBottleControllerTest SecondBottle;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider == null) return;

            if (hit.collider.GetComponent<MyBottleControllerTest>() == null) return;

            if (hit.collider.GetComponent<MyBottleControllerTest>().BottleIsLocked) return;


            if (FirstBottle == null)
            {
                if (hit.collider.GetComponent<MyBottleControllerTest>().IsBottleEmpty()) return;

                FirstBottle = hit.collider.GetComponent<MyBottleControllerTest>();
                FirstBottle.OnSelected();
            }
            else
            {
                if (FirstBottle == hit.collider.GetComponent<MyBottleControllerTest>())
                {
                    FirstBottle.OnSelectionCanceled();
                    FirstBottle = null;
                }
                else
                {
                    if (hit.collider.GetComponent<MyBottleControllerTest>().NumberOfColorsInBottle >= 4)
                    {
                        FirstBottle.OnSelectionCanceled();
                        FirstBottle = null;
                        SecondBottle = null;

                        print("second bottle full!");
                        return;
                    }

                    if (hit.collider.GetComponent<MyBottleControllerTest>().TopColor != FirstBottle.TopColor &&
                        hit.collider.GetComponent<MyBottleControllerTest>().NumberOfColorsInBottle > 0)
                    {
                        FirstBottle.OnSelectionCanceled();
                        FirstBottle = null;
                        SecondBottle = null;

                        print("top colors not matching!");
                        return;
                    }

                    SecondBottle = hit.collider.GetComponent<MyBottleControllerTest>();

                    FirstBottle.BottleControllerRef = SecondBottle;
                    
                    FirstBottle.UpdateTopColorValues();
                    SecondBottle.UpdateTopColorValues();

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

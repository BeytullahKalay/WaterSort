using UnityEngine;

public class GameController : MonoBehaviour
{
    public BottleController FirstBottle;
    public BottleController SecondBottle;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider == null) return;

            if (hit.collider.GetComponent<BottleController>() == null) return;

            

            if (!hit.collider.GetComponent<BottleController>().bottleIsLocked)
            {
                if (FirstBottle == null)
                {
                    if (hit.collider.GetComponent<BottleController>().IsBottleEmpty()) return;
                    if (hit.collider.GetComponent<BottleController>().bottleUnderWaterPouring) return;


                    FirstBottle = hit.collider.GetComponent<BottleController>();
                    FirstBottle.OnSelected();
                }
                else
                {
                    if (FirstBottle == hit.collider.GetComponent<BottleController>())
                    {
                        FirstBottle.OnSelectionCanceled();
                        FirstBottle = null;
                    }
                    else
                    {
                        if (hit.collider.GetComponent<BottleController>().numberOfColorsInBottle >= 4)
                        {
                            FirstBottle.OnSelectionCanceled();
                            return;
                        }
                        
                        SecondBottle = hit.collider.GetComponent<BottleController>();

                        FirstBottle.bottleControllerRef = SecondBottle;
                        
                        FirstBottle.UpdateTopColorValues();
                        SecondBottle.UpdateTopColorValues();

                        if (SecondBottle.FillBottleCheck(FirstBottle.topColor))
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
}

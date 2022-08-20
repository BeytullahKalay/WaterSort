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

            if (hit.collider != null && hit.collider.GetComponent<BottleController>() != null)
            {
                if (FirstBottle == null)
                {
                    FirstBottle = hit.collider.GetComponent<BottleController>();
                }
                else
                {
                    if (FirstBottle == hit.collider.GetComponent<BottleController>())
                    {
                        FirstBottle = null;
                    }
                    else
                    {
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

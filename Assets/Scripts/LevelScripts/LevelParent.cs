using UnityEngine;

public class LevelParent : MonoBehaviour
{
    public int numberOfColor;

    private void Start()
    {
        GameManager.Instance.TotalColorAmount = numberOfColor;

        var gm = GameManager.Instance;
        gm.bottleControllers.Clear();

        foreach (Transform child in transform)
        {
            foreach (Transform grandChild in child)
            {
                gm.bottleControllers.Add(grandChild.GetComponent<BottleController>());
            }
        }
    }
}

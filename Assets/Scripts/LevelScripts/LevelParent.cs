using BottleCodes;
using UnityEngine;

public class LevelParent : MonoBehaviour
{
    [Space (20)]
    public int NumberOfColor;


    [Header("Lines")]
    [SerializeField]private Transform _line1;
    [SerializeField]private Transform _line2;

    private void Start()
    {
        GameManager.Instance.TotalColorAmount = NumberOfColor;
        var gm = GameManager.Instance;
        gm.bottleControllers.Clear();

        AddControllersToTheControllerList(gm);

        AssignLinesToGameManager(_line1, _line2);
    }

    private void AssignLinesToGameManager(Transform line1, Transform line2)
    {
        var gm = GameManager.Instance;
        gm.line1 = line1;
        gm.line2 = line2;
    }

    private void AddControllersToTheControllerList(GameManager gm)
    {
        foreach (Transform child in transform)
        {
            foreach (Transform grandChild in child)
            {
                if (grandChild.TryGetComponent(out BottleController controller))
                {
                    gm.bottleControllers.Add(controller);
                }
            }
        }
    }

    public void GetLines(Transform line1, Transform line2)
    {
        _line1 = line1;
        _line2 = line2;
    }
}
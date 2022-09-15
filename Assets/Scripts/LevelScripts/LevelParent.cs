using UnityEngine;

public class LevelParent : MonoBehaviour
{
    public int numberOfColor;

    private void Start()
    {
        GameManager.Instance.TotalColorAmount = numberOfColor;
    }
}

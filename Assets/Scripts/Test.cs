using UnityEngine;

public class Test : MonoBehaviour
{
    public void CompleteLevel()
    {
        EventManager.LevelCompleted?.Invoke();
    }
}

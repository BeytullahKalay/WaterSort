using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.CheckIsLevelCompleted += CheckLevelIsCompleted;
    }

    private void OnDisable()
    {
        EventManager.CheckIsLevelCompleted -= CheckLevelIsCompleted;
    }

    #region Singleton

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion


    [Header("Game Objects")]
    [SerializeField] private GameObject confettiParticle;
    
    [Header("Integers")]
    public int TotalColorAmount;

    


    private void CheckLevelIsCompleted()
    {
        TotalColorAmount--;
        if (TotalColorAmount == 0)
        {
            EventManager.LevelCompleted();
        }
    }


    
    
    public GameObject ConfettiParticle => confettiParticle;
}
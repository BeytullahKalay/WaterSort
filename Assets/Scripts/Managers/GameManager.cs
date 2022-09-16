using System.Collections.Generic;
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

    [Header("Tubes")] public List<BottleController> bottleControllers;


    [Header("Particles")]
    [SerializeField] private GameObject confettiParticle;
    
    [Header("Integers")]
    public int TotalColorAmount;

    


    private void CheckLevelIsCompleted()
    {
        int completedColorAmount = 0;

        foreach (var bottle in bottleControllers)
        {
            if (bottle.BottleSorted)
                completedColorAmount++;
        }

        if (completedColorAmount == TotalColorAmount)
        {
            EventManager.LevelCompleted();
        }
    }


    
    
    public GameObject ConfettiParticle => confettiParticle;
}
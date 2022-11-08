using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.CheckIsLevelCompleted += CheckLevelIsCompleted;
        EventManager.LevelCompleted += DisableBottlesCollider;
    }

    private void OnDisable()
    {
        EventManager.CheckIsLevelCompleted -= CheckLevelIsCompleted;
        EventManager.LevelCompleted -= DisableBottlesCollider;
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

    [Header("In Action")] [SerializeField]
    public List<BottleController> InActionBottleList = new List<BottleController>();




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

    private void DisableBottlesCollider()
    {
        foreach (var bottleController in bottleControllers)
        {
            bottleController.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }


    
    
    public GameObject ConfettiParticle => confettiParticle;
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
    
    [Header("Particles")] [SerializeField] private GameObject confettiParticle;

    [Header("Material")] [SerializeField] private Material mat;
    
    [Header("Line Renderer")] [SerializeField] private LineRenderer lineRenderer;
    
    [Header("Integers")] public int TotalColorAmount;

    [Header("In Action")] [SerializeField]
    public List<BottleController> InActionBottleList = new List<BottleController>();


    private ObjectPool<LineRenderer> _pool;

    private void Start()
    {
        _pool = new ObjectPool<LineRenderer>(() =>
        {
            return Instantiate(lineRenderer);
        }, lr =>
        {
            lr.gameObject.SetActive(true);
        }, lr =>
        {
            lr.gameObject.SetActive(false);
        }, lr =>
        {
            Destroy(lr.gameObject);
        }, false, 10);
    }
    
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
            EventManager.LevelCompleted?.Invoke();
        }
    }

    private void DisableBottlesCollider()
    {
        foreach (var bottleController in bottleControllers)
        {
            bottleController.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    
    public LineRenderer GetLineRenderer()
    {
        return _pool.Get();
    }

    public void ReleaseLineRenderer(LineRenderer lr)
    {
        _pool.Release(lr);
    }
    
    public GameObject ConfettiParticle => confettiParticle;

    public Material Mat => mat;
    


}
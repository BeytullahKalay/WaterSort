using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.CheckIsLevelCompleted += CheckLevelIsCompleted;
        EventManager.LevelCompleted += DisableBottlesCollider;
        EventManager.LevelCompleted += ClearInActionBottleList;
        EventManager.RestartLevel += ClearInActionBottleList;
        EventManager.RestartLevel += ResetAllLineRenderers;
        EventManager.LoadNextLevel += ResetAllLineRenderers;
    }

    private void OnDisable()
    {
        EventManager.CheckIsLevelCompleted -= CheckLevelIsCompleted;
        EventManager.LevelCompleted -= DisableBottlesCollider;
        EventManager.LevelCompleted -= ClearInActionBottleList;
        EventManager.RestartLevel -= ClearInActionBottleList;
        EventManager.RestartLevel -= ResetAllLineRenderers;
        EventManager.LoadNextLevel -= ResetAllLineRenderers;
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

    [Header("Lines")] 
    public Transform line1;
    public Transform line2;
    
    

    private ObjectPool<LineRenderer> _pool;
    private List<LineRenderer> _gettedLineRenderers = new List<LineRenderer>();

    private void Start()
    {
        DefineLineRendererPool();
    }

    private void DefineLineRendererPool()
    {
        _pool = new ObjectPool<LineRenderer>(() => { return Instantiate(lineRenderer); },
            lr => { lr.gameObject.SetActive(true); }, lr => { lr.gameObject.SetActive(false); },
            lr => { Destroy(lr.gameObject); }, false, 10);
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

    private void ClearInActionBottleList()
    {
        InActionBottleList.Clear();
    }

    private void DisableBottlesCollider()
    {
        foreach (var bottleController in bottleControllers)
        {
            bottleController.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private void ResetAllLineRenderers()
    {
        if(_gettedLineRenderers.Count <= 0) return;
        
        foreach (var lineRenderer in _gettedLineRenderers)
        {
            lineRenderer.enabled = false;
            _pool.Release(lineRenderer);
        }
        _gettedLineRenderers.Clear();
    }
    
    public LineRenderer GetLineRenderer()
    {
        var lineRenderer = _pool.Get();
        _gettedLineRenderers.Add(lineRenderer);
        return lineRenderer;
    }

    public void ReleaseLineRenderer(LineRenderer lr)
    {
        _pool.Release(lr);
    }
    
    public GameObject ConfettiParticle => confettiParticle;

    public Material Mat => mat;
    


}
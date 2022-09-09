using UnityEngine;

public class LevelMaker : MonoBehaviour
{
    [SerializeField] private int numberOfBottlesCreate = 4;
    
    [SerializeField] private int bottleLineMax = 6;
    [SerializeField] [Range(0, 1)] private float bottleStartPosX = .1f;
    [SerializeField] [Range(0, 1)] private float bottleDistanceX = .01f;

    [SerializeField] [Range(0, 1)] private float bottleStartPosY = .75f;
    [SerializeField] [Range(0, 1)] private float bottleDistanceY = .01f;

    [SerializeField] private GameObject bottle;

    [Space(10)] [SerializeField] private LineRenderer lineRenderer;

    private int _createdBottles;
    private int _maxBottleCount = 12;

    private GameObject _levelParent;
    private GameObject _line1;
    private GameObject _line2;
    private GameObject _obj;


    private void Start()
    {
        CreateLevelParentAndLineObjects();

        CreateBottles(numberOfBottlesCreate);
    }

    private void CreateLevelParentAndLineObjects()
    {
        _levelParent = new GameObject("LevelParent");
        _line1 = new GameObject("Line1");
        _line2 = new GameObject("Line2");
        _line2.transform.parent = _line1.transform.parent = _levelParent.transform;
    }


    private void CreateBottles(int num)
    {
        for (int i = 0; i < Mathf.Min(_maxBottleCount,num); i++)
        {
            _obj = Instantiate(bottle, Vector3.zero, Quaternion.identity);
            _obj.GetComponent<MyBottleController>().LineRenderer = lineRenderer;

            Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(
                bottleStartPosX + _createdBottles % bottleLineMax * bottleDistanceX,
                bottleStartPosY - bottleDistanceY * Mathf.Floor(_createdBottles / bottleLineMax)));
            pos.z = 0;

            _obj.transform.position = pos;

            _obj.transform.SetParent(_createdBottles < bottleLineMax ? _line1.transform : _line2.transform);

            _createdBottles++;

            print("Created");
        }
    }


    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         CreateBottles();
    //     }
    // }
}
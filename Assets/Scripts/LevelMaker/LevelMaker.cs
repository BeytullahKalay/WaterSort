using UnityEngine;

public class LevelMaker : MonoBehaviour
{
    [SerializeField] [Range(4, 12)] private int numberOfBottlesCreate = 4;

    [SerializeField] [Range(0, 1)] private float bottleDistanceX = .01f;
    [SerializeField] [Range(0, 1)] private float bottleStartPosY = .75f;
    [SerializeField] [Range(0, 1)] private float bottleDistanceY = .01f;

    private int bottleLineMax = 6;


    [SerializeField] private GameObject bottle;

    [Space(10)] [SerializeField] private LineRenderer lineRenderer;

    private int _createdBottles;

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


    private void CreateBottles(float num)
    {
        for (int i = 0; i < num; i++)
        {
            _obj = Instantiate(bottle, Vector3.zero, Quaternion.identity);
            _obj.GetComponent<MyBottleController>().LineRenderer = lineRenderer;

            var posA = new Vector3(_createdBottles % (num / 2) * bottleDistanceX,
                bottleStartPosY - bottleDistanceY * Mathf.Floor(_createdBottles / (num / 2)), 0);

            Vector3 pos = Camera.main.ViewportToWorldPoint(posA);
            pos.z = 0;

            _obj.transform.position = pos;

            _obj.transform.SetParent(_createdBottles < (num / 2) ? _line1.transform : _line2.transform);

            _createdBottles++;
        }

        AlignBottles();
    }

    private void AlignBottles()
    {
        var line1Right = _line1.transform.GetChild(_line1.transform.childCount - 1);
        var rightOfScreen = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        var distanceToRight = Mathf.Abs(rightOfScreen - line1Right.transform.position.x);


        var x = Mathf.Abs(distanceToRight / 2);
        var newParentPos = _line1.transform.parent.position;
        newParentPos.x = x;
        _line1.transform.parent.position = newParentPos;
    }
}
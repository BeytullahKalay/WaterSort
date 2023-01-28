using UnityEngine;

namespace LevelScripts
{
    public class LevelBottlesAligner : MonoBehaviour
    {


        public GameObject LevelParent { get;  set; }
        public GameObject Line1 { get;  set; }
        public GameObject Line2 { get;  set; }
        public GameObject LastCreatedParent { get;  set; }

        public void CreateLevelParentAndLineObjects(int numberOfColorInLevel)
        {
            LevelParent = new GameObject("LevelParent");
            Line1 = new GameObject("Line1");
            Line2 = new GameObject("Line2");
            Line2.transform.parent = Line1.transform.parent = LevelParent.transform;

            LevelParent.AddComponent<LevelParent>();
            LevelParent.GetComponent<LevelParent>().NumberOfColor = numberOfColorInLevel;

            LevelParent.GetComponent<LevelParent>().GetLines(Line1.transform, Line2.transform);
            LastCreatedParent = LevelParent;
        }

        public void AlignBottles()
        {
            var line1Right = Line1.transform.GetChild(Line1.transform.childCount - 1);
            var rightOfScreen = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
            var distanceToRight = Mathf.Abs(rightOfScreen - line1Right.transform.position.x);


            var x = Mathf.Abs(distanceToRight / 2);
            var newParentPos = Line1.transform.parent.position;
            newParentPos.x = x;
            Line1.transform.parent.position = newParentPos;
        }
    }
}
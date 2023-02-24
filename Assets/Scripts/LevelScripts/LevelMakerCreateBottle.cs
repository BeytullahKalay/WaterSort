using BottleCodes;
using Solver;
using UnityEngine;

namespace LevelScripts
{
    [RequireComponent(typeof(LevelBottlesAligner))]
    [RequireComponent(typeof(LevelMakerBottlePositioning))]
    public class LevelMakerCreateBottle : MonoBehaviour
    {
        
        [SerializeField] private GameObject bottle;
        
        private GameObject _obj;

        private LevelBottlesAligner _levelBottlesAligner;
        private LevelMakerBottlePositioning _levelMakerBottlePositioning;

        private void Awake()
        {
            _levelBottlesAligner = GetComponent<LevelBottlesAligner>();
            _levelMakerBottlePositioning = GetComponent<LevelMakerBottlePositioning>();
        }

        private void OnEnable()
        {
            EventManager.AddExtraEmptyBottle += AddExtraEmptyBottle;
        }

        private void OnDisable()
        {
            EventManager.AddExtraEmptyBottle -= AddExtraEmptyBottle;
        }


        private void AddExtraEmptyBottle()
        {
            var gm = GameManager.Instance;

            // initialize extra bottle
            Bottle extraBottleHelper = new Bottle(-1);
            var extraBottle = InitializeBottle();
            extraBottle.HelperBottle = extraBottleHelper;
            extraBottle.BottleData.NumberOfColorsInBottle = 0;

            // add new bottle to list
            var bottleControllerList = gm.bottleControllers;
            bottleControllerList.Add(extraBottle);

            // get lines
            _levelBottlesAligner.Line1 = gm.line1.gameObject;
            _levelBottlesAligner.Line2 = gm.line2.gameObject;

            // reset parent position
            _levelBottlesAligner.Line1.transform.parent.position = Vector3.zero;


            for (int i = 0; i < bottleControllerList.Count; i++)
            {
                // New parenting
                bottleControllerList[i].transform.SetParent(null);
                bottleControllerList[i].HelperBottle.ParentNum = FindParent(bottleControllerList.Count, i);

                if (bottleControllerList[i].HelperBottle.ParentNum == 0)
                    bottleControllerList[i].transform.SetParent(_levelBottlesAligner.Line1.transform);
                else if (bottleControllerList[i].HelperBottle.ParentNum == 1)
                    bottleControllerList[i].transform.SetParent(_levelBottlesAligner.Line2.transform);

                // new bottle positioning
                bottleControllerList[i].transform.position = Vector3.zero;
                bottleControllerList[i].HelperBottle.FindPositionAndAssignToPos(bottleControllerList.Count, i,
                    _levelMakerBottlePositioning.BottleDistanceX, _levelMakerBottlePositioning.BottleStartPosY, _levelMakerBottlePositioning.BottleDistanceY);
                bottleControllerList[i].transform.position = bottleControllerList[i].HelperBottle.GetOpenPosition();
            }

            // align bottles
            _levelBottlesAligner.AlignBottles();

            // set origin position of bottles
            for (int i = 0; i < bottleControllerList.Count; i++)
            {
                bottleControllerList[i].SetOriginalPositionTo(bottleControllerList[i].transform.position);
            }
        }
        

        
        public void CreateBottlesAndAssignPositions(AllBottles AllBottlesInLevel,LevelBottlesAligner levelBottlesAligner)
        {
            for (int i = 0; i < AllBottlesInLevel._allBottles.Count; i++)
            {
                var newBottle = InitializeBottle();
                newBottle.HelperBottle = AllBottlesInLevel._allBottles[i];
                newBottle.BottleData.NumberOfColorsInBottle = AllBottlesInLevel._allBottles[i].NumberOfColorsInBottle;
                newBottle.transform.position = AllBottlesInLevel._allBottles[i].GetOpenPosition();
                AllBottlesInLevel._allBottles[i].BottleColors.CopyTo(newBottle.BottleData.BottleColors, 0);
                Parenting(newBottle,levelBottlesAligner);
            }

            levelBottlesAligner.AlignBottles();
        }
        
        private BottleController InitializeBottle()
        {
            _obj = Instantiate(bottle, Vector3.zero, Quaternion.identity);

            var objBottleControllerScript = _obj.GetComponent<BottleController>();

            objBottleControllerScript.BottleData.BottleSorted = false;

            return objBottleControllerScript;
        }
        
        private void Parenting(BottleController newBottle, LevelBottlesAligner levelBottlesAligner)
        {
            if (newBottle.HelperBottle.ParentNum == 0)
            {
                newBottle.transform.SetParent(levelBottlesAligner.Line1.transform);
            }
            else if (newBottle.HelperBottle.ParentNum == 1)
            {
                newBottle.transform.SetParent(levelBottlesAligner.Line2.transform);
            }
        }
        
        private int FindParent(float numberOfBottleToCreate, int createdBottles)
        {
            return (createdBottles < (numberOfBottleToCreate / 2) ? 0 : 1);
        }
    }
}

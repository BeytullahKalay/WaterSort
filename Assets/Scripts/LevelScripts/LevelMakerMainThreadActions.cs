using System.Threading;
using Solver;
using UnityEngine;

namespace LevelScripts
{
    [RequireComponent(typeof(LevelBottlesAligner))]
    [RequireComponent(typeof(LevelMakerBottlePositioning))]
    public class LevelMakerMainThreadActions : MonoBehaviour
    {
        private LevelBottlesAligner _bottlesAligner;
        private LevelMakerBottlePositioning _bottlePositioning;
        private LevelMakerCreateBottle _createBottle;
        private LevelMaker _levelMaker;

        private void Awake()
        {
            _bottlesAligner = GetComponent<LevelBottlesAligner>();
            _bottlePositioning = GetComponent<LevelMakerBottlePositioning>();
            _createBottle = GetComponent<LevelMakerCreateBottle>();
            _levelMaker = GetComponent<LevelMaker>();
        }
        
        private void Update()
        {
            Dispatcher.Instance.InvokePending();
        }
        
        public void MainThread_GetLevelParent()
        {
            Dispatcher.Instance.Invoke(() => { EventManager.GetLevelParent?.Invoke(_bottlesAligner.LastCreatedParent); });
        }
        
        public void MainThread_SaveToJson(AllBottles allBottles)
        {
            Dispatcher.Instance.Invoke(() => JsonManager.SaveToJson(allBottles));
        }
        
        public void MainThread_CreateLevelParentAndLineObjects(int numberOfColorInLevel)
        {
            Thread.Sleep(50);
            Dispatcher.Instance.Invoke(() => _bottlesAligner.CreateLevelParentAndLineObjects(numberOfColorInLevel));
        }
        
        public void MainThread_SetBottlePosition(int numberOfBottleToCreate, Bottle tempBottle, int createdBottles)
        {
            Dispatcher.Instance.Invoke(() => _bottlePositioning.SetBottlePosition(numberOfBottleToCreate, tempBottle, createdBottles));
        }
        
        public void MainThread_CreateBottlesAndAssignPositions(AllBottles allBottles)
        {
            Dispatcher.Instance.Invoke(() =>_createBottle.CreateBottlesAndAssignPositions(allBottles,_bottlesAligner));
        }
        
        public void MainThread_SaveLevelCreateDataToJson()
        {
            Dispatcher.Instance.Invoke(CallSaveLevelDataToJson);
        }

        private void CallSaveLevelDataToJson()
        {
            var levelMakerData = _levelMaker.Data;
            JsonManager.SaveLevelCreateDataToJson(ref levelMakerData);
        }
    }
}

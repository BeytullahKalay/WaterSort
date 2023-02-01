using UnityEngine;

namespace LevelScripts
{
    public class LevelMakerBottlePositioning : MonoBehaviour
    {
        [Range(0, 1)] public float BottleDistanceX = .01f;
        [Range(0, 1)] public float BottleStartPosY = .75f;
        [Range(0, 1)] public float BottleDistanceY = .01f;
        
        public void SetBottlePosition(int numberOfBottleToCreate, Bottle tempBottle, int createdBottles)
        {
            tempBottle.FindPositionAndAssignToPos(numberOfBottleToCreate, createdBottles,BottleDistanceX,
                BottleStartPosY, BottleDistanceY);
        }
    }
}
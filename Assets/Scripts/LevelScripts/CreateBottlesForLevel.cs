using System;
using UnityEngine;

namespace LevelScripts
{
    public class CreateBottlesForLevel : MonoBehaviour
    {
        public void CreateBottles(int numberOfBottleToCreate, bool matchState, bool rainbowBottle,ref int totalWaterCount,
            LevelColorController levelColorController, Data data, int createdBottles, Action<int, Bottle, int> MainThread_BottlePosition)
        {
            for (int i = 0; i < numberOfBottleToCreate; i++)
            {
                Bottle tempBottle = new Bottle(i);
                
                LevelMakerHelper.DecreaseTotalWaterCount(tempBottle,ref totalWaterCount);
                
                levelColorController.GetRandomColorForBottle(tempBottle, matchState, rainbowBottle, data);
                
                MainThread_BottlePosition?.Invoke(numberOfBottleToCreate, tempBottle, createdBottles);

                tempBottle.ParentNum = LevelMakerHelper.FindParent(numberOfBottleToCreate, createdBottles);

                data.CreatedBottles.Add(tempBottle);

                // increase created bottle amount
                createdBottles++;
            }
        }
    }
}
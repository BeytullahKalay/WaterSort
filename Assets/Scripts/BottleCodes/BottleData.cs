using System.Collections.Generic;
using UnityEngine;

namespace BottleCodes
{
    public class BottleData : MonoBehaviour
    {
        public Color[] BottleColors;
        public Color TopColor;
        public int NumberOfTopColorLayers = 1;
        [Range(0, 4)] [SerializeField] public int NumberOfColorsInBottle = 4;
        
        public List<BottleController> ActionBottles = new List<BottleController>();

        public bool BottleSorted;
        
        public Color PreviousTopColor;

        public void DecreaseNumberOfColorsInBottle(int decreaseAmount)
        {
            NumberOfColorsInBottle -= decreaseAmount;
        }
        
        public void IncreaseNumberOfColorsInBottle(int increaseAmount)
        {
            NumberOfColorsInBottle += increaseAmount;
        }
    }
}

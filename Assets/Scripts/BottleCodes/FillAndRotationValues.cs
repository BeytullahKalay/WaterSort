using UnityEngine;

namespace BottleCodes
{
    public class FillAndRotationValues : MonoBehaviour
    {
        [SerializeField] public float[] FillAmounts;
        [SerializeField] public float[] RotationValues;


        public float GetFillCurrentAmount(BottleData bottleData)
        {
            return FillAmounts[bottleData.NumberOfColorsInBottle];
        }

        public float GetRotationValue(BottleData bottleData, int numberOfEmptySpacesInSecondBottle)
        {
            var rotateIndex = 3 - (bottleData.NumberOfColorsInBottle -
                                   Mathf.Min(bottleData.NumberOfTopColorLayers, numberOfEmptySpacesInSecondBottle));
            return RotationValues[rotateIndex];
        }
    }
}

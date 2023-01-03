using UnityEngine;

namespace BottleCodes
{
    public class BottleTransferController : MonoBehaviour
    {
        public int CalculateNumberOfColorToTransfer(int numberOfTopColorLayers,int numberOfColorsInBottle)
        {
            return Mathf.Min(numberOfTopColorLayers, 4 - numberOfColorsInBottle);
        }
    }
}

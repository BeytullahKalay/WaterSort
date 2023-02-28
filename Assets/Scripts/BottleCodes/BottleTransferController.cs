using UnityEngine;

namespace BottleCodes
{
    public class BottleTransferController : MonoBehaviour
    {
        public BottleData BottleData { get; private set; }
        public BottleColorController BottleColorController { get; private set; }    
        
        public int NumberOfColorsToTransfer = 0;

        public BottleController BottleControllerRef;

        
        private void Awake()
        {
            BottleData = GetComponent<BottleData>();
            BottleColorController = GetComponent<BottleColorController>();
        }
        
        
        public bool FillBottleCheck(Color colorToCheck)
        {
            var numberOfColorsInBottle = BottleData.NumberOfColorsInBottle;

            return numberOfColorsInBottle switch
            {
                0 => true,
                4 => false,
                _ => string.Equals(ColorUtility.ToHtmlStringRGB(BottleData.TopColor),
                    ColorUtility.ToHtmlStringRGB(colorToCheck))
            };
        }
    }
}

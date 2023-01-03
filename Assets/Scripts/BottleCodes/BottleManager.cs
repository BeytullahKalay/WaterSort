using UnityEngine;

namespace BottleCodes
{
    [RequireComponent(typeof(BottleActionListController))]
    [RequireComponent(typeof(BottleRotationPointChooser))]
    [RequireComponent(typeof(BottleReferenceHolder))]
    [RequireComponent(typeof(BottleTransferController))]
    [RequireComponent(typeof(BottleColorManager))]
    [RequireComponent(typeof(BottleAnimationController))]
    [RequireComponent(typeof(BottleSortedController))]
    public class BottleManager : MonoBehaviour
    {
        private BottleActionListController _bottleActionListController;
        private BottleReferenceHolder _bottleReferenceHolder;
        private BottleAnimationController _bottleAnimationController;
        private BottleSortedController _bottleSortedController;

        public BottleTransferController BottleTransferController{ get; private set; }
        public BottleRotationPointChooser BottleRotationPointChooser{ get; private set; }
        public BottleColorManager  BottleColorManager { get; private set; }

        public bool BottleIsLocked;
        
        private void Awake()
        {
            _bottleActionListController = GetComponent<BottleActionListController>();
            _bottleReferenceHolder = GetComponent<BottleReferenceHolder>();
            BottleTransferController = GetComponent<BottleTransferController>();
            _bottleAnimationController = GetComponent<BottleAnimationController>();
            _bottleSortedController = GetComponent<BottleSortedController>();
            
            BottleRotationPointChooser = GetComponent<BottleRotationPointChooser>();
            BottleColorManager = GetComponent<BottleColorManager>();
        }


        public void StartColorTransfer()
        {
            //Add Action Bottle To Action Bottle List
            //_bottleActionListController.AddBottleToActionBottleList(this);


            var bottleReferenceColorManager = _bottleReferenceHolder.BottleReference.BottleColorManager;
            for (int i = 0; i <  GetNumberOfColorsToTransfer(); i++)
            {
                bottleReferenceColorManager.BottleColors[bottleReferenceColorManager.BottleColorsAmount + i] = BottleColorManager.TopColor;
            }

            bottleReferenceColorManager.UpdateColorsOnShader();

            // calculating rotation index 
            //CalculateRotationIndex(4 - BottleControllerRef.NumberOfColorsInBottle);

            BottleColorManager.SetRenderingOrderOfRenderers();

            var referenceBottle = _bottleReferenceHolder.BottleReference;
            _bottleAnimationController.MoveBottle(BottleRotationPointChooser.ChoseRotationPoint(referenceBottle), referenceBottle);
        }

        public int GetNumberOfColorsToTransfer()
        {
           return BottleTransferController.CalculateNumberOfColorToTransfer(BottleColorManager.NumberOfTopColorLayers, BottleColorManager.BottleColorsAmount);
        }
    }
}
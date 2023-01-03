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
        private BottleRotationPointChooser _bottleRotationPointChooser;
        private BottleReferenceHolder _bottleReferenceHolder;
        private BottleTransferController _bottleTransferController;
        private BottleColorManager _bottleColorManager;
        private BottleAnimationController _bottleAnimationController;
        private BottleSortedController _bottleSortedController;

        private void Awake()
        {
            _bottleActionListController = GetComponent<BottleActionListController>();
            _bottleRotationPointChooser = GetComponent<BottleRotationPointChooser>();
            _bottleReferenceHolder = GetComponent<BottleReferenceHolder>();
            _bottleTransferController = GetComponent<BottleTransferController>();
            _bottleColorManager = GetComponent<BottleColorManager>();
            _bottleAnimationController = GetComponent<BottleAnimationController>();
            _bottleSortedController = GetComponent<BottleSortedController>();
        }


        public void StartColorTransfer()
        {
            //Add Action Bottle To Action Bottle List
            //_bottleActionListController.AddBottleToActionBottleList(this);

           
            //_bottleRotationPointChooser.ChoseRotationPoint(this);

            
            // _bottleTransferController.CalculateNumberOfColorsToTransfer(_numberOfColors.NumberOfTopColorLayers,
            //     _numberOfColors.ColorsInBottle);

            var bottleReferenceColorManager = _bottleReferenceHolder.BottleReference._bottleColorManager;
            for (int i = 0; i < _bottleTransferController.GetNumberOfColorsToTransfer(_bottleColorManager.NumberOfTopColorLayers,_bottleColorManager.ColorsInBottle); i++)
            {
                bottleReferenceColorManager.BottleColors[bottleReferenceColorManager.ColorsInBottle + i] = _bottleColorManager.TopColor;
            }
            bottleReferenceColorManager.UpdateColorsOnShader();
            
            // calculating rotation index 
            //CalculateRotationIndex(4 - BottleControllerRef.NumberOfColorsInBottle);

            _bottleColorManager.SetRenderingOrderOfRenderers();
        }
    }
}
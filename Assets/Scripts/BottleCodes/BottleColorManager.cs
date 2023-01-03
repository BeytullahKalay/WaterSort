using System;
using System.Collections;
using UnityEngine;

namespace BottleCodes
{
    [RequireComponent(typeof(BottleSortedController))]
    public class BottleColorManager : MonoBehaviour
    {
        [Range(0, 4)] public int BottleColorsAmount = 4;
        public int NumberOfTopColorLayers;

        public SpriteRenderer BottleMaskSR;
        public SpriteRenderer BottleSpriteRenderer;
    
        public Color[] BottleColors;
        public Color TopColor;

        private BottleSortedController _bottleSortedController;

        private void Awake()
        {
            _bottleSortedController = GetComponent<BottleSortedController>();
        }

        private void Start()
        {
            BottleMaskSR.material = GameManager.Instance.Mat;
            UpdateColorsOnShader();
            UpdateTopColorValues();
        }
        
        public void UpdateColorsOnShader()
        {
            BottleMaskSR.material.SetColor("_C1", BottleColors[0]);
            BottleMaskSR.material.SetColor("_C2", BottleColors[1]);
            BottleMaskSR.material.SetColor("_C3", BottleColors[2]);
            BottleMaskSR.material.SetColor("_C4", BottleColors[3]);
        }

        public void UpdateTopColorValues()
        {
            // _previousTopColor = TopColor;
            // BottleSorted = false;
            if (BottleColorsAmount != 0)
            {
                NumberOfTopColorLayers = 1;

                if (BottleColorsAmount == 4)
                {
                    if (BottleColors[3].Equals(BottleColors[2]))
                    {
                        NumberOfTopColorLayers = 2;
                        if (BottleColors[2].Equals(BottleColors[1]))
                        {
                            NumberOfTopColorLayers = 3;
                            if (BottleColors[1].Equals(BottleColors[0]))
                            {
                                NumberOfTopColorLayers = 4;
                                print("Bottle Sorted!");
                                // BottleSorted = true;
                                StartCoroutine(_bottleSortedController.CheckIsSortedCO);
                            }
                        }
                    }
                }

                else if (BottleColorsAmount == 3)
                {
                    if (BottleColors[2].Equals(BottleColors[1]))
                    {
                        NumberOfTopColorLayers = 2;
                        if (BottleColors[1].Equals(BottleColors[0]))
                        {
                            NumberOfTopColorLayers = 3;
                        }
                    }
                }

                else if (BottleColorsAmount == 2)
                {
                    if (BottleColors[1].Equals(BottleColors[0]))
                    {
                        NumberOfTopColorLayers = 2;
                    }
                }
                //TopColor = bottleColors[NumberOfColorsInBottle - 1];
            }
        }

        public void SetRenderingOrderOfRenderers()
        {
            BottleMaskSR.sortingOrder += 2;
            BottleSpriteRenderer.sortingOrder += 2;
        }
    }
}

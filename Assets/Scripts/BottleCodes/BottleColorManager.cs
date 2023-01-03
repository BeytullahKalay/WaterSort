using System.Collections;
using UnityEngine;

namespace BottleCodes
{
    public class BottleColorManager : MonoBehaviour
    {
        [Range(0, 4)] public int ColorsInBottle = 4;
        public int NumberOfTopColorLayers;

        public SpriteRenderer BottleMaskSR;
        public SpriteRenderer BottleSpriteRenderer;
    
        public Color[] BottleColors;
        public Color TopColor;
        
        private void Start()
        {
            BottleMaskSR.material = GameManager.Instance.Mat;
            UpdateColorsOnShader();
            //UpdateTopColorValues();
        }
        
        public void UpdateColorsOnShader()
        {
            BottleMaskSR.material.SetColor("_C1", BottleColors[0]);
            BottleMaskSR.material.SetColor("_C2", BottleColors[1]);
            BottleMaskSR.material.SetColor("_C3", BottleColors[2]);
            BottleMaskSR.material.SetColor("_C4", BottleColors[3]);
        }

        private void UpdateTopColorValues(Color []bottleColors,IEnumerator coroutine)
        {
            // _previousTopColor = TopColor;
            // BottleSorted = false;
            if (ColorsInBottle != 0)
            {
                NumberOfTopColorLayers = 1;

                if (ColorsInBottle == 4)
                {
                    if (bottleColors[3].Equals(bottleColors[2]))
                    {
                        NumberOfTopColorLayers = 2;
                        if (bottleColors[2].Equals(bottleColors[1]))
                        {
                            NumberOfTopColorLayers = 3;
                            if (bottleColors[1].Equals(bottleColors[0]))
                            {
                                NumberOfTopColorLayers = 4;
                                print("Bottle Sorted!");
                                // BottleSorted = true;
                                StartCoroutine(coroutine);
                            }
                        }
                    }
                }

                else if (ColorsInBottle == 3)
                {
                    if (bottleColors[2].Equals(bottleColors[1]))
                    {
                        NumberOfTopColorLayers = 2;
                        if (bottleColors[1].Equals(bottleColors[0]))
                        {
                            NumberOfTopColorLayers = 3;
                        }
                    }
                }

                else if (ColorsInBottle == 2)
                {
                    if (bottleColors[1].Equals(bottleColors[0]))
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

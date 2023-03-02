using UnityEngine;

namespace BottleCodes
{
    public class BottleColorController : MonoBehaviour
    {
        private SpriteRenderer _bottleMaskSR;


        private void Awake()
        {
            _bottleMaskSR = GetComponent<BottleController>().BottleMaskSR;
        }

        public void SetFillAmount(float fillAmount)
        {
            _bottleMaskSR.material.SetFloat("_FillAmount", fillAmount);
        }

        public void SetSARM(float value)
        {
            _bottleMaskSR.material.SetFloat("_SARM", value);
        }

        public void FillUp(float fillAmountToAdd)
        {
            _bottleMaskSR.material.SetFloat("_FillAmount",
                _bottleMaskSR.material.GetFloat("_FillAmount") + fillAmountToAdd);
        }

        public void ClampFillAmount(float min, float max)
        {
            _bottleMaskSR.material.SetFloat("_FillAmount",
                Mathf.Clamp(_bottleMaskSR.material.GetFloat("_FillAmount"), min, max));
        }

        public void UpdateColorsOnShader(BottleData bottleData)
        {
            var bottleColors = bottleData.BottleColors;
            _bottleMaskSR.material.SetColor("_C1", bottleColors[0]);
            _bottleMaskSR.material.SetColor("_C2", bottleColors[1]);
            _bottleMaskSR.material.SetColor("_C3", bottleColors[2]);
            _bottleMaskSR.material.SetColor("_C4", bottleColors[3]);
        }

        public void UpdateTopColorValues(BottleData bottleData)
        {
            var bottleColors = bottleData.BottleColors;
            var searchColorLength = bottleColors.Length - (4 - bottleData.NumberOfColorsInBottle);
            FindNumberOfTopLayers(searchColorLength, bottleData);
            FindTopColor(searchColorLength, bottleData);
            UpdateColorsOnShader(bottleData);
        }

        private void FindNumberOfTopLayers(int searchColorLength, BottleData bottleData)
        {
            int numberOfTopColorLayers;
            var bottleColors = bottleData.BottleColors;

            if (searchColorLength <= 0)
            {
                numberOfTopColorLayers = 0;
                bottleData.NumberOfTopColorLayers = numberOfTopColorLayers;
                return;
            }

            numberOfTopColorLayers = 1;

            for (var i = searchColorLength - 1; i >= 1; i--)
            {
                if (string.Equals(ColorUtility.ToHtmlStringRGB(bottleColors[i]),
                        ColorUtility.ToHtmlStringRGB(bottleColors[i - 1])))
                {
                    numberOfTopColorLayers++;
                }
                else
                {
                    break;
                }
            }

            bottleData.NumberOfTopColorLayers = numberOfTopColorLayers;
        }

        public void CheckIsBottleSorted(BottleData bottleData)
        {
            var isBottleSorted = bottleData.NumberOfTopColorLayers == 4;

            
            if (isBottleSorted)
            {
                bottleData.BottleSorted = true;
                
                //if (bottleData.ActionBottles.Count != 0) return;

                var particleFX = Instantiate(GameManager.Instance.ConfettiParticle,
                    transform.position + new Vector3(0, .25f, -1),
                    GameManager.Instance.ConfettiParticle.transform.rotation);
                Destroy(particleFX, 3);
                EventManager.CheckIsLevelCompleted?.Invoke();
            }
            else
            {
                bottleData.BottleSorted = false;
            }
        }

        private void FindTopColor(int searchColorLength, BottleData bottleData)
        {
            var assignValue = Mathf.Clamp(searchColorLength - 1, 0, int.MaxValue);
            bottleData.TopColor = bottleData.BottleColors[assignValue];
        }
    }
}
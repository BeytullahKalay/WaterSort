using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace BottleCodes.Animation
{
    public class BottleAnimationSpeedUp : MonoBehaviour
    {
        public bool OnSpeedUp;

        [Header("Speed Up Values")] [SerializeField]
        private float speedMultiplier = 10f;


        public void CheckSpeedUp(Tween comingTween)
        {
            if (OnSpeedUp)
                comingTween.timeScale = speedMultiplier;
        }


        public async Task SpeedUpActions(BottleData bottleData)
        {
            var actionBottles = bottleData.ActionBottles;

            var tasks = new Task[actionBottles.Count];

            for (var i = 0; i < actionBottles.Count; i++)
            {
                tasks[i] = actionBottles[i].BottleAnimationSpeedUp.SpeedUp();
            }

            await Task.WhenAll(tasks);
            
            SetOnSpeedUpToFalse();
        }


        private async Task SpeedUp()
        {
            OnSpeedUp = true;

            while (OnSpeedUp)
            {
                await Task.Yield();
            }
        }

        private void SetOnSpeedUpToFalse()
        {
            OnSpeedUp = false;
        }
    }
}
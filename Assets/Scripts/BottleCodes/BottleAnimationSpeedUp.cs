using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace BottleCodes
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


        public async Task SpeedUpActions(BottleData bottleData, List<Tween> tweens)
        {
            var actionBottles = bottleData.ActionBottles;

            var tasks = new Task[actionBottles.Count];

            for (var i = 0; i < actionBottles.Count; i++)
            {
                tasks[i] = actionBottles[i].BottleAnimationSpeedUp
                    .SpeedUp(tweens);
            }

            await Task.WhenAll(tasks);

            SetSpeedToNormalSpeed(tweens);
            
            SetOnSpeedUpToFalse();
        }


        private async Task SpeedUp(List<Tween> tweens)
        {
            OnSpeedUp = true;

            while (OnSpeedUp)
            {
                foreach (var tween in tweens.Where(tween => tween != null))
                {
                    tween.timeScale = speedMultiplier;
                }

                await Task.Yield();
            }
        }

        private void SetSpeedToNormalSpeed(List<Tween> tweens)
        {
            foreach (var tween in tweens.Where(tween => tween != null))
            {
                tween.timeScale = 1f;
            }
        }

        private void SetOnSpeedUpToFalse()
        {
            OnSpeedUp = false;
        }
    }
}
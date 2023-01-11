using DG.Tweening;
using UnityEngine;

namespace Tutorial.Level0
{
    public class HandAnimation : MonoBehaviour
    {
        [SerializeField] private float moveY = 1;
        [SerializeField] private float moveDuration = 1;
        [SerializeField] private Ease moveEase;
        
        // [Header("Positions")] 
        // [SerializeField] private Transform pos1;
        // [SerializeField] private Transform pos2;
    
    
        private Tween _tween;

        // private void Start()
        // {
        //     transform.position = pos1.position;
        // }

        private void OnEnable()
        {
            _tween?.Kill();

            _tween = transform.DOMoveY(moveY, moveDuration).SetEase(moveEase).SetLoops(-1, LoopType.Yoyo);
        }

        // public void PlayAnimationOnPosition2()
        // {
        //     var pos = transform.position;
        //     pos.x = pos2.transform.position.x;
        //     transform.position = pos;
        // }
        //
        // public void PlayAnimationOnPosition1()
        // {
        //     var pos = transform.position;
        //     pos.x = pos1.transform.position.x;
        //     transform.position = pos;
        // }

        public void DestroyHand()
        {
            Destroy(gameObject);
        }
    }
}
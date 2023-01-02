using DG.Tweening;
using UnityEngine;

public class HandAnimation : MonoBehaviour
{
    [SerializeField] private float moveY = 1;
    [SerializeField] private float moveDuration = 1;
    [SerializeField] private Ease moveEase;

    private Tween _tween;

    private void OnEnable()
    {
        _tween?.Kill();

        _tween = transform.DOMoveY(moveY, moveDuration).SetEase(moveEase).SetLoops(-1, LoopType.Yoyo);
    }
}
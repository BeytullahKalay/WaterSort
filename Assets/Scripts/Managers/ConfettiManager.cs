using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConfettiManager : MonoBehaviour
{
    private GameObject _confetti;
    private IEnumerator _coroutine;

    private void OnEnable()
    {
        EventManager.LevelCompleted += StartConfettiEffects;
        EventManager.LoadNextLevel += StopConfettiEffect;
    }

    private void OnDisable()
    {
        EventManager.LevelCompleted -= StartConfettiEffects;
        EventManager.LoadNextLevel -= StopConfettiEffect;
    }

    private void Start()
    {
        _confetti = GameManager.Instance.ConfettiParticle;
        _coroutine = PlayConfetti();
    }

    private void StartConfettiEffects()
    {
        StartCoroutine(_coroutine);
    }

    private void StopConfettiEffect()
    {
        StopCoroutine(_coroutine);
    }

    private IEnumerator PlayConfetti()
    {
        var spawnTime = 0f;
        
        while (true)
        {
            if (Time.time > spawnTime)
            {
                spawnTime = Random.value + Time.time + .35f;
                var randomSpawnPosX = Mathf.Clamp(Random.value, .2f, .7f);
                var randomSpawnPosY = Mathf.Clamp(Random.value, .2f, .7f);
                var spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(randomSpawnPosX, randomSpawnPosY, 5));
                GameObject obj = Instantiate(_confetti,spawnPos ,_confetti.transform.rotation);
                Destroy(obj,3);
            }
            yield return null;  
        }
    }
}

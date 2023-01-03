using System.Collections;
using UnityEngine;

namespace BottleCodes
{
    public class BottleSortedController : MonoBehaviour
    {
        private IEnumerator _coroutine;

        private GameManager _gm;

        private void Awake()
        {
            _gm = GameManager.Instance;
        }

        private void Start()
        {
            _coroutine = CheckIsBottleSorted_Co();
        }

        private IEnumerator CheckIsBottleSorted_Co()
        {
            while (true)
            {
                if (_gm.InActionBottleList.Count == 0)
                {
                    GameObject particleFX = Instantiate(_gm.ConfettiParticle,
                        transform.position + new Vector3(0, .25f, -1),
                        _gm.ConfettiParticle.transform.rotation);
                    Destroy(particleFX, 3);
                    EventManager.CheckIsLevelCompleted?.Invoke();
                    StopCoroutine(_coroutine);
                }

                yield return null;
            }
        }
    }
    
}

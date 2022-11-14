using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSeedTest : MonoBehaviour
{
    public string texts = "deneme metini";

    // private void Awake()
    // {
    //     GenerateRandomSeed();
    // }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.S))
    //     {
    //         SetSeed(texts.GetHashCode());
    //
    //         Debug.Log(Random.Range(-100, 100));
    //     }
    //     else if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         GenerateRandomSeed();
    //     }
    // }

    private void SetSeed(int state = 0)
    {
        Random.InitState(state);
    }

    private void GenerateRandomSeed()
    {
        int tempSeed = (int)System.DateTime.Now.Ticks;
        Random.InitState(tempSeed);

        Debug.Log(Random.Range(-100, 100));
    }
}
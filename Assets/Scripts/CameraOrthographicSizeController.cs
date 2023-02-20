using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraOrthographicSizeController : MonoBehaviour
{
    [SerializeField] private List<LevelAndCameraSize> cameraSizes;


    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        FindAndAssignValues();
    }

    private void OnEnable()
    {
        EventManager.LevelCompleted += FindAndAssignValues;
    }

    private void OnDisable()
    {
        EventManager.LevelCompleted -= FindAndAssignValues;
    }

    private void FindAndAssignValues()
    {
        AssignValues(FindCameraValue(PlayerPrefs.GetInt(PlayerPrefNames.LevelIndex)));
    }

    private float FindCameraValue(int levelIndex)
    {
        foreach (var camSize in cameraSizes.Where(camSize => levelIndex >= camSize.startIndex && levelIndex <= camSize.finishIndex))
        {
            return camSize.cameraSize;
        }

        return cameraSizes[^1].cameraSize;
    }

    private void AssignValues(float camOrthographicSize)
    {
        _camera.orthographicSize = camOrthographicSize;
    }
}

[Serializable]
public struct LevelAndCameraSize
{
    public int startIndex;
    public int finishIndex;
    public float cameraSize;
}

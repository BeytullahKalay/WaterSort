using System;
using UnityEngine;

namespace BottleCodes
{
    public class BottleRotationPointChooser : MonoBehaviour
    {
        public Transform LeftRotationPoint;
        public Transform RightRotationPoint;
    
        private Vector3 _originalPosition;
        private float _directionMultiplier = 1;

        private void Start()
        {
            _originalPosition = transform.position;
        }


        public ChosenPoint ChoseRotationPoint(BottleManager bottleControllerRef)
        {
            if (transform.position.x > bottleControllerRef.transform.position.x)
            {
                _directionMultiplier = -1;
                return ChosenPoint.Left;
            }
            else
            {
                _directionMultiplier = 1;
                return ChosenPoint.Right;
            }
        }
    }
}

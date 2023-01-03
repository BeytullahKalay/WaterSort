using System;
using UnityEngine;

namespace BottleCodes
{
    public class BottleRotationPointChooser : MonoBehaviour
    {
        [SerializeField] private Transform leftRotationPoint;
        [SerializeField] private Transform rightRotationPoint;
    
        private Vector3 _originalPosition;
        private float _directionMultiplier = 1;

        private void Start()
        {
            _originalPosition = transform.position;
        }


        public Transform ChoseRotationPoint(BottleManager bottleControllerRef)
        {
            if (transform.position.x > bottleControllerRef.transform.position.x)
            {
                _directionMultiplier = -1;
                return leftRotationPoint;
            }
            else
            {
                _directionMultiplier = 1;
                return rightRotationPoint;
            }
        }
    }
}

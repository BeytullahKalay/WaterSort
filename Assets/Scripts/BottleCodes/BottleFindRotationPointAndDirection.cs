using UnityEngine;

namespace BottleCodes
{
    public class BottleFindRotationPointAndDirection : MonoBehaviour
    {
        public float BottlePouringDistanceIncreasor = .25f;
        public Transform LeftRotationPoint;
        public Transform RightRotationPoint;
        public Transform ChosenRotationPoint { get; private set; }
        [HideInInspector] public Vector3 MovePosition;


        private Camera _camera;

        public float DirectionMultiplier { get; private set; }


        private void Awake()
        {
            _camera = Camera.main;
            DirectionMultiplier = 1;
        }


        public void ChoseRotationPointAndDirection(BottleController bottleControllerRef)
        {
            var minBottleDistanceToCorner = 1f;

            var leftOfScreen = _camera.ViewportToWorldPoint(Vector3.zero).x;
            var rightOfScreen = _camera.ViewportToWorldPoint(Vector3.one).x;

            var bottleRefPosition = bottleControllerRef.transform.position;
            var distanceToLeft = Mathf.Abs(bottleRefPosition.x - leftOfScreen);
            var distanceToRight = Mathf.Abs(bottleRefPosition.x - rightOfScreen);

            if (transform.position.x > bottleRefPosition.x)
            {
                if (minBottleDistanceToCorner >= distanceToRight)
                {
                    ChosenRotationPoint = RightRotationPoint;
                    DirectionMultiplier = -1;
                }
                else
                {
                    ChosenRotationPoint = LeftRotationPoint;
                    DirectionMultiplier = 1;
                }
            }
            else
            {
                if (minBottleDistanceToCorner >= distanceToLeft)
                {
                    ChosenRotationPoint = LeftRotationPoint;
                    DirectionMultiplier = 1;
                }
                else
                {
                    ChosenRotationPoint = RightRotationPoint;
                    DirectionMultiplier = -1;
                }
            }
        }

        public void ChoseMovePosition(BottleTransferController bottleTransferController)
        {
            var bottleRef = bottleTransferController.BottleControllerRef;

            // if chosen position is left go right
            if (ChosenRotationPoint == LeftRotationPoint)
            {
                MovePosition = bottleRef.BottleFindRotationPointAndDirection.RightRotationPoint.position;
                MovePosition.x += BottlePouringDistanceIncreasor;
            }
            else // if chose position is right go left
            {
                MovePosition = bottleRef.BottleFindRotationPointAndDirection.LeftRotationPoint.position;
                MovePosition.x -= BottlePouringDistanceIncreasor;
            }
        }
    }
}
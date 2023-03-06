using UnityEngine;

namespace BottleCodes
{
    public class BottleLineRendererController : MonoBehaviour
    {
        [SerializeField] private float _lineRendererPouringDistance = 1f;
        private LineRenderer _lineRenderer;
        private GameManager _gm;

        public float LineRendererPouringDistance { get; private set; }

        private void Start()
        {
            LineRendererPouringDistance = _lineRendererPouringDistance;
            _gm = GameManager.Instance;
        }

        public void InitializeLineRenderer(BottleData bottleData)
        {
            _lineRenderer = _gm.GetLineRenderer();
            _lineRenderer.startColor = bottleData.TopColor;
            _lineRenderer.endColor = bottleData.TopColor;
        }

        public void ReleaseLineRenderer()
        {
            _gm.ReleaseLineRenderer(_lineRenderer);
        }

        public void SetLineRenderer(Transform chosenRotationPoint, float lineRendererPouringDistance)
        {
            if (_lineRenderer.enabled) return;

            // set line position
            var position = chosenRotationPoint.position;
            _lineRenderer.SetPosition(0, position);
            _lineRenderer.SetPosition(1, position - Vector3.up * lineRendererPouringDistance);

            // enable line renderer
            _lineRenderer.enabled = true;
        }
    }
}
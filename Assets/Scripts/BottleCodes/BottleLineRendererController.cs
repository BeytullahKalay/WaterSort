using UnityEngine;

namespace BottleCodes
{
    public class BottleLineRendererController : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private GameManager _gm;

        private void Start()
        {
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
        
        public void SetLineRenderer(Transform chosenRotationPoint,float lineRendererPouringDistance)
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

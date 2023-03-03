using UnityEngine;

namespace BottleCodes
{
    public class BottleSpriteRendererOrderController : MonoBehaviour
    {
        public void SetSortingOrder(SpriteRenderer bottleSpriteRenderer, SpriteRenderer bottleMaskSpriteRenderer)
        {
            bottleSpriteRenderer.sortingOrder += 2; // default bottle renderer sorting order
            bottleMaskSpriteRenderer.sortingOrder += 2; // liquid sprite renderer order
        }

        public void ResetSortingOrder(SpriteRenderer bottleSpriteRenderer, SpriteRenderer bottleMaskSpriteRenderer)
        {
            bottleSpriteRenderer.sortingOrder -= 2; // default bottle renderer sorting order
            bottleMaskSpriteRenderer.sortingOrder -= 2; // liquid sprite renderer order
        }
    }
}

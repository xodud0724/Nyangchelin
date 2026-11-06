using UnityEngine;

// 낚싯대 끝에서 물고기까지 이어지는 낚싯줄 (세로로 긴 스프라이트를 늘려서 그림)
[RequireComponent(typeof(SpriteRenderer))]
public class FishingLine : MonoBehaviour
{
    public Transform fishingRodTip;        // 낚싯줄이 시작되는 낚싯대 끝
    public Transform fish;

    private SpriteRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // 물고기가 나와 있는 동안만 줄을 그림
        lineRenderer.enabled = fish.gameObject.activeInHierarchy;

        if (!lineRenderer.enabled)
        {
            return;
        }

        Vector3 start = fishingRodTip.position;
        Vector3 end = fish.position;
        Vector3 diff = end - start;
        float distance = diff.magnitude;

        if (distance < 0.001f)
        {
            return;
        }

        // 스프라이트 피벗이 가운데이므로 두 점의 중간에 놓고, 위쪽이 물고기를 향하게 돌린 뒤 길이만큼 늘림
        transform.position = (start + end) * 0.5f;
        transform.up = diff / distance;

        float spriteHeight = lineRenderer.sprite.bounds.size.y;
        float parentScaleY = transform.parent != null ? transform.parent.lossyScale.y : 1f;
        Vector3 scale = transform.localScale;
        scale.y = distance / (spriteHeight * parentScaleY);
        transform.localScale = scale;
    }
}

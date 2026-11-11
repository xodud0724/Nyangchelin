using UnityEngine;

public class FishingLine : MonoBehaviour
{
    public Transform fishingRodTip;
    public Transform fish;
    public LineRenderer lineRenderer; // 인스펙터에서 LineRenderer 컴포넌트 연결

    void LateUpdate()
    {
        // 물고기가 활성화되어 있을 때만 줄을 표시
        lineRenderer.enabled = fish.gameObject.activeInHierarchy;

        if (!lineRenderer.enabled)
        {
            return;
        }

        // 0번 점(시작점)은 낚싯대 끝, 1번 점(끝점)은 물고기 위치로 설정
        lineRenderer.SetPosition(0, fishingRodTip.position);
        lineRenderer.SetPosition(1, fish.position);
    }
}

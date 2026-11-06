using UnityEngine;

// 노트가 향하는 캐치포인트 5개가 놓인 트랙
public class CatchTrack : MonoBehaviour
{
    public CatchPoint[] catchPoints;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        RestoreAll();
        gameObject.SetActive(false);
    }

    // 이번 노트의 목표 포인트를 랜덤으로 고르고 강조
    public CatchPoint PickRandomPoint()
    {
        RestoreAll();

        CatchPoint point = catchPoints[Random.Range(0, catchPoints.Length)];
        point.ChangePointSprite();
        return point;
    }

    // 모든 포인트를 평소 모습으로 되돌림
    void RestoreAll()
    {
        foreach (CatchPoint point in catchPoints)
        {
            point.RestorePointSprite();
        }
    }
}

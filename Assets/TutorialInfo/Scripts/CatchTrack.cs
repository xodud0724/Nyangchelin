using System.Drawing;
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

    // targetpoint를 random으로 설정
    public CatchPoint PickRandomPoint()
    {
        RestoreAll();

        CatchPoint point = catchPoints[Random.Range(0, catchPoints.Length)];
        point.ChangePointSprite();
        return point;
    }

    public CatchPoint GetTappedPoint(Vector2 tappedPos)
    {
        foreach (CatchPoint point in catchPoints)
        {
            if (point.Contains(tappedPos))
            {
                return point;
            }
        }

        // catchpoint가 아닌 다른 곳을 tap한 경우
        return null;
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

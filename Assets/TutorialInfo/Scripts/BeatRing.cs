using UnityEngine;

public class BeatRing : MonoBehaviour
{
    public RectTransform target;        // 고정 도넛 (scale 1)
    public RectTransform ring;          // 바깥에서 줄어드는 얇은 도넛
    public float startScale = 2.5f;     // 링이 줄어들기 시작하는 크기
    public float endScale = 0.7f;       // 이 크기 아래로 줄어들면 놓친 것으로 보고 다시 시작 (고정 도넛의 안쪽 가장자리)
    public float hitScale = 0.9f;       // 판정 중심 (두 도넛이 겹치는 구간의 한가운데)

    private float shrinkSpeed;
    private float tolerance;
    private float scale;
    private bool isRunning = false;
    private bool isShrinking = false;   // false면 시작 크기에서 대기 중

    public bool IsShrinking => isShrinking;

    void Update()
    {
        if (!isRunning || !isShrinking)
        {
            return;
        }

        scale -= shrinkSpeed * Time.deltaTime;

        // 고정 도넛의 안쪽까지 지나가면 놓친 것으로 보고 다시 시작
        if (scale < endScale)
        {
            Restart();
            return;
        }

        ApplyScale();
    }

    public void Begin(float speed, float tol)
    {
        shrinkSpeed = speed;
        tolerance = tol;
        isRunning = true;
        gameObject.SetActive(true);
        Restart();
    }

    // 시작 크기로 되돌리고 대기 (줄어드는 건 StartShrink를 불러야 시작)
    public void Restart()
    {
        scale = startScale;
        isShrinking = false;
        ApplyScale();
    }

    public void StartShrink()
    {
        isShrinking = true;
    }

    public void Stop()
    {
        isRunning = false;
        isShrinking = false;
        gameObject.SetActive(false);
    }

    // 지금 두 도넛이 겹쳐 있는지
    public bool IsOverlapping()
    {
        return isShrinking && Mathf.Abs(scale - hitScale) <= tolerance;
    }

    void ApplyScale()
    {
        ring.localScale = new Vector3(scale, scale, 1f);
    }
}

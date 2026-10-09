using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public static FishingManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public GameObject gauge;            // 게이지 Image 오브젝트 (성공하면 SetActive(false))
    public RectTransform[] circles;     // 원 7개, 왼쪽에서 오른쪽 순서
    public RectTransform signal;        // 게이지 위에서 움직이는 신호
    public float signalSpeed = 90f;     // 신호 속도 (초당 각도)
    public float hitRadius = 30f;       // 신호와 원 사이 거리가 이 값 이하이면 타이밍 성공

    public int fishLevel = 0;
    public bool isTimingHit = false;
    public bool isFishCaught = false;

    private bool isGaugeRunning = false;
    private float elapsed = 0f;

    // 신호가 움직이는 호 (원의 중심, 반지름, 시작 각도, 각도 변화량)
    private Vector2 arcCenter;
    private float arcRadius;
    private float startAngle;
    private float sweepAngle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartGauge();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGaugeRunning)
        {
            return;
        }

        MoveSignal();

        // 스페이스 바 감지시 타이밍 판정
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckTiming();
        }
    }

    // 게이지 시작 (나중에 낚싯대 동작과 연결할 때 이 함수를 호출)
    public void StartGauge()
    {
        if (!CalculateArc())
        {
            return;
        }

        isTimingHit = false;
        fishLevel = 0;
        elapsed = 0f;
        isGaugeRunning = true;

        if (gauge != null)
        {
            gauge.SetActive(true);
        }
        MoveSignal();
    }

    // 첫 번째, 가운데, 마지막 원을 지나는 원을 계산해서 신호가 움직일 호를 구함
    bool CalculateArc()
    {
        Vector2 a = circles[0].anchoredPosition;
        Vector2 b = circles[circles.Length / 2].anchoredPosition;
        Vector2 c = circles[circles.Length - 1].anchoredPosition;

        float d = 2f * (a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));
        if (Mathf.Abs(d) < 0.0001f)
        {
            Debug.LogWarning("FishingManager: 원들이 일직선이라 호를 계산할 수 없습니다.");
            return false;
        }

        float aSq = a.sqrMagnitude;
        float bSq = b.sqrMagnitude;
        float cSq = c.sqrMagnitude;
        arcCenter = new Vector2(
            (aSq * (b.y - c.y) + bSq * (c.y - a.y) + cSq * (a.y - b.y)) / d,
            (aSq * (c.x - b.x) + bSq * (a.x - c.x) + cSq * (b.x - a.x)) / d);
        arcRadius = (a - arcCenter).magnitude;

        // 시작 각도에서 가운데 원을 거쳐 마지막 원까지 가는 방향으로 각도 변화량 계산
        float angleA = AngleOf(a);
        float angleB = AngleOf(b);
        float angleC = AngleOf(c);
        startAngle = angleA;
        sweepAngle = Mathf.DeltaAngle(angleA, angleB) + Mathf.DeltaAngle(angleB, angleC);
        return true;
    }

    float AngleOf(Vector2 point)
    {
        Vector2 dir = point - arcCenter;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    // 신호를 호를 따라 왼쪽 끝과 오른쪽 끝 사이로 왕복시킴
    void MoveSignal()
    {
        elapsed += Time.deltaTime;
        float length = Mathf.Abs(sweepAngle);
        float t = Mathf.PingPong(elapsed * signalSpeed, length);
        float angle = (startAngle + Mathf.Sign(sweepAngle) * t) * Mathf.Deg2Rad;
        signal.anchoredPosition = arcCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * arcRadius;
    }

    // 신호와 가장 가까운 원을 찾아 타이밍이 맞는지 판단
    void CheckTiming()
    {
        int nearestIdx = 0;
        float nearestDist = float.MaxValue;
        for (int i = 0; i < circles.Length; i++)
        {
            float dist = Vector2.Distance(signal.anchoredPosition, circles[i].anchoredPosition);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestIdx = i;
            }
        }

        if (nearestDist > hitRadius)
        {
            return;
        }

        // 가운데 원이 4, 바깥으로 갈수록 3, 2, 1
        int centerIdx = circles.Length / 2;
        fishLevel = centerIdx + 1 - Mathf.Abs(nearestIdx - centerIdx);
        isTimingHit = true;
        isGaugeRunning = false;

        if (gauge != null)
        {
            gauge.SetActive(false);
            Debug.Log($"{fishLevel} 레벨 물고기가 느껴진다!");
        }
    }
}

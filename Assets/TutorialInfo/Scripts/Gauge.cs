using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
    public Image yellow;                // 타겟 게이지 노랑 (게이지 그림을 부채꼴로 잘라 씀)
    public Image green;                 // 타겟 게이지 초록 (노랑 위에 그려짐)
    public RectTransform signal;        // 호를 따라 움직이는 신호
    public float arcRadius = 88.5f;     // 신호가 움직이는 호의 반지름 (호 중심 = 이 오브젝트의 중심)
    public float minAngle = 12f;        // 신호가 움직이는 각도 범위 (오른쪽 0도, 위쪽 90도)
    public float maxAngle = 168f;
    public float greenWidth = 14f;      // 초록 구간의 각도 폭
    public float yellowWidth = 12f;     // 노랑 구간 한쪽의 각도 폭
    public float signalSpeed = 90f;     // 신호 속도 (초당 각도)

    private bool isRunning = false;
    private float elapsed = 0f;
    private float zoneAngle;            // 초록 구간 중심 각도
    private float signalAngle;          // 신호 현재 각도

    // Update is called once per frame
    void Update()
    {
        if (!isRunning)
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

    // 매턴 타겟 게이지를 새로 만들고 게이지를 시작
    public void Begin()
    {
        // 노랑-초록-노랑 묶음이 호 밖으로 나가지 않는 범위에서 위치를 랜덤으로 정함
        float half = greenWidth / 2f + yellowWidth;
        if (minAngle + half > maxAngle - half)
        {
            Debug.LogWarning("Gauge: 구간 폭이 신호 범위보다 커서 게이지를 시작할 수 없습니다.");
            return;
        }

        zoneAngle = Random.Range(minAngle + half, maxAngle - half);
        ShowZone(yellow, greenWidth + yellowWidth * 2f);
        ShowZone(green, greenWidth);

        elapsed = 0f;
        isRunning = true;
        gameObject.SetActive(true);
        MoveSignal();
    }

    // 게이지 그림을 폭(각도)만큼만 잘라서 구간 중심에 맞춰 회전시키고 켬
    // Fill은 위쪽(90도)에서 시계 방향으로 채우므로 [90 - width, 90] 구간이 보이고, 그 중심을 zoneAngle로 돌림
    void ShowZone(Image zone, float width)
    {
        zone.fillAmount = width / 360f;
        zone.rectTransform.localRotation = Quaternion.Euler(0f, 0f, zoneAngle - 90f + width / 2f);
        zone.gameObject.SetActive(true);
    }

    // 신호를 호를 따라 왼쪽 끝과 오른쪽 끝 사이로 왕복시킴
    void MoveSignal()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.PingPong(elapsed * signalSpeed, maxAngle - minAngle);
        signalAngle = minAngle + t;

        float rad = signalAngle * Mathf.Deg2Rad;
        signal.anchoredPosition = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * arcRadius;
        signal.localRotation = Quaternion.Euler(0f, 0f, signalAngle - 90f);
    }

    // 신호가 있는 구간으로 레벨을 정함 (초록 3, 노랑 2, 흰색 1)
    void CheckTiming()
    {
        float diff = Mathf.Abs(signalAngle - zoneAngle);

        int level;
        if (diff <= greenWidth / 2f)
        {
            level = 3;
        }
        else if (diff <= greenWidth / 2f + yellowWidth)
        {
            level = 2;
        }
        else
        {
            level = 1;
        }

        isRunning = false;
        FishingManager.instance.SetTimingResult(level);
        gameObject.SetActive(false);
    }
}

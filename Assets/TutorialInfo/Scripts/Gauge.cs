using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
    public Image targetGauge1; // 타겟 게이지 1 (밖)
    public Image targetGauge2; // 타겟 게이지 2 (안)
    public RectTransform signal; // 호를 따라 움직이는 신호
    public float signalRadius = 88.5f; // 신호가 움직이는 호의 반지름
    public float minAngle = 12f; // 신호가 움직이는 각도 범위
    public float maxAngle = 168f;
    public float targetWidth1 = 14f; //  타겟 게이지 바깥 쪽 폭
    public float targetWidth2 = 12f; // 타겟 게이지 안 쪽 폭
    public float signalSpeed = 60f; // 신호 속도

    private bool isGaugeRunning = false; // gauge가 작동 중인지
    private float signalTravel = 0f;
    private float targetAngle; // 초록 구간 중심 각도
    private float currentSignalAngle; // 신호 현재 각도

    // Update is called once per frame
    void Update()
    {
        if (!isGaugeRunning)
        {
            return;
        }

        MoveSignal();

        // 스페이스 바 감지시 타이밍 판정
        if (InputManager.instance.TapDown)
        {
            CheckTiming();
        }
    }

    // 매턴 타겟 게이지를 새로 만들고 게이지를 시작
    public void Begin()
    {
        float halfTargetAngle = targetWidth1 + targetWidth2 / 2f; // 타겟 게이지의 절반

        targetAngle = Random.Range(minAngle + halfTargetAngle, maxAngle - halfTargetAngle);
        ShowTarget(targetGauge1, targetWidth1 * 2f + targetWidth2);
        ShowTarget(targetGauge2, targetWidth2);

        signalTravel = 0f;
        currentSignalAngle = minAngle;

        isGaugeRunning = true;
        gameObject.SetActive(true);
        
        MoveSignal();
    }

    // 게이지 그림을 폭(각도)만큼만 잘라서 구간 중심에 맞춰 회전시키고 켬
    // Fill은 위쪽(90도)에서 시계 방향으로 채우므로 [90 - width, 90] 구간이 보이고, 그 중심을 targetAngle로 돌림
    void ShowTarget(Image target, float width)
    {
        target.fillAmount = width / 360f;
        target.rectTransform.localRotation = Quaternion.Euler(0f, 0f, targetAngle - 90f + width / 2f);
        target.gameObject.SetActive(true);
    }

    // 신호를 호를 따라 왼쪽 끝과 오른쪽 끝 사이로 왕복시킴
    void MoveSignal()
    {
        float diff = Mathf.Abs(currentSignalAngle - targetAngle);
        float speedMultiplier = 1f;

        // signal 위치에 따라 속도를 다르게 
        if (diff <= targetWidth2 / 2f)
        {
            speedMultiplier = 2f; // signalSpeed = 120f
        }
        else if (diff <= targetWidth1 + targetWidth2 / 2f)
        {
            speedMultiplier = 1.5f; // signalSpeed = 90f
        }
        
        signalTravel += signalSpeed * speedMultiplier * Time.deltaTime;
        float offset = Mathf.PingPong(signalTravel, maxAngle - minAngle);
        currentSignalAngle = minAngle + offset;

        // gauge 위에 signal 표시
        float rad = currentSignalAngle * Mathf.Deg2Rad;
        signal.anchoredPosition = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * signalRadius;
        signal.localRotation = Quaternion.Euler(0f, 0f, currentSignalAngle - 90f);
    }

    // signal 위치에 따른 물고기 등급
    void CheckTiming()
    {
        float diff = Mathf.Abs(currentSignalAngle - targetAngle);

        int grade;
        if (diff <= targetWidth2 / 2f)
        {
            grade = 3;
        }
        else if (diff <= targetWidth2 / 2f + targetWidth1)
        {
            grade = 2;
        }
        else
        {
            grade = 1;
        }

        isGaugeRunning = false;
        FishingManager.instance.SetTimingResult(grade);
        gameObject.SetActive(false);
    }
}

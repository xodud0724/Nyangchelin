using System.Collections.Generic;
using UnityEngine;

public class Gauge : MonoBehaviour
{
    public SpriteRenderer targetGauge1; // 타겟 게이지 1 (밖), Mask Interaction = Visible Inside Mask
    public SpriteRenderer targetGauge2; // 타겟 게이지 2 (안), Mask Interaction = Visible Inside Mask
    public SpriteMask targetMask1; // targetGauge1을 부채꼴로 잘라 보여 주는 마스크
    public SpriteMask targetMask2; // targetGauge2를 부채꼴로 잘라 보여 주는 마스크
    public Transform signal; // 호를 따라 움직이는 신호
    public float signalRadius = 6.6f; // 신호가 움직이는 호의 반지름 (Gauge 루트 기준 로컬 단위)
    public float minAngle = 12f; // 신호가 움직이는 각도 범위
    public float maxAngle = 168f;
    public float targetWidth1 = 14f; //  타겟 게이지 바깥 쪽 폭
    public float targetWidth2 = 12f; // 타겟 게이지 안 쪽 폭
    public float signalSpeed = 60f; // 신호 속도

    private const int WedgeTextureSize = 512;
    private const float WedgeWorldSize = 15.2f; // 부채꼴 스프라이트가 덮는 로컬 크기 (호 폭 14.92 이상)

    private bool isGaugeRunning = false; // gauge가 작동 중인지
    private float signalTravel = 0f;
    private float targetAngle; // 초록 구간 중심 각도
    private float currentSignalAngle; // 신호 현재 각도
    private readonly Dictionary<float, Sprite> wedgeCache = new Dictionary<float, Sprite>(); // 폭별 부채꼴 마스크 스프라이트

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

    void OnDestroy()
    {
        // 런타임에 만든 부채꼴 텍스처 정리
        foreach (Sprite wedge in wedgeCache.Values)
        {
            if (wedge != null)
            {
                Destroy(wedge.texture);
                Destroy(wedge);
            }
        }
        wedgeCache.Clear();
    }

    // 매턴 타겟 게이지를 새로 만들고 게이지를 시작
    public void Begin()
    {
        float halfTargetAngle = targetWidth1 + targetWidth2 / 2f; // 타겟 게이지의 절반

        targetAngle = Random.Range(minAngle + halfTargetAngle, maxAngle - halfTargetAngle);
        ShowTarget(targetGauge1, targetMask1, targetWidth1 * 2f + targetWidth2);
        ShowTarget(targetGauge2, targetMask2, targetWidth2);

        signalTravel = 0f;
        currentSignalAngle = minAngle;

        isGaugeRunning = true;
        gameObject.SetActive(true);

        MoveSignal();
    }

    // 마스크를 폭(각도)만큼의 부채꼴로 바꾸고 구간 중심(targetAngle)으로 회전시킨 뒤 게이지 그림을 켬
    // 부채꼴은 +x 방향(0도)이 중심이므로 targetAngle만큼 돌리면 중심이 targetAngle이 됨
    void ShowTarget(SpriteRenderer target, SpriteMask mask, float width)
    {
        mask.sprite = GetWedge(width);
        mask.transform.localRotation = Quaternion.Euler(0f, 0f, targetAngle);
        target.gameObject.SetActive(true);
    }

    // +x 방향을 중심으로 폭(도)만큼 펼쳐진 부채꼴 마스크 스프라이트 (피벗은 중앙, 폭별로 한 번만 생성)
    Sprite GetWedge(float widthDeg)
    {
        if (wedgeCache.TryGetValue(widthDeg, out Sprite cached))
        {
            return cached;
        }

        int size = WedgeTextureSize;
        float center = size * 0.5f;
        float half = widthDeg * 0.5f;
        Color32[] pixels = new Color32[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x + 0.5f - center;
                float dy = y + 0.5f - center;
                float angle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;       // -180 ~ 180, +x가 0도
                float radius = Mathf.Max(1f, Mathf.Sqrt(dx * dx + dy * dy));

                // 부채꼴 가장자리까지의 거리(픽셀)로 경계를 1픽셀 정도 부드럽게
                float edgeDistance = (half - Mathf.Abs(angle)) * Mathf.Deg2Rad * radius;
                byte alpha = (byte)(Mathf.Clamp01(edgeDistance + 0.5f) * 255f);

                pixels[y * size + x] = new Color32(255, 255, 255, alpha);
            }
        }

        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels32(pixels);
        texture.Apply();

        Sprite wedge = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size / WedgeWorldSize);
        wedgeCache[widthDeg] = wedge;
        return wedge;
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
        signal.localPosition = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * signalRadius;
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

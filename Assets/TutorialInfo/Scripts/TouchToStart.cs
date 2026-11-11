using UnityEngine;
using UnityEngine.UI;

public class BlinkImage : MonoBehaviour
{
    public float blinkSpeed = 1.2f; // 깜빡이는 속도
    public float minAlpha = 0.1f;   // 가장 흐려졌을 때 투명도 (0이면 완전히 사라짐)

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        // minAlpha(0.1) ~ 1.0 사이를 부드럽게 왕복
        Color c = image.color;
        c.a = Mathf.Lerp(minAlpha, 1f, Mathf.PingPong(Time.time * blinkSpeed, 1f));
        image.color = c;
    }
}

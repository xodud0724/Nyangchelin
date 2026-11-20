using UnityEngine;
using UnityEngine.UI;

public class ResultImage : MonoBehaviour
{   
    public float hidingTime = 1f;
    public float hidingHeight = 80f;

    private Image image;
    private RectTransform rect;
    private Vector2 startPos;
    private float startAlpha;
    private float elapsed = 0f;

    void Start()
    {
        image = GetComponent<Image>();
        rect = image.rectTransform;
        startPos = rect.anchoredPosition;
        startAlpha = image.color.a;
    }

    void Update()
    {
        // 시간 업데이트
        elapsed += Time.deltaTime;
        float t = elapsed / hidingTime;

        // 시간에 따라 위로 올라감
        rect.anchoredPosition = Vector2.Lerp(startPos, startPos + Vector2.up * hidingHeight, t);
        
        // 시간에 따라 투명해짐
        Color c = image.color;
        c.a = Mathf.Lerp(startAlpha, 0f, t);
        image.color = c;

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ResultImage : MonoBehaviour
{
    public Image[] images;

    public Vector2[] startPositions;
    public Coroutine[] hideCoroutines;

    public float hidingtime = 1f;
    public float hidingHeight = 80f;

    void Awake()
    {
        hideCoroutines = new Coroutine[images.Length];
    }

    // SetActive 와 관련된 함수들
    public void SetImageActive(int grade)
    {
        images[grade].gameObject.SetActive(true);
    }

    public void SetImageInactive(int grade)
    {
        images[grade].gameObject.SetActive(false);
    }

    public void SetResultImagesActive()
    {
        foreach (Image image in images)
        {
            image.gameObject.SetActive(true);
        }
    }

    public void SetResultImagesInactive()
    {
        foreach (Image image in images)
        {
            image.gameObject.SetActive(false);
        }
    }

    // show and hide 결과
    public void ShowAndHideResult(int grade)
    {
        ShowImage(grade);
        HideImage(grade);
    }

    // 보이게 하기
    public void ShowImage(int grade)
    {
        // 사라지는 중이라면 hideCoroutine 정지
        if (hideCoroutines[grade] != null)
        {
            StopCoroutine(hideCoroutines[grade]);
            hideCoroutines[grade] = null;
        }

        // 원래 위치로 되돌리고 선명하게 표시
        images[grade].rectTransform.anchoredPosition = startPositions[grade];
        SetAlpha(grade, 1f);
    }

    // 숨기기 (서서히 위로 올라가며 투명해짐)
    public void HideImage(int grade)
    {
        if (hideCoroutines[grade] != null)
        {
            StopCoroutine(hideCoroutines[grade]);
        }

        hideCoroutines[grade] = StartCoroutine(FadeOutAndFloat(grade));
    }

    // 서서히 사라지는 코루틴
    private IEnumerator FadeOutAndFloat(int grade)
    {
        float elapsed = 0f;
        RectTransform rect = images[grade].rectTransform;

        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * hidingHeight;
        float startAlpha = images[grade].color.a;

        while (elapsed < hidingtime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hidingtime;

            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            SetAlpha(grade, Mathf.Lerp(startAlpha, 0f, t));

            yield return null;
        }

        rect.anchoredPosition = endPos;
        SetAlpha(grade, 0f);
        hideCoroutines[grade] = null;
    }

    // 모든 result image 보이기
    public void ShowResultImages()
    {
        for (int i = 0; i < 4; i++)
        {
            SetAlpha(i, 1f);
        }
    }

    // 모든 result image 숨기기
    public void HideResultImages()
    {
        for (int i = 0; i < 4; i++)
        {
            SetAlpha(i, 0f);
        }
    }

    // 투명도 조절 헬퍼 함수
    private void SetAlpha(int index, float alpha)
    {
        Color c = images[index].color;
        c.a = alpha;
        images[index].color = c;
    }
}

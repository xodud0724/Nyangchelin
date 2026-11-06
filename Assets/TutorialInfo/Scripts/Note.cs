using System;
using UnityEngine;
using UnityEngine.UI;

// 물고기에서 목표 CatchPoint로 날아가는 노트 (Canvas 아래 UI Image, Screen Space - Overlay 기준)
public class Note : MonoBehaviour
{
    public const float NoteSpeed = 100f;    // 모든 노트가 같은 속도 (캔버스 단위/초)

    public Image noteImage;
    public Sprite[] noteSprites;            // 0: 왼쪽, 1: 오른쪽, 2: 위, 3: 아래

    private RectTransform target;
    private Vector3 direction;
    private float hitRadius;                // 화면 좌표 기준, 목표 포인트의 반지름
    private Action onMissed;
    private bool isFlying = false;

    public bool IsFlying => isFlying;

    void Update()
    {
        if (!isFlying)
        {
            return;
        }

        transform.position += direction * NoteSpeed * transform.lossyScale.x * Time.deltaTime;

        // 목표 포인트를 완전히 지나쳐 버리면 놓친 것
        if (Vector3.Dot(transform.position - target.position, direction) > hitRadius)
        {
            Stop();
            onMissed?.Invoke();
        }
    }

    // from: 화면 좌표(픽셀), spriteIndex: noteSprites의 인덱스
    public void Launch(Vector2 from, RectTransform targetPoint, int spriteIndex, Action missed)
    {
        target = targetPoint;
        onMissed = missed;
        hitRadius = target.rect.width * 0.5f * target.lossyScale.x;

        noteImage.sprite = noteSprites[spriteIndex];
        transform.position = new Vector3(from.x, from.y, 0f);
        direction = (target.position - transform.position).normalized;

        isFlying = true;
        gameObject.SetActive(true);
    }

    // 지금 목표 포인트와 겹쳐 있는지
    public bool IsOverlapping()
    {
        return isFlying && Vector3.Distance(transform.position, target.position) <= hitRadius;
    }

    public void Stop()
    {
        isFlying = false;
        gameObject.SetActive(false);
    }
}

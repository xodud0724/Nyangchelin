using System;
using System.Collections;
using UnityEngine;

// 줄다리기 연출용 물고기 (승부 규칙은 FishFight가 맡고, 여기서는 움직임만 표현)
public class Fish : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float pullDistance = 3f;     // 당기는 방향으로 벗어나는 최대 거리
    public float moveSpeed = 4f;
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 30f;
    public float jumpHeight = 3f;       // 낚였을 때 튀어 오르는 높이
    public float jumpTime = 0.5f;

    private Vector3 center;             // 물고기가 처음 있던 자리
    private Vector3 basePos;            // 흔들림을 빼고 계산한 위치
    private Vector2 pullDir;
    private float pullRatio = 1f;       // 1이면 최대 거리, 0이면 중앙
    private bool isPulling = false;

    // 물고기가 처음 있던 자리 (당겨져 움직여도 변하지 않음, 루트 오브젝트라 월드 좌표와 같음)
    public Vector3 Center => center;

    Vector3 TargetPos => center + (Vector3)(pullDir * pullDistance * pullRatio);

    // 목표 위치로 아직 이동하는 중인지
    public bool IsMoving => isPulling && basePos != TargetPos;

    void Awake()
    {
        center = transform.localPosition;
        basePos = center;
    }

    void Update()
    {
        if (!isPulling)
        {
            return;
        }

        basePos = Vector3.MoveTowards(basePos, TargetPos, moveSpeed * Time.deltaTime);

        Vector2 side = new Vector2(-pullDir.y, pullDir.x);
        float shake = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
        transform.localPosition = basePos + (Vector3)(side * shake);
    }

    // 싸움 시작 전에 데이터를 적용하고 제자리에 둠 (물속에 있으므로 그림은 숨김)
    public void Ready(FishData data)
    {
        gameObject.SetActive(true);
        transform.localPosition = center;
        basePos = center;
        pullRatio = 1f;
        isPulling = false;

        Sprite sprite = data.Sprite;
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }

        spriteRenderer.enabled = false;
    }

    // 해당 방향으로 움직이려고 애씀
    public void Pull(Vector2 dir)
    {
        pullDir = dir;
        pullRatio = 1f;
        isPulling = true;
    }

    // 성공할 때마다 중앙으로 끌려옴 (ratio: 1이면 최대 거리, 0이면 중앙)
    public void Recover(float ratio)
    {
        pullRatio = ratio;
    }

    // 3번 모두 이겼을 때 모습을 드러내고 튀어 올랐다가 사라짐
    public void Catch(Action onDone)
    {
        isPulling = false;
        spriteRenderer.enabled = true;
        StartCoroutine(CatchRoutine(onDone));
    }

    IEnumerator CatchRoutine(Action onDone)
    {
        Vector3 start = transform.localPosition;
        float t = 0f;

        while (t < jumpTime)
        {
            t += Time.deltaTime;
            transform.localPosition = start + Vector3.up * jumpHeight * (t / jumpTime);
            yield return null;
        }

        gameObject.SetActive(false);
        onDone?.Invoke();
    }
}

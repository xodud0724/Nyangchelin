using System;
using System.Collections;
using UnityEngine;

// 줄다리기 연출용 물고기 (승부 규칙은 FishFight가 맡고, 여기서는 움직임만 표현)
public class Fish : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Transform fishingRodTip;

    private Vector2 spawnDir = Vector2.right; // 오른쪽 (1, 0) 기본 setting
    private float minSpawnDist = 3f;
    private float maxSpawnDist = 8f;

    private float jumpHeight = 2f;
    private float jumpTime = 1f;

    public void Spawn(FishData.FishInfo info)
    {
        spriteRenderer.sprite = info.sprite;

        float randomDist = UnityEngine.Random.Range(minSpawnDist, maxSpawnDist);
        Vector2 spawnOffset = spawnDir.normalized * randomDist;

        transform.position = new Vector2(fishingRodTip.position.x + spawnOffset.x, fishingRodTip.position.y + spawnOffset.y);

        gameObject.SetActive(true);
        // 물 안에 있음
        spriteRenderer.enabled = false;
    }

    public void Catch(Action onDone)
    {
        // 물 밖으로 나옴
        spriteRenderer.enabled = true;
        StartCoroutine(CatchRoutine(onDone));
    }

    private IEnumerator CatchRoutine(Action onDone)
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < jumpTime)
        {
            elapsed += Time.deltaTime;
            transform.position = startPos + Vector3.up * (jumpHeight * (elapsed / jumpTime));
            yield return null;
        }

        gameObject.SetActive(false); // 낚시 완전 종료 시 오브젝트 비활성화 (낚싯줄도 함께 사라짐)
        onDone?.Invoke();
    }
}

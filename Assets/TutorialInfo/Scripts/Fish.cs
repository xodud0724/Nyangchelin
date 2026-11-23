using UnityEngine;
using System.Collections;

// 줄다리기 연출용 물고기 (승부 규칙은 FishFight가 맡고, 여기서는 움직임만 표현)
public class Fish : MonoBehaviour
{
    public TugOfWar tugOfWar;
    public FishData fishData;
    public Transform fishingRodTip;

    public SpriteRenderer spriteRenderer;
    public Sprite[] trashSprite;
    
    
    private float minSpawnAngle = -160f;
    private float maxSpawnAngle = -20f;
    private float minSpawnDist = 3f;
    private float maxSpawnDist = 8f;

    private float jumpHeight = 2f;
    private float jumpTime = 1f;

    public void Spawn(FishData.FishInfo info)
    {
        spriteRenderer.sprite = info.sprite;

        float randomAngle = (Random.Range(0, 2) == 0) ? Random.Range(minSpawnAngle, -140f) : Random.Range(-40f, maxSpawnAngle);
        float rad = randomAngle * Mathf.Deg2Rad;
        Vector2 spawnDir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        float randomDist = Random.Range(minSpawnDist, maxSpawnDist);
        Vector2 spawnOffset = spawnDir * randomDist;

        transform.position = new Vector2(fishingRodTip.position.x + spawnOffset.x, fishingRodTip.position.y + spawnOffset.y);

        gameObject.SetActive(true);
        spriteRenderer.enabled = false; // 물 안에 있음
    }

    public void Catch()
    {
        spriteRenderer.enabled = true; // 물 밖에 나옴
        StartCoroutine(CatchRoutine());
    }

    private IEnumerator CatchRoutine()
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
    }

    public void Escape()
    {
        spriteRenderer.enabled = true; // 물 밖에 나옴
        StartCoroutine(EscapeRoutine());
    }

    private IEnumerator EscapeRoutine()
    {
        // 낚싯대에 걸린 물고기가 쓰레기로 바뀜
        FishingManager.instance.fishInfo = fishData.GetRandomInfo(TapGrade.Miss);
        tugOfWar.fishInfo = fishData.GetRandomInfo(TapGrade.Miss);
        // 랜덤 쓰레기 sprite 로 변경
        int randomIndex = Random.Range(0,3);
        spriteRenderer.sprite = trashSprite[randomIndex];
        
        
        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < jumpTime)
        {
            elapsed += Time.deltaTime;
            transform.position = startPos + Vector3.up * (jumpHeight * (elapsed / jumpTime));
            yield return null;
        }

        gameObject.SetActive(false); // 낚시 완전 종료 시 오브젝트 비활성화 (낚싯줄도 함께 사라짐)
    }
}

using UnityEngine;

public class FisingRod : MonoBehaviour
{
    public Sprite[] fishingRodSprites;
    public SpriteRenderer spriteRenderer;

    private int fishingRodStatusIdx = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isHit = FishingManager.instance.isTimingHit;
        // 스페이스 바 감지시 낚싯대 sprite 변화
        if (Input.GetKeyDown(KeyCode.UpArrow) && isHit)
        {
            ChangeFishingRodSprite();
        }
    }

    void ChangeFishingRodSprite()
    {
        fishingRodStatusIdx++;
        
        if (fishingRodStatusIdx == 3)
        {
            FishingManager.instance.StartGauge();
        }

        fishingRodStatusIdx %= 3;
        spriteRenderer.sprite = fishingRodSprites[fishingRodStatusIdx];

        if (fishingRodStatusIdx == 2)
        {
            int level = FishingManager.instance.fishLevel;
            Debug.Log($"{level} 레벨 물고기를 낚았다!");
        }

        if (fishingRodStatusIdx == 0)
        {
            transform.localPosition = new Vector3(0.5f, 0, 0);
        }
        else
        {
            transform.localPosition = new Vector3(5f, 0, 0);
        }
    }
}

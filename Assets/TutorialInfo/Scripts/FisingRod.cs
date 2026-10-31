using UnityEngine;

public class FisingRod : MonoBehaviour
{
    public Sprite[] fishingRodSprites;
    public SpriteRenderer spriteRenderer;
    public float leanAngle = 15f;       // 물고기가 당길 때 낚싯대가 기우는 각도

    private Quaternion baseRotation;    // 씬에서 설정한 원래 기울기

    void Awake()
    {
        baseRotation = transform.localRotation;
    }

    // 0: 대기, 1: 휘어짐, 2: 낚는 중
    public void SetState(int idx)
    {
        spriteRenderer.sprite = fishingRodSprites[idx];

        if (idx == 0)
        {
            transform.localPosition = new Vector3(0.5f, 0, 0);
        }
        else
        {
            transform.localPosition = new Vector3(5f, 0, 0);
        }
    }

    // 물고기가 당기는 방향으로 낚싯대를 기울임 (Vector2.zero면 원래대로)
    public void Lean(Vector2 dir)
    {
        transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, (dir.y - dir.x) * leanAngle);
    }
}

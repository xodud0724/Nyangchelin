using UnityEngine;

public class BackgroundLoader : MonoBehaviour
{
    [SerializeField]
    private Sprite backgroundSprite;

    private void Awake()
    {
        LoadBackground();
    }

    private void LoadBackground()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("BackgroundLoader: SpriteRenderer not found!");
            return;
        }

        if (backgroundSprite != null)
        {
            spriteRenderer.sprite = backgroundSprite;
            Debug.Log("한강 배경 로드 완료!");
        }
        else
        {
            Debug.LogWarning("BackgroundLoader: backgroundSprite이 할당되지 않았습니다. Inspector에서 설정해주세요.");
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LoadHanGangBackground : MonoBehaviour
{
    private void Start()
    {
        // 한강 배경 이미지를 파일에서 로드
        string imagePath = System.IO.Path.Combine(Application.persistentDataPath, "..\\..\\Assets\\Sprites\\Background\\hangang_bg.png");

        // 또는 절대 경로 사용
        string assetPath = System.IO.Path.Combine(Application.dataPath, "Sprites\\Background\\hangang_bg.png");

        if (System.IO.File.Exists(assetPath))
        {
            byte[] fileData = System.IO.File.ReadAllBytes(assetPath);
            Texture2D texture = new Texture2D(1080, 1920, TextureFormat.RGBA32, false);
            texture.LoadImage(fileData);

            // Sprite 생성
            Sprite sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100);

            // SpriteRenderer에 할당
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.sprite = sprite;

            Debug.Log("한강 배경이 성공적으로 로드되었습니다!");
        }
        else
        {
            Debug.LogError($"한강 배경 이미지를 찾을 수 없습니다: {assetPath}");
        }
    }
}

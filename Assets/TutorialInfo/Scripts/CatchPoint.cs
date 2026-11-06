using UnityEngine;
using UnityEngine.UI;

public class CatchPoint : MonoBehaviour
{
    public Image pointImage;
    public Sprite catchPointSprite;
    public Sprite targetPointSprite;

    // 노트 이동과 판정에 쓰는 위치/크기
    public RectTransform Rect => (RectTransform)transform;

    public void ChangePointSprite()
    {
        pointImage.sprite = targetPointSprite;
    }

    public void RestorePointSprite()
    {
        pointImage.sprite = catchPointSprite;
    }
}

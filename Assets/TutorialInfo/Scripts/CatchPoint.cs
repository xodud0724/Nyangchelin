using UnityEngine;

public class CatchPoint : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite catchPointSprite;
    public Sprite targetPointSprite;

    public void ChangePointSprite()
    {
        spriteRenderer.sprite = targetPointSprite;
        transform.localScale = new Vector3(0.35f, 0.35f, 1f);
    }

    public void RestorePointSprite()
    {
        spriteRenderer.sprite = catchPointSprite;
        transform.localScale = new Vector3(0.25f, 0.25f, 1f);
    }

    // catchpoint가 특정 position을 포함하는지
    public bool Contains(Vector2 pos)
    {
        Bounds pointBound = spriteRenderer.bounds;
        return Mathf.Abs(pos.x - pointBound.center.x) <= pointBound.extents.x && Mathf.Abs(pos.y - pointBound.center.y) <= pointBound.extents.y;
    }

    // catchpoint가 포함하는 note를 반환, 없다면 null 반환
    public Note GetOverlappingNote()
    {
        Note[] notes = FindObjectsByType<Note>(FindObjectsSortMode.None);

        foreach(Note note in notes)
        {
            if (Contains(note.transform.position))
            {
                return note;
            }
        }

        return null;
    }
}

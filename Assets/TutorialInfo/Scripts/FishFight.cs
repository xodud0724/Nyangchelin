using UnityEngine;

public class FishFight : MonoBehaviour
{
    public CatchTrack track;
    public Note note;
    public Fish fish;
    public int roundsToWin = 3;         // 이겨야 하는 라운드 수
    public int turnsPerRound = 2;       // 한 라운드에서 이겨야 하는 턴 수

    // 물고기가 움직이는 방향 (Note.noteSprites의 0:왼쪽 1:오른쪽 2:위 3:아래와 같은 인덱스)
    private static readonly Vector2[] directions = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
    // 그 반대 방향의 노트 스프라이트 인덱스
    private static readonly int[] oppositeSprites = { 1, 0, 3, 2 };

    private bool isRunning = false;
    private bool needLaunch = false;    // 물고기가 다 움직이면 노트를 쏘려고 대기 중
    private int round = 0;
    private int turn = 0;
    private int dirIndex = 0;
    private CatchPoint targetPoint;

    void Update()
    {
        if (!isRunning)
        {
            return;
        }

        // 물고기가 다 움직인 뒤에 노트를 쏨
        if (needLaunch && !fish.IsMoving)
        {
            LaunchNote();
        }

        if (note.IsFlying && InputManager.instance.TapDown)
        {
            Judge();
        }
    }

    // 줄다리기 시작
    public void Begin(FishData fishData)
    {
        round = 0;
        turn = 0;
        fish.Ready(fishData);
        track.Show();
        isRunning = true;
        StartTurn();
    }

    // 새 턴: 방향을 랜덤으로 정하고 물고기를 움직임
    void StartTurn()
    {
        dirIndex = Random.Range(0, directions.Length);
        fish.Pull(directions[dirIndex]);
        needLaunch = true;
    }

    // 물고기 위치에서 랜덤 캐치포인트로 노트를 쏨 (놓쳐서 다시 쏠 때도 사용)
    void LaunchNote()
    {
        needLaunch = false;
        targetPoint = track.PickRandomPoint();
        Vector2 from = Camera.main.WorldToScreenPoint(fish.transform.position);
        note.Launch(from, targetPoint.Rect, oppositeSprites[dirIndex], OnNoteMissed);
    }

    // 노트를 놓치면 페널티 없이 같은 턴을 다시 시도
    void OnNoteMissed()
    {
        needLaunch = true;
    }

    // 노트가 목표 포인트와 겹친 상태에서 그 포인트를 터치했을 때만 성공, 나머지 터치는 무시
    void Judge()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(targetPoint.Rect,InputManager.instance.TapPosition, null))
        {
            return;
        }

        if (!note.IsOverlapping())
        {
            return;
        }

        note.Stop();
        
        turn++;

        if (turn >= turnsPerRound)
        {
            turn = 0;
            round++;
        }

        if (round >= roundsToWin)
        {
            Win();
        }
        else
        {
            StartTurn();
        }
    }

    void Win()
    {
        isRunning = false;
        needLaunch = false;
        track.Hide();
        fish.Catch(() => FishingManager.instance.EndFight());
    }
}

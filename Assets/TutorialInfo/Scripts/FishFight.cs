using UnityEngine;

public class FishFight : MonoBehaviour
{
    public BeatRing beatRing;
    public Fish fish;
    public FisingRod rod;
    public float ringOffset = 150f;     // 물고기가 가려는 반대쪽으로 도넛이 떨어지는 거리
    public int roundsToWin = 3;         // 이겨야 하는 싸움 수
    public int hitsPerRound = 2;        // 한 싸움에서 필요한 성공 횟수

    // 물고기가 움직이려는 방향과, 그 반대 방향의 화살표 (같은 인덱스)
    private static readonly Vector2[] directions = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
    private static readonly KeyCode[] oppositeKeys = { KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.UpArrow };

    private bool isRunning = false;
    private int round = 0;
    private int hits = 0;
    private int dirIndex = 0;
    private FishData data;

    void Update()
    {
        if (!isRunning)
        {
            return;
        }

        // 물고기가 다 움직인 뒤에 링이 줄어들기 시작
        if (!beatRing.IsShrinking && !fish.IsMoving)
        {
            beatRing.StartShrink();
        }

        // 링이 줄어드는 중에만 입력을 받고, 반대 방향 화살표만 인정하고 나머지 키는 무시
        if (beatRing.IsShrinking && Input.GetKeyDown(oppositeKeys[dirIndex]))
        {
            Judge();
        }
    }

    // 줄다리기 시작
    public void Begin(FishData fishData)
    {
        data = fishData;
        round = 0;
        fish.Ready(data);
        gameObject.SetActive(true);
        isRunning = true;
        StartRound();
    }

    // 새 싸움: 방향을 랜덤으로 정하고 물고기와 낚싯대, 도넛을 맞춤
    void StartRound()
    {
        hits = 0;
        dirIndex = Random.Range(0, directions.Length);
        Vector2 dir = directions[dirIndex];

        fish.Pull(dir);
        rod.Lean(dir);

        // 도넛은 물고기가 처음 있던 자리를 기준으로, 당기는 방향의 반대쪽에 놓음
        Vector2 screenPos = Camera.main.WorldToScreenPoint(fish.Center);
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, screenPos, null, out Vector2 fishPos);
        ((RectTransform)beatRing.transform).anchoredPosition = fishPos - dir * ringOffset;
        beatRing.Begin(data.ringSpeed, data.tolerance);
    }

    void Judge()
    {
        if (!beatRing.IsOverlapping())
        {
            beatRing.Restart();     // 놓치면 페널티 없이 다시 시도
            return;
        }

        hits++;
        fish.Recover(1f - (float)hits / hitsPerRound * 0.5f);

        if (hits < hitsPerRound)
        {
            beatRing.Restart();
            return;
        }

        round++;
        if (round >= roundsToWin)
        {
            Win();
        }
        else
        {
            StartRound();
        }
    }

    void Win()
    {
        isRunning = false;
        beatRing.Stop();
        rod.Lean(Vector2.zero);
        rod.SetState(2);
        gameObject.SetActive(false);
        fish.Catch(() => FishingManager.instance.EndFight());
    }
}

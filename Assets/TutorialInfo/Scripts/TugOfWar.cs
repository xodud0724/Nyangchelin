using UnityEngine;

public class TugOfWar : MonoBehaviour
{
    public Fish fish;
    public FishData.FishInfo fishInfo;
    public CatchTrack catchTrack;
    public NoteSpawner noteSpawner;
    public ResultImageSpawner resultImageSpawner;
    
    public int targetNoteCount = 0; // noteSpawner 로부터 받아옴
    public int targetMissCount = 5;
    public int targetRoundCount = 3;

    private int clearedNoteCount = 0; // round 마다 초기화
    private int missCount = 0; // tug of war 마다 초기화
    private int clearedRoundCount = 0; // tug of war 마다 초기화
    

    [SerializeField] private Transform pullTarget; // 물고기가 끌려가는 곳 (낚싯대 모서리)

    // note 박자 세부 판정을 위한 변수들
    [SerializeField] private float perfectRange = 0.35f;
    [SerializeField] private float goodRange = 0.65f;
    
    private float pullStep; // round 마다 얼마나 끌려오는지
    private bool isRunning = false; // 줄다리기 중인지

    void Start()
    {
        noteSpawner.enabled = false;
    }

    void Update()
    {
        if (!isRunning)
        {
            return;
        }

        // 플레이어가 입력했을 때 판정 실행
        if (InputManager.instance.TapDown)
        {
            HandleTap();
        }
    }
    
    // tap 한 위치에 따라서 다르게 동작
    public void HandleTap()
    {
        Vector2 tappedPos = Camera.main.ScreenToWorldPoint(InputManager.instance.TapPosition);

        // tap한 catchpoint 파악
        CatchPoint tappedPoint = catchTrack.GetTappedPoint(tappedPos);

        // 허공 touch 한 경우 => 넘어감
        if (tappedPoint == null)
        {
            return;
        }

        tappedPoint.RestorePointSprite();

        // catchpoint가 포함하는 note get
        Note overlappingNote = tappedPoint.GetOverlappingNote();

        // 겹친 note 가 없는 경우 => Miss
        if (overlappingNote == null)
        {
            missCount++;
            // targetMissCount 만큼 miss 시 낚시 실패
            if (missCount >= targetMissCount)
            {
                FailEnding();
            }

            resultImageSpawner.SpawnResultImage((int)TapGrade.Miss);
            return;
        }

        // 겹친 note 가 있는 경우 거리에 따라 판단
        if (overlappingNote != null)
        {
            // catchpoint 와 note 의 거리에 따라 등급 구분 (Bad, Good, Perfect)
            Vector2 catchPointPos = tappedPoint.transform.position;
            Vector2 notePos = overlappingNote.transform.position;
            TapGrade tapGrade = GetTapGrade(catchPointPos, notePos);

            resultImageSpawner.SpawnResultImage(tapGrade);

            Destroy(overlappingNote.gameObject);

            OnNoteSuccess();
        }
    }
    
    // 줄다리기 시작 (지금은 info 가 필요없지만 추후에 사용될 수 있음)
    public void Begin(FishData.FishInfo info, TapGrade grade)
    {
        catchTrack.Show(); // catch track 활성화     

        fishInfo = info;

        int rhythmIndex = (int)grade - 1;
        BGMManager.instance.PlayRhythm(rhythmIndex);

        // count 변수 초기화
        clearedNoteCount = 0;
        missCount = 0;
        clearedRoundCount = 0;

        float totalDistance = Vector3.Distance(fish.transform.position, pullTarget.position);
        pullStep = totalDistance / targetRoundCount;

        isRunning = true;
        noteSpawner.enabled = true;
    }

    public void StartRound(int noteCount)
    {
        // catch point 의 sprite 모두 원상 복귀
        catchTrack.RestoreAllSprites();

        targetNoteCount = noteCount;
        clearedNoteCount = 0;
    }

    // 노트 판정 성공 시 호출
    public void OnNoteSuccess()
    {
        if (!isRunning)
        {
            return;
        }

        clearedNoteCount++;

        if (clearedNoteCount >= targetNoteCount)
        {
            // 라운드 성공 시 호출
            OnRoundSuccess();
        }
    }

    // round 승리 시 호출
    public void OnRoundSuccess()
    {
        clearedRoundCount++;

        // 내 쪽으로 1/targetRoundCount 만큼 끌려옴
        fish.transform.position = Vector3.MoveTowards(
            fish.transform.position, 
            pullTarget.position, 
            pullStep
        );

        // targetRoundCount 번 성공하면 즉시 승리
        if (clearedRoundCount >= targetRoundCount)
        {
            SuccessEnding();
        }
    }

    public void SuccessEnding()
    {
        catchTrack.Hide();

        isRunning = false;
        noteSpawner.enabled = false;

        fish.Catch();

        Debug.Log($"{fishInfo.fishName} 을(를) 낚았다!");

        FishingManager.instance.AddScore(fishInfo.score);
        FishingManager.instance.ResetFishing(); // 낚시 대기 상태로
        BGMManager.instance.PlayMainBGM(1);
    }

    public void FailEnding()
    {
        catchTrack.Hide();

        isRunning = false;
        noteSpawner.enabled = false;

        fish.Escape();

        Debug.Log("물고기를 놓쳤다..");

        FishingManager.instance.AddScore(fishInfo.score);
        FishingManager.instance.ResetFishing(); // 낚시 대기 상태로
        BGMManager.instance.PlayMainBGM(1);
    }

    // TapGrade 반환
   public TapGrade GetTapGrade(Vector2 pointPos, Vector2 notePos)
    {
        float distance = Vector2.Distance(pointPos, notePos);

        if (distance <= perfectRange)
        {
            return TapGrade.Perfect;
        }
        else if (distance <= goodRange)
        {
            return TapGrade.Good;
        }
        else
        {
            return TapGrade.Bad;
        }
    }
}

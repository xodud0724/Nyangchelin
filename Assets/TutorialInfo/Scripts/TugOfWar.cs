using UnityEngine;

public class TugOfWar : MonoBehaviour
{
    public Fish fish;
    public FishData.FishInfo fishInfo;
    public CatchTrack catchTrack;            // 판정할 CatchPoint 5개를 가진 트랙
    public NoteSpawner noteSpawner;
    public ResultImage resultImage;

    public enum TapGrade
    {
        Miss = 0,
        Bad = 1,
        Good = 2,
        Perfect = 3
    }
    
    [SerializeField] Transform pullTarget;

    [SerializeField] float perfectRange = 0.35f;
    [SerializeField] float goodRange = 0.65f;
    
    private int successCount = 0;
    private float pullStep;
    private bool isRunning = false;

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
    
    // 터치한 CatchPoint에 노트가 겹쳐 있으면 노트를 지우고 성공 처리
    public void HandleTap()
    {
        Vector2 tappedPos = Camera.main.ScreenToWorldPoint(InputManager.instance.TapPosition);

        // tap한 catchpoint 파악
        CatchPoint tappedPoint = catchTrack.GetTappedPoint(tappedPos);

        // 허공 touch 한 경우
        if (tappedPoint == null)
        {
            return;
        }

        // catchpoint가 포함하는 note get
        Note overlappingNote = tappedPoint.GetOverlappingNote();

        // 겹친 note 가 없는 경우 => Miss
        if (overlappingNote == null)
        {
            resultImage.ShowAndHideResult((int)TapGrade.Miss);
            return;
        }

        // 겹친 note 가 있는 경우 거리에 따라 판단
        if (overlappingNote != null)
        {
            // catchpoint 와 note 의 거리에 따라 등급 구분 (Bad, Good, Perfect)
            Vector2 catchPointPos = tappedPoint.transform.position;
            Vector2 notePos = overlappingNote.transform.position;
            TapGrade tapGrade = GetTapGrade(catchPointPos, notePos);

            resultImage.ShowAndHideResult((int)tapGrade);

            Destroy(overlappingNote.gameObject);
            OnNoteSuccess();
        }
    }

    // 줄다리기 시작 (당장은 info 필요없지만 추후에 사용될 수 있음)
    public void Begin(FishData.FishInfo info, int grade)
    {
        catchTrack.Show(); // catch track 활성화
        resultImage.SetResultImagesActive(); // result image 모두 활성화
        resultImage.HideResultImages(); // result image 모두 숨기기     

        fishInfo = info;
        BGMManager.instance.PlayRhythm(grade - 1);

        successCount = 0;

        float totalDistance = Vector3.Distance(fish.transform.position, pullTarget.position);
        pullStep = totalDistance / 10;

        isRunning = true;
        noteSpawner.enabled = true;
    }

    // 노트 판정 성공 시 호출
    public void OnNoteSuccess()
    {
        if (!isRunning) return;

        successCount++;

        // 내 쪽으로 1/5만큼 끌려옴
        fish.transform.position = Vector3.MoveTowards(
            fish.transform.position, 
            pullTarget.position, 
            pullStep
        );

        // 5번 성공하면 즉시 승리
        if (successCount >= 10)
        {
            End();
        }
    }

    public void End()
    {
        catchTrack.Hide();
        resultImage.SetResultImagesInactive(); // result image 모두 비활성화
        resultImage.ShowResultImages(); // result image 모두 보이기

        isRunning = false;
        noteSpawner.enabled = false;

        fish.Catch();

        Debug.Log($"{fishInfo.fishName} 을(를) 낚았다!");

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

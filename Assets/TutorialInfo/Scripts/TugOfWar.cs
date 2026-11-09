using UnityEngine;

public class TugOfWar : MonoBehaviour
{
    public NoteSpawner noteSpawner;
    public Fish fish;
    public CatchTrack catchTrack;            // 판정할 CatchPoint 5개를 가진 트랙

    [SerializeField] Transform pullTarget;
    
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
            Judge();
        }
    }
    
    // 터치한 CatchPoint에 노트가 겹쳐 있으면 노트를 지우고 성공 처리
    void Judge()
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
        if (overlappingNote != null)
        {
            Destroy(overlappingNote.gameObject);
            OnNoteSuccess();
        }
    }

    // 줄다리기 시작
    public void Begin(FishData fishData)
    {
        catchTrack.Show();

        BGMManager.instance.PlayRhythm();
        
        fish.Ready(fishData);

        successCount = 0;

        float totalDistance = Vector3.Distance(fish.transform.position, pullTarget.position);
        pullStep = totalDistance / 18;

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
        if (successCount >= 18)
        {
            Win();
        }
    }

    public void Win()
    {
        catchTrack.Hide();

        isRunning = false;
        noteSpawner.enabled = false;

        fish.Catch(() => FishingManager.instance.EndTugOfWar());
    }
}

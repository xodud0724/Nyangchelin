using UnityEngine;
using System;

public class NoteSpawner : MonoBehaviour
{
    public BGM bgm;
    public TugOfWar tugOfWar;
    public Note notePrefab;
    public CatchTrack catchTrack;

    [SerializeField] private float minSpawnPosY = 4f;
    [SerializeField] private float maxSpawnPosY = 6f;

    [SerializeField] private AudioSource soundEffect;
    [SerializeField] private double soundEffectOffset1 = 0.2;
    [SerializeField] private double soundEffectOffset2 = 0.08;

    // 물고기 grade 에 따라 조정되는 변수 3개
    public int slotCount; // note 생성 구간 (2초)을 나눈 슬롯 수 (4 또는 8)
    public float slotLength; // 시간 slot 의 길이 (0.25초 또는 0.5초)
    public int maxNoteCount; // note count 의 상한선 (4 또는 7)
    public int noteCount; // 현재 round 에서 나와야 하는 note 개수 (1 이상 maxNoteCount 이하)
    
    private float roundLength; // 한 round 의 길이 (4초)
    private double roundStartDspTime;

    private int[] pattern = { 0, 1, 2, 3, 4, 5, 6, 7 };
    private int patternIndex;

    private const int pointCount = 7; // catch point 개수
    private bool[] isPointUsed = new bool[pointCount]; // catch point 사용 여부 (라운드마다 초기화)

    private bool soundEffectScheduled;

    // spawner 가 enable 될 때 자동으로 호출, 2초 후에 note 생성 시작
    void OnEnable()
    {
        roundLength = slotLength * slotCount * 2;

        // note sprite 모두 restore
        catchTrack.RestoreAllSprites();

        // note 생성 시점 갱신
        roundStartDspTime = AudioSettings.dspTime + slotLength * slotCount;
        
        // note 생성 패턴 초기화
        GeneratePattern();
        patternIndex = 0;

        soundEffectScheduled = false;
    }

    // spawner 가 disable 될 때 자동으로 호출, 남아있는 note 모두 destroy
    void OnDisable()
    {
        // 예약된 효과음 취소
        soundEffect.Stop();

        Note[] remainingNotes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        foreach (Note note in remainingNotes)
        {
            Destroy(note.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        double now = AudioSettings.dspTime;
        double spawnTime = roundStartDspTime + pattern[patternIndex] * slotLength;

        // spawn 직전에 효과음 재생 예약
        if (!soundEffectScheduled && now >= spawnTime - soundEffectOffset1)
        {
            soundEffect.PlayScheduled(spawnTime - soundEffectOffset2);
            soundEffectScheduled = true;
        }

        if (now >= spawnTime)
        {
            // 새 round 시작
            if (patternIndex == 0)
            {
                tugOfWar.StartRound(noteCount);
            }

            SpawnNote(spawnTime);
            patternIndex++;
            soundEffectScheduled = false;

            // 이번 round 의 note 가 모두 생성되면 다음 round 준비
            if (patternIndex >= noteCount)
            {
                // note 생성 시점 갱신
                roundStartDspTime += roundLength;

                // note 생성 패턴 초기화
                GeneratePattern();
                patternIndex = 0;
            }
        }
    }

    public void SpawnNote(double spawnTime)
    {
        int index = GetRandomIndex();
        CatchPoint randomPoint = catchTrack.ActivatePoint(index);

        // spawn 위치
        Vector2 spawnPos = new Vector2(0f, 0f);
        spawnPos.x = randomPoint.transform.position.x;
        spawnPos.y = UnityEngine.Random.Range(minSpawnPosY, maxSpawnPosY);

        // target 위치 (target point 의 위치)
        Vector2 targetPos = randomPoint.transform.position;

        float travelTime = slotLength * slotCount;

        // note 객체 생성
        Note note = Instantiate(notePrefab, spawnPos, Quaternion.identity);
        // note 객체 정보 초기화 (지금은 random point 필요 없음)
        note.Init(randomPoint, targetPos, spawnTime, travelTime);
    }

    // note 의 생성 패턴 생성
    private void GeneratePattern()
    {
        // catch point 사용 여부 false 로 초기화
        Array.Clear(isPointUsed, 0, isPointUsed.Length);

        // note 생성 횟수 1 ~ 7 중 랜덤 (물고기 grade에 따라 조정)
        noteCount = UnityEngine.Random.Range(1, maxNoteCount);

        // noteCount 개의 시간 slot 을 랜덤으로 뽑음
        for (int i = 0; i < noteCount; i++)
        {
            int j = UnityEngine.Random.Range(i, slotCount);
            (pattern[i], pattern[j]) = (pattern[j], pattern[i]);
        }

        // 뽑힌 시간 slot 정렬
        Array.Sort(pattern, 0, noteCount);
    }

    // note 의 target point index 를 겹치지 않게 랜덤으로 설정 (0 ~ 6)
    private int GetRandomIndex()
    {
        int index;

        do
        {
            index = UnityEngine.Random.Range(0, pointCount);
        } while (isPointUsed[index]);

        isPointUsed[index] = true;

        return index;
    }
}

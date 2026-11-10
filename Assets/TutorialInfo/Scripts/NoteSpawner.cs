using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public BGM bgm;
    public Note notePrefab;
    public CatchTrack catchTrack;

    [SerializeField] private Transform spawnPoint;

    private float interval;
    private double nextSpawnDspTime;

    // spawner 가 enable 될 때 자동으로 호출, 2초 후에 note 생성 시작
    void OnEnable()
    {
        interval = 60 / bgm.rhythmBGMs[0].bpm;
        nextSpawnDspTime = AudioSettings.dspTime + 4 * interval;
    }

    // spawner 가 disable 될 때 자동으로 호출, 남아있는 note 모두 destroy
    void OnDisable()
    {
        Note[] remainingNotes = FindObjectsByType<Note>(FindObjectsSortMode.None);
        foreach (Note note in remainingNotes)
        {
            Destroy(note.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (AudioSettings.dspTime >= nextSpawnDspTime)
        {
            SpawnNote(nextSpawnDspTime);
            nextSpawnDspTime += 2 * interval; // 한 박자마다 note 생성
        }
    }

    public void SpawnNote(double spawnTime)
    {
        CatchPoint randomPoint = catchTrack.PickRandomPoint();
        Vector2 targetPos = randomPoint.transform.position;

        Note note = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);
        note.Spawn(spawnPoint.position, targetPos, spawnTime, 2 *interval);
    }
}

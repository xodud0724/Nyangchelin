using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public BGM bgm;
    public Note notePrefab;
    public CatchTrack catchTrack;

    [SerializeField] private Transform spawnPoint;

    private float interval;
    private double nextSpawnDspTime;

    public void OnEnable()
    {
        interval = bgm.rhythmBGMs[0].bpm / 120f;
        nextSpawnDspTime = AudioSettings.dspTime + 2 * interval;
    }

    // Update is called once per frame
    void Update()
    {
        if (AudioSettings.dspTime >= nextSpawnDspTime)
        {
            SpawnNote(nextSpawnDspTime);
            nextSpawnDspTime += interval; // 한 박자마다 note 생성
        }
    }

    public void SpawnNote(double spawnTime)
    {
        CatchPoint randomPoint = catchTrack.PickRandomPoint();
        Vector2 targetPos = randomPoint.transform.position;

        Note note = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);
        // note.Spawn(spawnPoint.position, targetPoint.position, spawnTime, interval);
        note.Spawn(spawnPoint.position, targetPos, spawnTime, interval);
    }
}

using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;
    public BGM bgm;

    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private double rhythmStartDspTime;      // 리듬 BGM이 실제로 재생되기 시작하는 dspTime
    private float currentBpm = 120f;

    // 한 박의 길이(초)
    public float BeatInterval => 60f / currentBpm;

    public void PlayRhythm()
    {
        BGM.RhythmBMG rhythm = bgm.rhythmBGMs[0];
        currentBpm = rhythm.bpm;

        // 시작 시각을 미리 정해 두어야 박 시각을 정확히 계산할 수 있음
        rhythmStartDspTime = AudioSettings.dspTime + 0.1;

        audioSource.Stop();
        audioSource.clip = rhythm.clip;
        audioSource.PlayScheduled(rhythmStartDspTime);
    }

    // 지금 이후에 오는 가장 가까운 박의 dspTime (곡의 첫 박이 0초라고 가정)
    public double NextBeatDspTime()
    {
        double elapsed = AudioSettings.dspTime - rhythmStartDspTime;
        double beatIndex = System.Math.Max(0, System.Math.Ceiling(elapsed / BeatInterval));
        return rhythmStartDspTime + beatIndex * BeatInterval;
    }

    public void PlayMainBGM()
    {
        audioSource.Stop();
        audioSource.clip = bgm.normalBGMs[0];
        audioSource.Play();
    }
}

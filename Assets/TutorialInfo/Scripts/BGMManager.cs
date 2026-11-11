using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    public BGM bgm;

    [SerializeField] private AudioSource audioSource;
    
    private double RhythmStartDspTime; // rhythm bgm 정밀한 시작을 위해

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

    public void PlayRhythm(int index)
    {
        BGM.RhythmBGM rhythm = bgm.rhythmBGMs[index];
        RhythmStartDspTime = AudioSettings.dspTime + 0.1;

        audioSource.Stop();
        audioSource.clip = rhythm.clip;
        audioSource.PlayScheduled(RhythmStartDspTime);
    }

    public void PlayMainBGM(int index)
    {
        AudioClip main = bgm.mainBGMs[index];

        audioSource.Stop();
        audioSource.clip = main;
        audioSource.Play();
    }
}

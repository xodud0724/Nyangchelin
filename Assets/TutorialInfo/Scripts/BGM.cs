using UnityEngine;

public class BGM : MonoBehaviour
{
    [System.Serializable]
    public struct RhythmBGM
    {
        public AudioClip clip;
        public float bpm;
    }

    public RhythmBGM[] rhythmBGMs;
    public AudioClip[] mainBGMs;
}

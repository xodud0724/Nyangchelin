using UnityEngine;

public class BGM : MonoBehaviour
{
    [System.Serializable]
    public struct RhythmBMG
    {
        public AudioClip clip;
        public float bpm;
    }

    public RhythmBMG[] rhythmBGMs;
    public AudioClip[] normalBGMs;
}

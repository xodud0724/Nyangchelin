using UnityEngine;

public class FishData : MonoBehaviour
{
    [System.Serializable]
    public class FishInfo
    {
        public int grade;
        public string fishName;
        public int score;
        public string description;
        public Sprite sprite;
    }

    public FishInfo[] fishList;
}

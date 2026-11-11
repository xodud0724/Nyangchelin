using System.Collections.Generic;
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

    public FishInfo GetRandomInfo(int grade)
    {
        List<FishInfo> candidates = new List<FishInfo>();
        foreach (FishInfo fish in fishList)
        {
            if (fish.grade == grade)
            {
                candidates.Add(fish);
            }
        }

        int randomIndex = Random.Range(0, candidates.Count);
        
        return candidates[randomIndex];
    }
}

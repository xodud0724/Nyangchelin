using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public static FishingManager instance;

    public FishData fishData;

    public Gauge gauge;                 // 타이밍 게이지 (판정이 끝나면 스스로 SetActive(false))
    public TugOfWar tugOfWar;             // 줄다리기 (이기면 스스로 SetActive(false))

    public int baitCount = 10;
    public int fishGrade = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartGauge();
    }

    // 게이지 시작
    public void StartGauge()
    {
        fishGrade = 0;
        gauge.Begin();
    }

    // 게이지의 판정 결과를 받고, 그 등급의 물고기와 줄다리기를 시작
    public void SetTimingResult(int fishGrade)
    {
        this.fishGrade = fishGrade;
        Debug.Log($"{fishGrade} 레벨 물고기가 느껴진다!");

        // 등급 1~3이 FishData.list의 인덱스 0~2
        if (fishGrade < 0 || fishGrade >= fishData.fishList.Length)
        {
            Debug.LogWarning($"{fishGrade} 등급 FishData가 list에 없다");
            StartGauge();
            return;
        }
        tugOfWar.Begin(fishData, fishGrade);
    }

    // 줄다리기를 이겨서 물고기를 낚음
    public void EndTugOfWar()
    {
        BGMManager.instance.PlayMainBGM();

        Debug.Log($"{fishGrade} 레벨 물고기를 낚았다!");
        StartGauge();
    }
}

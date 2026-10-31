using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public static FishingManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public Gauge gauge;                 // 타이밍 게이지 (판정이 끝나면 스스로 SetActive(false))
    public FishFight fight;             // 줄다리기 (이기면 스스로 SetActive(false))
    public FisingRod rod;

    public int fishGrade = 0;

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
    public void SetTimingResult(int grade)
    {
        fishGrade = grade;
        Debug.Log($"{fishGrade} 레벨 물고기가 느껴진다!");

        // 등급 1~3이 FishData.list의 인덱스 0~2
        int index = grade - 1;
        if (index < 0 || index >= FishData.list.Length)
        {
            Debug.LogWarning($"{grade} 등급 FishData가 list에 없다");
            StartGauge();
            return;
        }

        rod.SetState(1);
        fight.Begin(FishData.list[index]);
    }

    // 줄다리기를 이겨서 물고기를 낚음
    public void EndFight()
    {
        Debug.Log($"{fishGrade} 레벨 물고기를 낚았다!");
        rod.SetState(0);
        StartGauge();
    }
}

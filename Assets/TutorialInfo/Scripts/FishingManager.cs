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

    public int fishGrade = 0;
    public bool isTimingHit = false;
    public bool isFishCaught = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartGauge();
    }

    void Update()
    {
        if (isFishCaught)
        {
            isFishCaught = false;
            StartGauge();
        }
    }

    // 게이지 시작 (나중에 낚싯대 동작과 연결할 때 이 함수를 호출)
    public void StartGauge()
    {
        isTimingHit = false;
        fishGrade = 0;
        gauge.Begin();
    }

    // 게이지의 판정 결과를 받음
    public void SetTimingResult(int grade)
    {
        fishGrade = grade;
        isTimingHit = true;
        Debug.Log($"{fishGrade} 레벨 물고기가 느껴진다!");
    }
}

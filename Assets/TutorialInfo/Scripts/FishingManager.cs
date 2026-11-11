using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public static FishingManager instance;

    public GameObject throwButton;
    public Gauge gauge;
    public Fish fish;
    public FishData fishData;
    public FishData.FishInfo fishInfo;
    public TugOfWar tugOfWar;


    public TMP_Text baitCountText;
    public TMP_Text scoreText;

    private int baitCount = 5;
    private int score = 0;

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
        baitCount = 5;
        score = 0;

        UpdateUI();

        ResetFishing();
    }

    // 낚시 대기 상태로
    public void ResetFishing()
    {
        if (baitCount <= 0)
        {
            Debug.Log("게임 종료");
        }

        gauge.gameObject.SetActive(false);
        throwButton.gameObject.SetActive(true);
    }

    // 낚시 시작 상태로
    public void StartFishing()
    {
        throwButton.SetActive(false);

        UseBait();

        gauge.gameObject.SetActive(true);
        gauge.Begin();
    }

    // gauge 결과 등급에 맞는 물고기와 줄다리기 시작
    public void SetTimingResult(int grade)
    {
        Debug.Log($"{grade} 등급 물고기가 느껴진다!");

        // grade 등급 물고기 중 랜덤 info 반환
        fishInfo = fishData.GetRandomInfo(grade);
        fish.Spawn(fishInfo);

        tugOfWar.Begin(fishInfo, grade);
    }

    // 줄다리기를 이겨서 물고기를 낚음
    public void EndTugOfWar()
    {
        Debug.Log($"{fishInfo.fishName} 레벨 물고기를 낚았다!");

        AddScore(fishInfo.score);

        ResetFishing(); // 낚시 대기 상태로

        BGMManager.instance.PlayMainBGM(1);
    }

    // 먹이 사용
    public void UseBait()
    {
        baitCount--;
        UpdateUI();
    }

    // 점수 추가
    public void AddScore(int fishScore)
    {
        score += fishScore;
        UpdateUI();
    }

    // UI text 와 실제 변수 값 동기화
    public void UpdateUI()
    {
        baitCountText.text = baitCount.ToString();
        scoreText.text = score.ToString();
    }
}

using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public static FishingManager instance;

    public GameObject throwButton;
    public GameObject homeButton;

    public Player player;
    public Gauge gauge;
    public Fish fish;
    public FishData fishData;
    public FishData.FishInfo fishInfo;
    public TugOfWar tugOfWar;


    public TMP_Text baitCountText;
    public TMP_Text scoreText;

    public int baitCount;
    public int score;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        baitCount = 5;
        score = 0;

        homeButton.gameObject.SetActive(false);

        UpdateUI();

        ResetFishing();
    }

    // 낚시 대기 상태로
    public void ResetFishing()
    {
        if (baitCount <= 0)
        {
            Debug.Log("게임 종료");
            FinishFishing();
            
            return;
        }

        player.gameObject.SetActive(false);
        gauge.gameObject.SetActive(false);
        throwButton.gameObject.SetActive(true);
    }

    // 낚시 시작 상태로 (throwButton click 에서 구현)
    public void StartFishing()
    {
        player.gameObject.SetActive(true);
        throwButton.gameObject.SetActive(false);

        UseBait();

        gauge.gameObject.SetActive(true);
        gauge.Begin();
    }

    public void FinishFishing()
    {
        homeButton.gameObject.SetActive(true);
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

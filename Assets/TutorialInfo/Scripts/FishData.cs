using UnityEngine;

// 물고기 한 종류의 데이터. 종류별 수치는 아래 list에서 코드로 관리하고, FishData.list[0]처럼 인덱스로 접근
public class FishData
{
    public int grade;                   // 게이지 등급 (1~3)
    public string fishName;
    public string spriteName;           // Assets/Resources/Fish/ 안의 이미지 이름
    public float ringSpeed;             // 수축 링이 줄어드는 속도 (초당 scale)
    public float tolerance;             // 두 도넛이 겹친 것으로 인정하는 scale 오차

    // 이미지가 없으면 null이고, 그때는 Fish 오브젝트의 기본 스프라이트를 그대로 씀
    public Sprite Sprite => Resources.Load<Sprite>("Fish/" + spriteName);

    // 등급 1~3이 인덱스 0~2
    public static readonly FishData[] list =
    {
        new FishData { 
            grade = 1,
            fishName = "피라미",
            spriteName = "Fish_Grade1",
            ringSpeed = 0.8f,
            tolerance = 0.2f
        },

        new FishData {
            grade = 2,
            fishName = "붕어",
            spriteName = "Fish_Grade2",
            ringSpeed = 1.1f,
            tolerance = 0.15f
        },
        
        new FishData {
            grade = 3,
            fishName = "잉어",
            spriteName = "Fish_Grade3",
            ringSpeed = 1.4f,
            tolerance = 0.1f
        },
    };
}

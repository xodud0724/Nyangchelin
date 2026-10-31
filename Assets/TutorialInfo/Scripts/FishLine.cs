using UnityEngine;

// 낚싯대 끝과 물고기를 잇는 낚싯줄
[RequireComponent(typeof(LineRenderer))]
public class FishLine : MonoBehaviour
{
    public Transform rodTip;
    public Transform fish;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
    }

    void LateUpdate()
    {
        // 물고기가 보이는 동안만 줄을 그림
        line.enabled = fish.gameObject.activeInHierarchy;

        if (line.enabled)
        {
            line.SetPosition(0, rodTip.position);
            line.SetPosition(1, fish.position);
        }
    }
}

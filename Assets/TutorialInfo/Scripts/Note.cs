using System;
using UnityEngine;


public class Note : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] noteSprites;
    
    private Vector3 spawnPos;
    private Vector3 direction;
    private float distance;            
    private double spawnDspTime;
    private float interval;

    void Update()
    {
        // 경과 시간으로 위치를 정해서 프레임이 튀어도 도착 시각이 박에서 밀리지 않음
        float t = (float)((AudioSettings.dspTime - spawnDspTime) / interval);
        transform.position = spawnPos + direction * distance * t;

        if (transform.position.y <= -5)
        {
            Destroy(gameObject);
        }
    }

    public void Spawn(Vector3 spawnPos, Vector3 targetPos, double spawnDspTime, float interval)
    {
        this.spawnPos = spawnPos;
        this.spawnDspTime = spawnDspTime;
        this.interval = interval;

        spriteRenderer.sprite = noteSprites[0];

        Vector3 toTarget = targetPos - spawnPos;
        distance = toTarget.magnitude;
        direction = toTarget.normalized;
    }
}

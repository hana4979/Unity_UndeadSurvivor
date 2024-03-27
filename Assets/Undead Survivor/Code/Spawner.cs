using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // .. 자식 오브젝트 Point 중 하나를 딱 집음
    // 자식 오브젝트의 트랜스폼을 담을 배열 변수
    public Transform[] spawnPoint;
    // 만든 클래스를 그대로 타입으로 활용하여 배열 변수
    public SpawnData[] spawnData;
    public float levelTime; // 소호나 레벨 구간을 결정하는 변수

    int level;
    float timer; // 소환 타이머

    void Awake()
    {
        // 배열(여러 개) 초기화, 자기 자신 포함
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / spawnData.Length; // 최대 시간에 몬스터 데이터 크기(개수)로 나눔
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // deltaTime : 한 프레임이 소비한 시간
        timer += Time.deltaTime;
        // FloorToInt : 소수점 아래는 버리고 Int형으로 바꾸는 함수
        // CeilToInt : 소수점 아래 올리고 Int형으로 바꾸는 함수
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

        // 타이머가 일정값에 도달하면 소환
        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        // Get : GameObject 반환
        GameObject enemy = GameManager.instance.pool.Get(0); // Enemy 프리팹만 가져옴
        // 자식 오브젝트에서만 선택되도록 랜덤 시작은 1부터
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        // 오브젝트 풀에서 가져온 오브젝트를 Enemy 컴포넌트로 접근
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

// 직접 작성한 클래스를 직렬화를 통하여 인스펙터에서 초기화 가능
[System.Serializable]
public class SpawnData
{
    public float spawnTime; // 소환 시간
    public int spriteType; // 스프라이트 타입
    public int health; // 체력
    public float speed; // 속도
}

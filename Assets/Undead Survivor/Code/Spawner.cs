using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // .. �ڽ� ������Ʈ Point �� �ϳ��� �� ����
    // �ڽ� ������Ʈ�� Ʈ�������� ���� �迭 ����
    public Transform[] spawnPoint;
    // ���� Ŭ������ �״�� Ÿ������ Ȱ���Ͽ� �迭 ����
    public SpawnData[] spawnData;
    public float levelTime; // ��ȣ�� ���� ������ �����ϴ� ����

    int level;
    float timer; // ��ȯ Ÿ�̸�

    void Awake()
    {
        // �迭(���� ��) �ʱ�ȭ, �ڱ� �ڽ� ����
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / spawnData.Length; // �ִ� �ð��� ���� ������ ũ��(����)�� ����
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // deltaTime : �� �������� �Һ��� �ð�
        timer += Time.deltaTime;
        // FloorToInt : �Ҽ��� �Ʒ��� ������ Int������ �ٲٴ� �Լ�
        // CeilToInt : �Ҽ��� �Ʒ� �ø��� Int������ �ٲٴ� �Լ�
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);

        // Ÿ�̸Ӱ� �������� �����ϸ� ��ȯ
        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        // Get : GameObject ��ȯ
        GameObject enemy = GameManager.instance.pool.Get(0); // Enemy �����ո� ������
        // �ڽ� ������Ʈ������ ���õǵ��� ���� ������ 1����
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        // ������Ʈ Ǯ���� ������ ������Ʈ�� Enemy ������Ʈ�� ����
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
    }
}

// ���� �ۼ��� Ŭ������ ����ȭ�� ���Ͽ� �ν����Ϳ��� �ʱ�ȭ ����
[System.Serializable]
public class SpawnData
{
    public float spawnTime; // ��ȯ �ð�
    public int spriteType; // ��������Ʈ Ÿ��
    public int health; // ü��
    public float speed; // �ӵ�
}

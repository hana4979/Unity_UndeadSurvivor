using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public bool isLive; // �ð� ���� ����
    public float gameTime; // ������ �帣�� ���� �ð�
    public float maxGameTime = 2 * 10f; // �ִ� �ð�
    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int level; // ����
    public int kill; // ų��
    public int exp; // ����ġ
    public int[] nextExp = { 10, 30, 60, 100, 150, 210, 280, 360, 450, 600 }; // �� ������ �ʿ� ����ġ�� ������ �迭 ����
    [Header("# Game Object")]
    public PoolManager pool; // �پ��� ���� ���� ������ �� �ֵ���
    public Player player;
    public LevelUp uiLevelUp; // ������ �˾� ��Ʈ��
    public Result uiResult; // ���Ӱ�� �˾� ��Ʈ��
    public GameObject enemyCleaner;

    void Awake()
    {
        instance = this; // Awake �����ֱ⿡�� �ڱ� �ڽ��� ������ �ʱ�ȭ
    }

    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;

        player.gameObject.SetActive(true); // player ������Ʈ Ȱ��ȭ
        uiLevelUp.Select(playerId % 2); // ���� ����ֱ�
        Resume(); // ���� ����� �Ŀ��� ���������� �۵�
    }

    // ���� ���� ����
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine()); // Dead �ִϸ��̼� ���� �ð��� ���� ���� �ڷ�ƾ
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop(); // �ð� ����
    }

    // ���� �¸� ����
    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine()); // ��� Enemy�� ��� �ִϸ��̼� ���
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop(); // �ð� ����
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0); // �� �����
    }

    void Update()
    {
        if (!isLive)
            return;

        // deltaTime : �� �������� �Һ��� �ð�
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory(); // maxGameTime �ʰ� �� ���� �¸�
        }
    }

    public void GetExp()
    {
        // enemyCleaner ��ȯ���� exp ��� ���� ����
        if (!isLive)
            return;

        exp++;

        if (exp == nextExp[Mathf.Min(level, nextExp.Length-1)]) // ������ ���� �ʿ� ����ġ
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        // timeScale : ����Ƽ�� �ð� �ӵ�(����)
        Time.timeScale = 0; // 0 : �ð��� ����
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
}
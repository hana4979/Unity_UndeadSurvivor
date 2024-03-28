using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public bool isLive; // 시간 정지 여부
    public float gameTime; // 실제로 흐르는 게임 시간
    public float maxGameTime = 2 * 10f; // 최대 시간
    [Header("# Player Info")]
    public int playerId;
    public float health;
    public float maxHealth = 100;
    public int level; // 레벨
    public int kill; // 킬수
    public int exp; // 경험치
    public int[] nextExp = { 10, 30, 60, 100, 150, 210, 280, 360, 450, 600 }; // 각 레벨의 필요 경험치를 보관할 배열 변수
    [Header("# Game Object")]
    public PoolManager pool; // 다양한 곳에 쉽게 접근할 수 있도록
    public Player player;
    public LevelUp uiLevelUp; // 레벨업 팝업 컨트롤
    public Result uiResult; // 게임결과 팝업 컨트롤
    public Transform uiJoy; // 조이스틱 오브젝트
    public GameObject enemyCleaner;

    void Awake()
    {
        instance = this; // Awake 생명주기에서 자기 자신을 변수로 초기화
    }

    public void GameStart(int id)
    {
        playerId = id;
        health = maxHealth;

        player.gameObject.SetActive(true); // player 오브젝트 활성화
        uiLevelUp.Select(playerId % 2); // 무기 쥐어주기
        Resume(); // 게임 재시작 후에도 정상적으로 작동

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    // 게임 오버 로직
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine()); // Dead 애니메이션 실행 시간을 벌기 위한 코루틴
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop(); // 시간 멈춤

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    // 게임 승리 로직
    public void GameVictory()
    {
        StartCoroutine(GameVictoryRoutine()); // 모든 Enemy의 사망 애니메이션 대기
    }

    IEnumerator GameVictoryRoutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop(); // 시간 멈춤

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0); // 씬 재시작
    }

    public void GameQuit()
    {
        Application.Quit(); // 어플리케이션 종료
    }


    void Update()
    {
        if (!isLive)
            return;

        // deltaTime : 한 프레임이 소비한 시간
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
            GameVictory(); // maxGameTime 초과 시 게임 승리
        }
    }

    public void GetExp()
    {
        // enemyCleaner 소환으로 exp 얻는 오류 방지
        if (!isLive)
            return;

        exp++;

        if (exp == nextExp[Mathf.Min(level, nextExp.Length-1)]) // 레벨에 따른 필요 경험치
        {
            level++;
            exp = 0;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        // timeScale : 유니티의 시간 속도(배율)
        Time.timeScale = 0; // 0 : 시간이 멈춤
        uiJoy.localScale = Vector3.zero; // uiJoy의 스케일 0
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
        uiJoy.localScale = Vector3.one; // uiJoy의 스케일 1
    }
}

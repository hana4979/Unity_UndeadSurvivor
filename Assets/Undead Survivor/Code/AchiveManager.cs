using System; // Enum.GetValues 사용
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;
    public GameObject uiNotice;

    enum Achive { unlockPotato, UnlockBean }
    Achive[] achives;
    WaitForSecondsRealtime wait;

    void Awake()
    {
        // Enum.GetValues() : 주어진 열거형의 데이터를 모두 가져오는 함수
        // (Achive[]) : 타입을 명시적으로 지정하여 타입 맞추기
        achives = (Achive[])Enum.GetValues(typeof(Achive));
        wait = new WaitForSecondsRealtime(5);

        if (!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }

    void Init()
    {
        // PlayerPrefs : 유니티가 지원하는 데이터 저장 함수
        // SetInt 함수를 사용하여 key와 연결된 int형 데이터 저장
        PlayerPrefs.SetInt("MyData", 1); // MyData 속 1 저장

        // 업적 데이터와 동일한 이름의 key로 0을 저장
        foreach (Achive achive in achives)
        {
            PlayerPrefs.SetInt(achive.ToString(), 0); // 0 : 달성X
            /*
            PlayerPrefs.SetInt("unlockPotato", 0); // 0 : 달성X
            PlayerPrefs.SetInt("UnlockBean", 0);
            */
        }
    }

    void Start()
    {
        UnlockCharacter();
    }

    void UnlockCharacter()
    {
        for (int index = 0; index < lockCharacter.Length; index++)
        {
            string achiveName = achives[index].ToString(); // 업적 이름 받기
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1; // 업적 달성 여부 판단
            lockCharacter[index].SetActive(!isUnlock);
            unlockCharacter[index].SetActive(isUnlock);
        }
    }

    // 후속 점검을 위해 LateUpdate에서 진행
    void LateUpdate()
    {
        // 업적들 확인
        foreach (Achive achive in achives) {
            CheckAchive(achive);
        }
    }

    void CheckAchive(Achive achive)
    {
        bool isAchive = false;

        switch (achive)
        {
            case Achive.unlockPotato:
                isAchive = GameManager.instance.kill >= 10; // 킬 수가 10 이상이면 true
                break;
            case Achive.UnlockBean:
                isAchive = GameManager.instance.gameTime == GameManager.instance.maxGameTime; // 클리어 조건
                break;
        }

        if(isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0)
        {
            PlayerPrefs.SetInt(achive.ToString(), 1); // 해당 업적 달성

            // 알림 창의 자식 오브젝트를 순회
            for(int index=0; index < uiNotice.transform.childCount; index++)
            {
                bool isActive = index == (int)achive; // 순번이 맞으면 활성화
                uiNotice.transform.GetChild(index).gameObject.SetActive(isActive);
            }
            StartCoroutine(NoticeRoutine());

        }
    }

    IEnumerator NoticeRoutine()
    {
        uiNotice.SetActive(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);

        yield return wait;

        uiNotice.SetActive(false);
    }
}

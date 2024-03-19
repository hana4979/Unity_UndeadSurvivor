using System; // Enum.GetValues 사용
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;

    enum Achive { unlockPotato, UnlockBean }
    Achive[] achives;

    void Awake()
    {
        // Enum.GetValues() : 주어진 열거형의 데이터를 모두 가져오는 함수
        // (Achive[]) : 타입을 명시적으로 지정하여 타입 맞추기
        achives = (Achive[])Enum.GetValues(typeof(Achive));

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
            PlayerPrefs.SetInt(achive.ToString(), 1); // 0 : 달성X
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
        for(int index = 0; index < lockCharacter.Length; index++)
        {
            string achiveName = achives[index].ToString(); // 업적 이름 받기
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1; // 업적 달성 여부 판단
            lockCharacter[index].SetActive(!isUnlock);
            unlockCharacter[index].SetActive(isUnlock);
        }
    }

    void Update()
    {
        
    }
}

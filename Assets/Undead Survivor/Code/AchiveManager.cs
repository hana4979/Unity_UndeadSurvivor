using System; // Enum.GetValues ���
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
        // Enum.GetValues() : �־��� �������� �����͸� ��� �������� �Լ�
        // (Achive[]) : Ÿ���� ��������� �����Ͽ� Ÿ�� ���߱�
        achives = (Achive[])Enum.GetValues(typeof(Achive));

        if (!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }

    void Init()
    {
        // PlayerPrefs : ����Ƽ�� �����ϴ� ������ ���� �Լ�
        // SetInt �Լ��� ����Ͽ� key�� ����� int�� ������ ����
        PlayerPrefs.SetInt("MyData", 1); // MyData �� 1 ����

        // ���� �����Ϳ� ������ �̸��� key�� 0�� ����
        foreach (Achive achive in achives)
        {
            PlayerPrefs.SetInt(achive.ToString(), 1); // 0 : �޼�X
            /*
            PlayerPrefs.SetInt("unlockPotato", 0); // 0 : �޼�X
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
            string achiveName = achives[index].ToString(); // ���� �̸� �ޱ�
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1; // ���� �޼� ���� �Ǵ�
            lockCharacter[index].SetActive(!isUnlock);
            unlockCharacter[index].SetActive(isUnlock);
        }
    }

    void Update()
    {
        
    }
}

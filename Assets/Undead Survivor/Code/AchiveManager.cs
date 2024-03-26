using System; // Enum.GetValues ���
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
        // Enum.GetValues() : �־��� �������� �����͸� ��� �������� �Լ�
        // (Achive[]) : Ÿ���� ��������� �����Ͽ� Ÿ�� ���߱�
        achives = (Achive[])Enum.GetValues(typeof(Achive));
        wait = new WaitForSecondsRealtime(5);

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
            PlayerPrefs.SetInt(achive.ToString(), 0); // 0 : �޼�X
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
        for (int index = 0; index < lockCharacter.Length; index++)
        {
            string achiveName = achives[index].ToString(); // ���� �̸� �ޱ�
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1; // ���� �޼� ���� �Ǵ�
            lockCharacter[index].SetActive(!isUnlock);
            unlockCharacter[index].SetActive(isUnlock);
        }
    }

    // �ļ� ������ ���� LateUpdate���� ����
    void LateUpdate()
    {
        // ������ Ȯ��
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
                isAchive = GameManager.instance.kill >= 10; // ų ���� 10 �̻��̸� true
                break;
            case Achive.UnlockBean:
                isAchive = GameManager.instance.gameTime == GameManager.instance.maxGameTime; // Ŭ���� ����
                break;
        }

        if(isAchive && PlayerPrefs.GetInt(achive.ToString()) == 0)
        {
            PlayerPrefs.SetInt(achive.ToString(), 1); // �ش� ���� �޼�

            // �˸� â�� �ڽ� ������Ʈ�� ��ȸ
            for(int index=0; index < uiNotice.transform.childCount; index++)
            {
                bool isActive = index == (int)achive; // ������ ������ Ȱ��ȭ
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

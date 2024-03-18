using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image

public class Item : MonoBehaviour
{
    public ItemData data; // ��ũ��Ƽ�� ������Ʈ ItemData
    public int level;
    public Weapon weapon; // Player�� ���� Weapon ������Ʈ�� ����
    public Gear gear; // ��� Ÿ���� ���� ����

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake()
    {
        // GetComponentsInChildren���� �ڽ� ������Ʈ �� �������� (ù��°�� �ڱ� �ڽ�)
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0]; // Item�� ù��° �ؽ�Ʈ
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName; // ������ �̸��� ������ �ʾ� �ٷ� �ʱ�ȭ
    }

    void OnEnable()
    {
        // ���� �ؽ�Ʈ ����
        textLevel.text = "Lv." + (level + 1);

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                // ������ % ����� ������ �� 100 ���ϱ�
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;
            default:
                textDesc.text = string.Format(data.itemDesc);
                break;
        }
    }

    // ��ư Ŭ�� �̺�Ʈ�� ������ �Լ�
    public void OnClick()
    {
        switch (data.itemType)
        {
            // ���� ���� case ���̱�
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0) // �ʱ�ȭ
                {
                    GameObject newWeapon = new GameObject(); // ���ο� ���� ������Ʈ�� �ڵ�� ����
                    // AddComponent<T> : ���ӿ�����Ʈ�� T ������Ʈ�� �߰��ϴ� �Լ�
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    nextDamage += data.baseDamage * data.damages[level];
                    nextCount += data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);
                }
                level++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if(level == 0) // �ʱ�ȭ
                {
                    GameObject newGear = new GameObject(); // ���ο� ���� ������Ʈ�� �ڵ�� ����
                    // AddComponent<T> : ���ӿ�����Ʈ�� T ������Ʈ�� �߰��ϴ� �Լ�
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }
                level++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;

        }

        if(level == data.damages.Length)
        {
            // �ִ� ������ �����ϸ� ��ư Ŭ�� ��Ȱ��ȭ
            GetComponent<Button>().interactable = false;
        }

    }
}

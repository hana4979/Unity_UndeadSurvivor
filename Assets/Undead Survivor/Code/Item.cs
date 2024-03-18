using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image

public class Item : MonoBehaviour
{
    public ItemData data; // 스크립티블 오브젝트 ItemData
    public int level;
    public Weapon weapon; // Player가 가진 Weapon 오브젝트와 연동
    public Gear gear; // 장비 타입의 변수 선언

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake()
    {
        // GetComponentsInChildren에서 자식 오브젝트 값 가져오기 (첫번째는 자기 자신)
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0]; // Item의 첫번째 텍스트
        textName = texts[1];
        textDesc = texts[2];
        textName.text = data.itemName; // 아이템 이름은 변하지 않아 바로 초기화
    }

    void OnEnable()
    {
        // 레벨 텍스트 갱신
        textLevel.text = "Lv." + (level + 1);

        switch (data.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                // 데미지 % 상승을 보여줄 땐 100 곱하기
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

    // 버튼 클릭 이벤트와 연결할 함수
    public void OnClick()
    {
        switch (data.itemType)
        {
            // 동일 로직 case 붙이기
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0) // 초기화
                {
                    GameObject newWeapon = new GameObject(); // 새로운 게임 오브젝트를 코드로 생성
                    // AddComponent<T> : 게임오브젝트에 T 컴포넌트를 추가하는 함수
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
                if(level == 0) // 초기화
                {
                    GameObject newGear = new GameObject(); // 새로운 게임 오브젝트를 코드로 생성
                    // AddComponent<T> : 게임오브젝트에 T 컴포넌트를 추가하는 함수
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
            // 최대 레벨에 도달하면 버튼 클릭 비활성화
            GetComponent<Button>().interactable = false;
        }

    }
}

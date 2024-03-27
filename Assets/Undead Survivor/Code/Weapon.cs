using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id; // 무기 ID
    public int prefabId; // 프리펩 ID
    public float damage; // 데미지
    public int count; // 개수, 총탄이 몬스터와 부딪혀도 사라지지 않고 계속 뚫고 나가는 수치
    public float speed; // 회전 속도

    float timer; // 일정 간격으로 발사하기 위함
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        switch (id)
        {
            case 0:
                // forward : (0,0,1), back : (0,0,-1) 가독성 높히는 방법
                // Update에서 이동이나 회전을 한다면 deltaTime 곱해주기
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime; // 프레임이 소비한 시간 == 게임 시간
                if (timer > speed)
                {
                    timer = 0f;
                    Fire(); // 발사
                }
                break;
        }

        // .. Test Code..
        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(10, 1);
        }
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        if(id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); // 데미지/관통을 올리는 장비가 있을 경우 적용
    }

    public void Init(ItemData data)
    {
        // Basic Set
        name = "Weapon" + data.itemId;
        transform.parent = player.transform; // 부모 오브젝트를 플레이어로 지정
        transform.localPosition = Vector3.zero; // 플레이어 안에서 위치를 (0,0,0)으로 (지역위치)

        // Property Set (각종 무기 속성 변수들을 스크립트블 오브젝트 데이터로 초기화)
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        // prefabId는 풀링 매니저의 변수에서 찾아서 초기화
        // 스크립트블 오브젝트의 독립성을 위해서 인덱스가 아닌 프리펩으로 설정
        for(int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if(data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }


        // 무기 ID에 따라 초기화
        switch (id)
        {
            case 0:
                speed = 150 * Character.WeaponSpeed;
                // count 수마다 근접무기 배치
                Batch();
                break;
            default:
                speed = 0.5f * Character.WeaponRate; // 연사 속도
                break;
        }

        // Hand Set
        // enum 열거형 데이터는 정수 형태로도 사용 가능
        Hand hand = player.hands[(int)data.itemType]; // int로 강제 형변환
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);

        // BroadcastMessage : 특정 함수 호출을 모든 자식에게 방송하는 함수
        // 두 번째 인자 : 꼭 받는 인자가 존재하지 않아도 된다
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver); // 장비가 생성되었을 때

    }

    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            // for문으로 count만큼 풀링에서 가져오기
            // Bullet에 부모를 바꾸기 위함
            Transform bullet;
            
            if(index < transform.childCount)
            {
                bullet = transform.GetChild(index); // 가지고 있는 child 재활용
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform; // 모자란 것을 가져오기
                bullet.parent = transform; // 나 자신으로 부모를 바꿈
            }

            // 배치하면서 먼저 위치, 회전 초기화 하기
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            //‘bullet’ 오브젝트를 월드 공간에서 위쪽 방향으로 1.5만큼 이동
            // Translate 함수로 자신의 위쪽으로 이동
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero); // -1 is Infinity Per. --> 관통력 -100 수정

        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget) // 근처에 대상이 없다면
            return;

        // .. 총알이 나아가고자 하는 방향 계산
        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position; // 크기가 포함된 방향
        // normalized : 현재 벡터의 방향은 유지하고 크기를 1로 변환된 속성(크기는 항상 동일하게 하기 위함)
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        // .. 위치와 회전 결정
        bullet.position = transform.position;
        // FromToRotation : 지정된 축을 중심으로 목표를 향해 회전하는 함수
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        // .. bullet에 전달
        bullet.GetComponent<Bullet>().Init(damage, count, dir);

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
    }

}

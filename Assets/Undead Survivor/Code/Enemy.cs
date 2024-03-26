using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float MaxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait; // 하나의 FixedUpdate를 기다리는 시간

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        // ..Hit 상태에서 Enemy가 Player에게 가는 로직을 멈춤
        // GetCurrentAnimatorStateInfo : 현재 상태 정보를 가져오는 함수
        // IsName : 해당 상태의 이름이 지정된 것과 같은지 확인하는 함수
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        // 위치 차이 : 타겟 위치 - 나의 위치
        Vector2 dirVec = target.position - rigid.position;
        // 방향 : 위치 차이의 정규화 (Normalized)
        // 프레임의 영향으로 결과가 달라지지 않도록 fixedDeltaTime 사용
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        // 물리 속도가 이동에 영향을 주지 않도록 속도 제거
        rigid.velocity = Vector2.zero; // (0, 0)

    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive)
            return;

        // 목표의 X축 값과 자신의 X축 값을 비교하여 작으면 true
        spriter.flipX = target.position.x < rigid.position.x;

    }

    // OnEnable : 스크립트가 활성화 될 때, 호출되는 이벤트 함수
    void OnEnable()
    {
        // 자기 스스로 초기화
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        // 재활용을 위해 되돌리기
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2; // Order in Layer
        anim.SetBool("Dead", false);
        health = MaxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        MaxHealth = data.health;
        health = data.health; // maxHealth 값 변경으로 인한 체력 동기화
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Bullet과 충돌하지 않았을 때(다른 오브젝트와 충돌)
        // 살아 있을 때만(사망 로직 연달아 실행 방지)
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack()); // == ("KockBack") 코루틴 호출 함수

        if(health > 0)
        {
            // .. Live, Hit Action
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit); // 피격 사운드
        }
        else
        {
            // .. Die
            isLive = false;
            coll.enabled = false; // 컴포넌트의 비활성화 .enabled = false
            rigid.simulated = false; // 리지드바디의 물리적 비활성화 .sumulated = false
            spriter.sortingOrder = 1; // Order in Layer
            anim.SetBool("Dead", true);
            GameManager.instance.kill++; // 킬수 증가
            GameManager.instance.GetExp();

            if(GameManager.instance.isLive) // 게임 진행 도중에만 효과음 재생
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead); // 사망 사운드
        }
    }

    // Coroutine : 생명 주기와 비동기처럼 실행되는 함수
    // IEnumerator : 코루틴만의 반환형 인터페이스
    IEnumerator KnockBack()
    {
        yield return wait; // 다음 하나의 물리 프레임 딜레이
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos; // 플레이어의 반대방향으로 가는 벡터
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse); // Impulse : 순간적인 힘
    }

    void Dead()
    {
        gameObject.SetActive(false); // 일단 비활성화
    }
}

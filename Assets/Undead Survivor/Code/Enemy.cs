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
    WaitForFixedUpdate wait; // �ϳ��� FixedUpdate�� ��ٸ��� �ð�

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

        // ..Hit ���¿��� Enemy�� Player���� ���� ������ ����
        // GetCurrentAnimatorStateInfo : ���� ���� ������ �������� �Լ�
        // IsName : �ش� ������ �̸��� ������ �Ͱ� ������ Ȯ���ϴ� �Լ�
        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        // ��ġ ���� : Ÿ�� ��ġ - ���� ��ġ
        Vector2 dirVec = target.position - rigid.position;
        // ���� : ��ġ ������ ����ȭ (Normalized)
        // �������� �������� ����� �޶����� �ʵ��� fixedDeltaTime ���
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        // ���� �ӵ��� �̵��� ������ ���� �ʵ��� �ӵ� ����
        rigid.velocity = Vector2.zero; // (0, 0)

    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive)
            return;

        // ��ǥ�� X�� ���� �ڽ��� X�� ���� ���Ͽ� ������ true
        spriter.flipX = target.position.x < rigid.position.x;

    }

    // OnEnable : ��ũ��Ʈ�� Ȱ��ȭ �� ��, ȣ��Ǵ� �̺�Ʈ �Լ�
    void OnEnable()
    {
        // �ڱ� ������ �ʱ�ȭ
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        // ��Ȱ���� ���� �ǵ�����
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
        health = data.health; // maxHealth �� �������� ���� ü�� ����ȭ
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Bullet�� �浹���� �ʾ��� ��(�ٸ� ������Ʈ�� �浹)
        // ��� ���� ����(��� ���� ���޾� ���� ����)
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        health -= collision.GetComponent<Bullet>().damage;
        StartCoroutine(KnockBack()); // == ("KockBack") �ڷ�ƾ ȣ�� �Լ�

        if(health > 0)
        {
            // .. Live, Hit Action
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit); // �ǰ� ����
        }
        else
        {
            // .. Die
            isLive = false;
            coll.enabled = false; // ������Ʈ�� ��Ȱ��ȭ .enabled = false
            rigid.simulated = false; // ������ٵ��� ������ ��Ȱ��ȭ .sumulated = false
            spriter.sortingOrder = 1; // Order in Layer
            anim.SetBool("Dead", true);
            GameManager.instance.kill++; // ų�� ����
            GameManager.instance.GetExp();

            if(GameManager.instance.isLive) // ���� ���� ���߿��� ȿ���� ���
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead); // ��� ����
        }
    }

    // Coroutine : ���� �ֱ�� �񵿱�ó�� ����Ǵ� �Լ�
    // IEnumerator : �ڷ�ƾ���� ��ȯ�� �������̽�
    IEnumerator KnockBack()
    {
        yield return wait; // ���� �ϳ��� ���� ������ ������
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos; // �÷��̾��� �ݴ�������� ���� ����
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse); // Impulse : �������� ��
    }

    void Dead()
    {
        gameObject.SetActive(false); // �ϴ� ��Ȱ��ȭ
    }
}

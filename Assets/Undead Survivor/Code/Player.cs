using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// MonoBehaviour : ���� ���� ������ �ʿ��� �͵��� ���� Ŭ����
public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner; // ���� ���� Scanner ��ũ��Ʈ �������� ����
    public Hand[] hands; // �÷��̾��� �� ��ũ��Ʈ�� ���� �迭 ���� ����
    public RuntimeAnimatorController[] animCon; // ���� �ִϸ����� ��Ʈ�ѷ��� ������ �迭 ���� ����

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    // �ʱ�ȭ ���� �Լ�
    void Awake()
    {
        // GetComponenet<T> : ������Ʈ���� ������Ʈ�� �������� �Լ�
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>(); // ���� ���� ��ũ��Ʈ�� ������Ʈ�� �����ϰ� ���
        hands = GetComponentsInChildren<Hand>(true); // true : ��Ȱ��ȭ �� ������Ʈ�� ����
    }

    void OnEnable()
    {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }
    
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        /*
        // GetAxis : �࿡ ���� ��, �Է� ���� �ε巴�� �ٲ�
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
        */
    }
    

    // ���� ����
    void FixedUpdate()
    {
        /*
        // 1. ���� �ֱ�
        rigid.AddForce(inputVec);

        // 2. �ӵ� ����
        rigid.velocity = inputVec; // �ӵ��� ���� ����
        */

        // 3. ��ġ �̵�

        if (!GameManager.instance.isLive)
            return;

        // normalized : ��Ÿ��� ������ ���� �밢������ �̵��� 1�� ������ �� ������ �̵�(����)
        // fixedDeltaTime : ���� ������ �ϳ��� �Һ��� �ð�
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    // �������� ���� �Ǳ� �� ����Ǵ� �����ֱ� �Լ�
    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        // magnitude : Returns the length of this vector (������ ������ ũ�� ��)
        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0; // inputVec.x�� ������� true���� flipX �� ��
        }
    }

    void OnCollisionStay2D(Collision2D collision) // �浹�ϴ� ���� ���������� ü���� ����
    {
        if (!GameManager.instance.isLive)
            return;

        // Time.deltaTime : ������ ���� ������ �����Ͽ� ������ �ǰ� ������ ���
        GameManager.instance.health -= Time.deltaTime * 10;

        if (GameManager.instance.health < 0)
        {
            // childCount : �ڽ� ������Ʈ�� ����
            for(int index = 2; index < transform.childCount; index++)
            {
                // GetChilde : �־��� �ε����� �ڽ� ������Ʈ�� ��ȯ�ϴ� �Լ�
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead"); // Dead �ִϸ��̼� ����
            GameManager.instance.GameOver();
        }
    }
}

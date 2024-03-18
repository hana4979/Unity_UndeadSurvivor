using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per; // 근접/원거리 나누는 역할, 원거리 총알이 몇 번 관통하는지 알려줌

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // rigidbody2D.velocity값은 항상 Vector2지만 Vector3 와 암묵적 변환이 가능해 오류가 안 남
    public void Init(float damage, int per, Vector3 dir)
    {
        // this : 해당 클래스의 변수로 접근
        this.damage = damage;
        this.per = per;

        // 관통이 -1(무한)보다 큰 것에 대해서는 속도 적용
        if (per > -1)
        {
            // velocity : 속도
            rigid.velocity = dir * 15; // 속력을 곱해주어 총알이 날아가는 속도 증가시키기
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -1)
            return;

        per--; // 관통력 줄어듦

        if(per == -1) // 역할을 다 함
        {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false); // 비활성화, 오브젝트 풀링으로 관리
        }

    }
    
}

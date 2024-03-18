using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per; // ����/���Ÿ� ������ ����, ���Ÿ� �Ѿ��� �� �� �����ϴ��� �˷���

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // rigidbody2D.velocity���� �׻� Vector2���� Vector3 �� �Ϲ��� ��ȯ�� ������ ������ �� ��
    public void Init(float damage, int per, Vector3 dir)
    {
        // this : �ش� Ŭ������ ������ ����
        this.damage = damage;
        this.per = per;

        // ������ -1(����)���� ū �Ϳ� ���ؼ��� �ӵ� ����
        if (per > -1)
        {
            // velocity : �ӵ�
            rigid.velocity = dir * 15; // �ӷ��� �����־� �Ѿ��� ���ư��� �ӵ� ������Ű��
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -1)
            return;

        per--; // ����� �پ��

        if(per == -1) // ������ �� ��
        {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false); // ��Ȱ��ȭ, ������Ʈ Ǯ������ ����
        }

    }
    
}

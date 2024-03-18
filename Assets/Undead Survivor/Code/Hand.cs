using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft; // �޼� ������ ����
    public SpriteRenderer spriter;

    SpriteRenderer player; // �÷��̾��� ��������Ʈ������ (�������� Ȯ��)

    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0); // ������ ���� ��ġ
    Vector3 rightPosReverse = new Vector3(-0.15f, -0.15f, 0); // ���� �� ������ ���� ��ġ
    Quaternion leftRot = Quaternion.Euler(0, 0, -35); // �޼� ������ ȸ����
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -135); // ���� �� �޼� ���� ȸ����

    void Awake()
    {
        // �ڱ� �ڽ��� ��������Ʈ�������� ���� ��� 0
        player = GetComponentsInParent<SpriteRenderer>()[1]; // �ι�°�� �θ��� ��������Ʈ
    }

    void LateUpdate()
    {
        bool isReverse = player.flipX;

        if (isLeft) // ��������
        {
            // localRotation : ������Ʈ�� ���������� ��
            transform.localRotation = isReverse ? leftRotReverse : leftRot;
            spriter.flipY = isReverse;
            spriter.sortingOrder = isReverse ? 4 : 6; // ���� �� 4 , �׷��� ������ 6
        }
        else // ���Ÿ�����
        {
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            spriter.flipX = isReverse;
            spriter.sortingOrder = isReverse ? 6 : 4;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange; // ��ĵ ����
    public LayerMask targetLayer; // Ÿ�� ���̾�
    public RaycastHit2D[] targets; // ��ĵ ��� �迭
    public Transform nearestTarget; // ���� ����� ��ǥ

    void FixedUpdate()
    {
        // CircleCastAll : ������ ĳ��Ʈ�� ��� ��� ����� ��ȯ�ϴ� �Լ�
        // (ĳ���� ���� ��ġ, ���� ������, ĳ���� ����, ĳ���� ����, ��� ���̾�)
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
    }

    Transform GetNearest()
    {
        Transform result = null;
        float diff = 100; // (�ӽ�)���� ū �Ÿ�

        foreach(RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;

            // Distance(A, B) : ����A�� B�� �Ÿ��� ������ִ� �Լ�
            float curDiff = Vector3.Distance(myPos, targetPos);

            // ������ �Ÿ��� ����� �Ÿ����� ������ ��ü
            if(curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;

            }
        }

        return result;
    }


}

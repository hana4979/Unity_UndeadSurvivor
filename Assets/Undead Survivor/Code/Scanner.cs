using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange; // 스캔 범위
    public LayerMask targetLayer; // 타겟 레이어
    public RaycastHit2D[] targets; // 스캔 결과 배열
    public Transform nearestTarget; // 가장 가까운 목표

    void FixedUpdate()
    {
        // CircleCastAll : 원형의 캐스트를 쏘고 모든 결과를 반환하는 함수
        // (캐스팅 시작 위치, 원의 반지름, 캐스팅 방향, 캐스팅 길이, 대상 레이어)
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
    }

    Transform GetNearest()
    {
        Transform result = null;
        float diff = 100; // (임시)가장 큰 거리

        foreach(RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;

            // Distance(A, B) : 벡터A와 B의 거리를 계산해주는 함수
            float curDiff = Vector3.Distance(myPos, targetPos);

            // 가져온 거리가 저장된 거리보다 작으면 교체
            if(curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;

            }
        }

        return result;
    }


}

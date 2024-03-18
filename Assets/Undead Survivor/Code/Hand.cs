using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft; // 왼손 오른손 구분
    public SpriteRenderer spriter;

    SpriteRenderer player; // 플레이어의 스프라이트렌더러 (반전여부 확인)

    Vector3 rightPos = new Vector3(0.35f, -0.15f, 0); // 오른손 무기 위치
    Vector3 rightPosReverse = new Vector3(-0.15f, -0.15f, 0); // 반전 시 오른손 무기 위치
    Quaternion leftRot = Quaternion.Euler(0, 0, -35); // 왼손 무기의 회전값
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -135); // 반전 시 왼손 무기 회전값

    void Awake()
    {
        // 자기 자신의 스프라이트렌더러가 있을 경우 0
        player = GetComponentsInParent<SpriteRenderer>()[1]; // 두번째가 부모의 스프라이트
    }

    void LateUpdate()
    {
        bool isReverse = player.flipX;

        if (isLeft) // 근접무기
        {
            // localRotation : 오브젝트를 기준으로한 축
            transform.localRotation = isReverse ? leftRotReverse : leftRot;
            spriter.flipY = isReverse;
            spriter.sortingOrder = isReverse ? 4 : 6; // 반전 시 4 , 그렇지 않으면 6
        }
        else // 원거리무기
        {
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            spriter.flipX = isReverse;
            spriter.sortingOrder = isReverse ? 6 : 4;
        }
    }
}

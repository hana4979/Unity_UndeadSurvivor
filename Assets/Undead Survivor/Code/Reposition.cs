using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area")) // Area tag가 아니라면
            return;

        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 myPos = transform.position;

        /*
        // 플레이어의 이동 방향을 저장하기 위한 변수
        // normalized로 인한 보정으로 대각선 이동 값이 1보다 작음
        float dirX = playerPos.x - myPos.x;
        float dirY = playerPos.y - myPos.y;

        float diffX = Mathf.Abs(dirX); // 절대값으로 계산
        float diffY = Mathf.Abs(dirY);

        dirX = dirX > 0 ? 1 : -1;
        dirY = dirY > 0 ? 1 : -1;
        */

        switch (transform.tag)
        {
            case "Ground":
                // 위치 차이
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;
                // 위치 차이에 따른 방향값 설정
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;
                // 위치 차이 절대값만 알면 됨
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if (diffX > diffY) // X축 방향으로 더 이동
                {
                    // Translate : 좌표 X , 이동할 양
                    transform.Translate(Vector3.right * dirX * 40); // 두 칸 수평 이동할 것이라 40
                }
                else if (diffX < diffY) // Y축 방향으로 더 이동
                {
                    transform.Translate(Vector3.up * dirY * 40);
                }
                break;
            case "Enemy":
                if (GetComponent<Collider2D>().enabled)
                {
                    Vector3 dist = playerPos - myPos; // 거리 차이
                    Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0); // 랜덤한 X, Y, Z 좌표
                    transform.Translate(ran + dist * 2);

                    /*
                    if (diffX > diffY)
                    {
                        // Random.Range(-3f, 3f) 적당한 위치에서 랜덤하게 나오도록 설정
                        transform.Translate(Vector3.right * dirX * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));
                    }
                    else if (diffX < diffY)
                    {
                        transform.Translate(Vector3.up * dirY * 20 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f));
                    }
                    */
                }
                break;
        }
    }

}

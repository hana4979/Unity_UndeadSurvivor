using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// MonoBehaviour : 게임 로직 구성에 필요한 것들을 가진 클래스
public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner; // 직접 만든 Scanner 스크립트 가져오기 위함
    public Hand[] hands; // 플레이어의 손 스크립트를 담을 배열 변수 선언
    public RuntimeAnimatorController[] animCon; // 여러 애니메이터 컨트롤러를 저장할 배열 변수 선언

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    // 초기화 진행 함수
    void Awake()
    {
        // GetComponenet<T> : 오브젝트에서 컴포넌트를 가져오는 함수
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>(); // 직접 만든 스크립트도 컴포넌트로 동일하게 취급
        hands = GetComponentsInChildren<Hand>(true); // true : 비활성화 된 오브젝트도 포함
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
        // GetAxis : 축에 대한 값, 입력 값이 부드럽게 바뀜
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
        */
    }
    

    // 물리 수정
    void FixedUpdate()
    {
        /*
        // 1. 힘을 주기
        rigid.AddForce(inputVec);

        // 2. 속도 제어
        rigid.velocity = inputVec; // 속도를 직접 제어
        */

        // 3. 위치 이동

        if (!GameManager.instance.isLive)
            return;

        // normalized : 피타고라스 정리에 의해 대각선으로 이동시 1의 값보다 더 빠르게 이동(보정)
        // fixedDeltaTime : 물리 프레임 하나가 소비한 시간
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    // 프레임이 종료 되기 전 실행되는 생명주기 함수
    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        // magnitude : Returns the length of this vector (벡터의 순수한 크기 값)
        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0; // inputVec.x가 음수라면 true값이 flipX 에 들어감
        }
    }

    void OnCollisionStay2D(Collision2D collision) // 충돌하는 동안 지속적으로 체력이 깎임
    {
        if (!GameManager.instance.isLive)
            return;

        // Time.deltaTime : 프레임 단위 실행을 감안하여 적절한 피격 데미지 계산
        GameManager.instance.health -= Time.deltaTime * 10;

        if (GameManager.instance.health < 0)
        {
            // childCount : 자식 오브젝트의 개수
            for(int index = 2; index < transform.childCount; index++)
            {
                // GetChilde : 주어진 인덱스의 자식 오브젝트를 반환하는 함수
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead"); // Dead 애니메이션 실행
            GameManager.instance.GameOver();
        }
    }
}

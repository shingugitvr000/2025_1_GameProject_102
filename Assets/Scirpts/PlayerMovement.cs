using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;                          //이동 속도 변수 설정
    public float jumpForce = 5.0f;                          //점프의 침 값을 준다. 

    public bool isGrounded = true;                          //땅에 있는지 체크 하는 변수 (true/false)

    public int coinCount = 0;                               //코인 획득 변수 선언
    public int totalCoins = 5;                              //총 코인 획득 필요 변수 선언 

    public Rigidbody rb;                                    //플레이어 강체를 선언

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //움직임 입력
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //속도값으로 직접 이동
        rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);  
        
        //점프 입력
        if(Input.GetButtonDown("Jump") && isGrounded)   //&& 두 값이 True 일때 -> (Jump 버튼 {보통 스페이스바} 와 땅 위에 있을 때)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);     //위쪽으로 설정한 힘만큼 강체에 힘을 전달한다. 
            isGrounded = false;                                         //점프를 하는 순간 땅에서 떨어졌기 때문에 False 라고 한다. 
        }
    }

    void OnCollisionEnter(Collision collision)          //충돌이 일어났을때 호출 되는 함수 
    {
        if (collision.gameObject.tag == "Ground")            //충돌이 일어난 불체의 Tag가 Ground인 경우
        {
            isGrounded = true;                         //땅과 충돌 했을때 true로 변경 해준다. 
        }
    }

    void OnTriggerEnter(Collider other)         //트리거 영역 안에 들어왔다를 감시하는 함수 
    {
        //코인 수집
        if(other.CompareTag("Coin"))                                    //코인 트리거와 충돌 하면
        {
            coinCount++;                                                //코인 변수 1 증가 시킨다. ++은 1을 증가 시킨다는 축약
            Destroy(other.gameObject);                                  //코인 오브젝트를 제거한다. 
            Debug.Log($"코인 수집 : {coinCount}/{totalCoins}");         //수집한 코인 1/5 식으로 콘솔 로그에 표현한다. 
        }

        if (other.CompareTag("Door") && coinCount >= totalCoins)        //코인 전체 카운트보다 코인을 더 수집 했을 경우
        {
            Debug.Log("게임 클리어");
            //이후 완료 연출 및 Scene 전환 한다. 
        }
    }
}

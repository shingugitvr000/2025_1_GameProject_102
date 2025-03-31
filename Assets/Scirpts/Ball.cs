using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{   
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)          //충돌이 일어났을때 호출 되는 함수 
    {
        if(collision.gameObject.tag == "Ground")            //충돌이 일어난 불체의 Tag가 Ground인 경우
        {
            Debug.Log("땅과 충돌");                         //충돌이 일어났을 경우 로그로 확인한다. 
        }
    }

    void OnTriggerEnter(Collider other)         //트리거 영역 안에 들어왔다를 감시하는 함수 
    {
        Debug.Log("큐브 범위 안에 들어옴");                        
    }
}

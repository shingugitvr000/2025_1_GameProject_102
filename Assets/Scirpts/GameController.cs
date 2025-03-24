using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float Timer = 1.0f;                              //타이머 변수를 선언 한다. (float)
    public GameObject EnemyObject;                          //적 오브젝트를 선언 한다. (GameObject)

    void Update()
    {
        Timer -= Time.deltaTime;            //시간을 매 프레이마다 감소 시킨다 (deltaTime 프레임 간격의 시간 을 의미합니다)
                                            //(Timer = Timer - Time.deltaTime)
        if (Timer <= 0)                     //만약 Timer 의 수치가 0이하로 내려갈 경우 (1초마다 동작되는 행동을 만들때)
        {
            Timer = 1;                      //다시 1초로 타이머를 초기화 시켜준다.

            GameObject Temp = Instantiate(EnemyObject);                     //선언한 게임 오브젝트를 복제 생성한다. 
            Temp.transform.position = new Vector3(Random.Range(-8, 8), Random.Range(-4, 4), 0);     //위치는 랜덤으로 옮긴다. 
        }

        if (Input.GetMouseButtonDown(0))                                        //마우스 버튼을 눌렀을 때 
        {
            RaycastHit hit;                                                     //물리 Hit 선언
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);        //카메라에서 Ray를 쏴서 3D 공간상의 물체를 확인한다. 

            if (Physics.Raycast(ray, out hit))                                  //Ray를 쐈을때 Hit 되는 물체가 있으면 
            {
                if (hit.collider != null)                                       //물체가 존재 하면 
                {
                    //Debug.Log($"hit : {hit.collider.name}");                    //물체 이름을 출력한다. 
                    hit.collider.gameObject.GetComponent<Enemy>().CharacterHit(30);     //Enemy 스크립트의 히트 함수를 호출 한다. 
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float Timer = 1.0f;                              //Ÿ�̸� ������ ���� �Ѵ�. (float)
    public GameObject EnemyObject;                          //�� ������Ʈ�� ���� �Ѵ�. (GameObject)

    void Update()
    {
        Timer -= Time.deltaTime;            //�ð��� �� �����̸��� ���� ��Ų�� (deltaTime ������ ������ �ð� �� �ǹ��մϴ�)
                                            //(Timer = Timer - Time.deltaTime)
        if (Timer <= 0)                     //���� Timer �� ��ġ�� 0���Ϸ� ������ ��� (1�ʸ��� ���۵Ǵ� �ൿ�� ���鶧)
        {
            Timer = 1;                      //�ٽ� 1�ʷ� Ÿ�̸Ӹ� �ʱ�ȭ �����ش�.

            GameObject Temp = Instantiate(EnemyObject);                     //������ ���� ������Ʈ�� ���� �����Ѵ�. 
            Temp.transform.position = new Vector3(Random.Range(-8, 8), Random.Range(-4, 4), 0);     //��ġ�� �������� �ű��. 
        }

        if (Input.GetMouseButtonDown(0))                                        //���콺 ��ư�� ������ �� 
        {
            RaycastHit hit;                                                     //���� Hit ����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);        //ī�޶󿡼� Ray�� ���� 3D �������� ��ü�� Ȯ���Ѵ�. 

            if (Physics.Raycast(ray, out hit))                                  //Ray�� ������ Hit �Ǵ� ��ü�� ������ 
            {
                if (hit.collider != null)                                       //��ü�� ���� �ϸ� 
                {
                    //Debug.Log($"hit : {hit.collider.name}");                    //��ü �̸��� ����Ѵ�. 
                    hit.collider.gameObject.GetComponent<Enemy>().CharacterHit(30);     //Enemy ��ũ��Ʈ�� ��Ʈ �Լ��� ȣ�� �Ѵ�. 
                }
            }
        }
    }
}

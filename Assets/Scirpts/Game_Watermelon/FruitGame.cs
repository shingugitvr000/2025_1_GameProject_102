using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitGame : MonoBehaviour
{
    public GameObject[] fruitPrefabs;               //과일 프리팹 배열 (익스펙터에서 할당)

    public float[] fruitSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };     //과일 크기 배열

    public GameObject currentFruit;                     //현재 들고 있는 과일
    public int currentFruitType;

    public float fruitStartHeight = 6f;                 //과일 시작 높이 (인스펙터에서 조절 가능)

    public float gameWidth = 5f;                        //게임판 정보 

    public bool isGameOver = false;                     //게임 상태

    public Camera mainCamera;                           //카메라 참조(마우스 위치 변환에 필요)

    public float fruitTimer;                            //잰 시간 설정을 위한 타이머

    public float gameHeight;                                //게임 높이 설정 

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;       //메인 카메라 참조 가저 오기 

        SpawnNewFruit();                        //게임 시작 시 첫 과일 생성 
        fruitTimer = -3.0f;                     //타이머 시간을 -3으로 보낸다. 
    }

    // Update is called once per frame
    void Update()
    {
        //게임 오버면 종료
        if (isGameOver)
            return;

        if(fruitTimer >= 0)                         //타이머가 0보타 클 경우 
        fruitTimer -= Time.deltaTime;

        if(fruitTimer < 0 && fruitTimer > -2)       //타이머 시간이 0 과 - 2 사이에 있을때 잰을 하고 
        {
            CheckGameOver();
            SpawnNewFruit();
            fruitTimer = -3.0f;                     //타이머 시간을 -3으로 보낸다. 
        }



        if (currentFruit != null)                //현재 과일이 있을 때만 처리 
        {
            Vector3 mousePosition = Input.mousePosition;                            //마우스 위치를 따라 X 좌표만 이동 시키기 위해 사용
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            Vector3 newPosition = currentFruit.transform.position;              //과일 위치 업데이트 (X좌표만, Y는 그대로 유지 )
            newPosition.x = worldPosition.x;

            float halfFruitSize = fruitSizes[currentFruitType] / 2;                 //과일이 원이기 때문에 반 값을 나눠서 바구니 밖으로 안나가게 설정
            if(newPosition.x < -gameWidth / 2 + halfFruitSize)
            {
                newPosition.x = -gameWidth / 2 + halfFruitSize;
            }
            if (newPosition.x > gameWidth / 2 - halfFruitSize)
            {
                newPosition.x = gameWidth / 2 - halfFruitSize;
            }
            currentFruit.transform.position = newPosition;                              //과일 좌표 갱신 
        }

        //마우스를 좌 클릭하면 과일 떨어뜨리기 연타시오류가 나기 때문에 시간을 보고 드랍되게 설정 
        if(Input.GetMouseButtonDown(0) && fruitTimer == -3.0f)
        {
            DropFruit();
        }

    }

    public void MergeFruits(int fruitType , Vector3 position)
    {
        if(fruitType < fruitPrefabs.Length -1)          //마지막 과일 타입이 아니라면
        {
            GameObject newFruit = Instantiate(fruitPrefabs[fruitType + 1] , position , Quaternion.identity);    //다음 단계 과일 생성

            newFruit.transform.localScale = new Vector3(fruitSizes[fruitType + 1], fruitSizes[fruitType + 1], 1.0f);

            //점수 추가 로직 
        }
    }


    void SpawnNewFruit()            //과일 생성 함수 
    {
        if(!isGameOver)                         //게임 오버가 아닐 때만 새 과일 생성 
        {
            currentFruitType = Random.Range(0, 3);          //0 ~ 2 사이의 랜덤 과일 타입 

            Vector3 mousePostion = Input.mousePosition;
            Vector3 worldPoision = mainCamera.ScreenToWorldPoint(mousePostion);     //마우스 위치를 월드 좌표로 변환

            Vector3 spawnPosition = new Vector3(worldPoision.x, fruitStartHeight, 0);   //X 좌표만 사용하고 Y는 설정된 높이로 , Z는 2D라서 0으로 설정

            float halfFruitSize = fruitSizes[currentFruitType] / 2;
            spawnPosition.x = Mathf.Clamp(spawnPosition.x, -gameWidth / 2 + halfFruitSize, gameWidth / 2 - halfFruitSize);  //X 위치가 게임 영역을 벗어나지 않도록 제한

            currentFruit = Instantiate(fruitPrefabs[currentFruitType], spawnPosition, Quaternion.identity);     // 과일 생성

            currentFruit.transform.localScale = new Vector3(fruitSizes[currentFruitType], fruitSizes[currentFruitType], 1f); //과일 크기 설정 

            Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;                                   //중력을 0으로 설정 한다. 
            }
        }
    }

    void DropFruit()
    {
        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        if(rb !=null)
        {
            rb.gravityScale = 1f;

            currentFruit = null;            //현재 들고 있는 과일 해제 

            fruitTimer = 1.0f;
        }
    }

    void CheckGameOver()                    //게임 오버 체크 
    {
        Fruit[] allFruits = FindObjectsOfType<Fruit>();

        float gameOverHeight = gameHeight;             //일정 높이보다 높은 위치에 과일이 있는 지 확인 

        for(int i = 0; i < allFruits.Length; i++)
        {
            if(allFruits[i] != null)
            {
                Rigidbody2D rb = allFruits[i].GetComponent<Rigidbody2D>();
                //과일이 정지 상태이고 높은 위치에 있다면
                if (rb != null && rb.velocity.magnitude < 0.1f && allFruits[i].transform.position.y > gameOverHeight)
                {
                    //게임 오버
                    isGameOver = true;
                    Debug.Log("게임 오버");

                    break;
                }
            }          
        }
    }
}

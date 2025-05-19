using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableRank : MonoBehaviour
{

    public int rankLevel = 1;                   //계급장 레벨
    public float dragSpeed = 10f;               //드래그 시 이동 속도
    public float snapBackSpeed = 20f;           //원위치로 돌아가는 속도

    public bool isDragging = false;             //현재 드래그 중인지
    public Vector3 originalPosition;            //원래 위치 
    public GridCell currentCell;                //현재 위치한 칸

    public Camera mainCamera;                   //메인 카메라
    public Vector3 dragOffset;                  //드래그 시 오프셋 (보정값)
    public SpriteRenderer spriteRenderer;       //계급장 이미지 렌더러
    public GameManager gamemanager;           //게임 메니저

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToCell(GridCell targetCell)     //특정 칸으로 이동
    {
        if (currentCell != null)                
        {
            currentCell.currentRank = null; //기존 칸에서 제거
        }

        currentCell = targetCell;           //새 칸으로 이동 
        targetCell.currentRank = this;

        originalPosition = new Vector3(targetCell.transform.position.x, targetCell.transform.position.y, 0f);
        transform.position = originalPosition;
    }

    public void ReturnToOriginalPosition()//원래 위치로 돌아가는 함수 
    {
        transform.position = originalPosition;
    }

    public void MergeWithCell(GridCell targetCell)
    {
        if(targetCell.currentRank == null || targetCell.currentRank.rankLevel != rankLevel) //같은 레벨인지 확인
        {
            ReturnToOriginalPosition(); //원래 위치로 돌아가기
            return;
        }

        if (currentCell != null)
        {
            currentCell.currentRank = null; //기존 칸에서 제거
        }

        //합치기 실행 MergeRanks 함수를 통해서 실행 

    }

    public Vector3 GetMouseWorldPosition()                  //마우스 월드 좌표 구하기 
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    public void SetRankLevel(int level)
    {
        rankLevel = level;

        if(gamemanager != null && gamemanager.rankSprites.Length > level - 1)
        {
            spriteRenderer.sprite = gamemanager.rankSprites[level - 1];     //레벨에 맞는 스프라이트로 변경 
        }
    }


}

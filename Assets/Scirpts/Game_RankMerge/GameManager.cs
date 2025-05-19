using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int gridWidth = 7;                           //가로 칸 수
    public int gridHeight = 7;                          //세로 칸 수
    public float cellSize = 1.4f;                       //각 칸의 크기
    public GameObject cellPrefabs;                      //빈칸 프리팹
    public Transform gridContainer;                     //그리드를 담을 부모 오브젝트 

    public GameObject rankPrefabs;                      //계급장 프리팹
    public Sprite[] rankSprites;                        //각 레벨별 계급장 이미지
    public int maxRankLevel = 7;                       //최대 계급장 레벨 

    public GridCell[,] grid;                            //모든 칸을 저장하는 2차원 배열

    void InitializeGrid()                   //그리드 초기화 
    {
        grid = new GridCell[gridWidth, gridHeight];         //2차원 배열 생성

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(
                    x * cellSize - (gridWidth * cellSize / 2) + cellSize / 2,
                    y * cellSize - (gridHeight * cellSize / 2) + cellSize / 2,
                    1f
                );

                GameObject cellObj = Instantiate(cellPrefabs, position, Quaternion.identity, gridContainer);
                GridCell cell = cellObj.AddComponent<GridCell>();
                cell.Initialize(x, y);

                grid[x,y] = cell;       //배열에 저장
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeGrid();
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            SpawnNewRank();
        }
    }

    public DraggableRank CreateRankInCell(GridCell cell, int level)
    {
       
        if (cell == null || !cell.IsEmpty()) return null;  //비어있는 칸이 아니면 생성 실패

        level = Mathf.Clamp(level, 1, maxRankLevel);            //레벨 넘위 확인

        Vector3 rankPosition = new Vector3(cell.transform.position.x, cell.transform.position.y, 0f);   //계급장 위치 설정 

        //드래그 가능한 계급장 컴포넌트 추가
        GameObject rankObj = Instantiate(rankPrefabs, rankPosition, Quaternion.identity, gridContainer);    
        rankObj.name = "Rank_Lvel" + level;

        DraggableRank rank = rankObj.AddComponent<DraggableRank>();
        rank.SetRankLevel(level);

        cell.SetRank(rank);

        return rank;

    }

    private GridCell FindEmptyCell()            //비어있는 칸 찾기
    {
        List<GridCell> emptyCells = new List<GridCell>();       //빈 칸들을 저장할 리스트

        for(int x = 0; x < gridWidth; x++)                  //모든 칸을 검사 
        {
            for(int y = 0; y < gridHeight; y++)
            {
                if(grid[x,y].IsEmpty())         // 칸이라면 리스트에 추가
                {
                    emptyCells.Add(grid[x,y]);
                }
            }
        }

        if(emptyCells.Count == 0)               //빈칸이 없으면 null 값 반환
        {
            return null;
        }

        return emptyCells[Random.Range(0, emptyCells.Count)];       //랜덤하게 빈 칸 하나 선택 

    }

    public bool SpawnNewRank()      //새 계급장 생성
    {
        GridCell emptyCell = FindEmptyCell();       //1. 비어있는 칸 찾기
        if(emptyCell == null) return false;         //2. 비어있는 칸이 없으면 실패

        int rankLevel = Random.Range(0, 100) < 80 ? 1 : 2;  //80% 확률로 레벨 1, 20%확률로 레벨 2

        CreateRankInCell(emptyCell, rankLevel);     //3. 계급장 생성 및 설정

        return true;
    }

    public GridCell FindClosestCell(Vector3 position)       //가장 가까운 칸 찾기
    {
        for (int x = 0; x < gridWidth; x++)                 //1. 먼저 위치가 포함된 칸 확인
        {
            for(int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].ContainsPosition(position))
                {
                    return grid[x, y];
                }
            }
        }

        GridCell closestCell = null;                        //2. 없다면 가장 가까운 칸 찾기
        float closestDistance = float.MaxValue;         

        for(int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float distance = Vector3.Distance(position, grid[x, y].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCell = grid[x, y];
                }
            }
        }

        if( closestDistance > cellSize * 2)         //너무 멀면 null 반환
        {
            return null;
        }

        return closestCell;
    }

    public void MergeRanks(DraggableRank draggedRank , DraggableRank targetRank)
    {
        if(draggedRank == null || targetRank == null || draggedRank.rankLevel != targetRank.rankLevel)  //같은 레벨이 아니면 합치기 실패
        {
            if (draggedRank != null) draggedRank.ReturnToOriginalPosition();
            return;
        }      

        int newLevel = targetRank.rankLevel + 1;   //새 레벨 계산
        if (newLevel > maxRankLevel)                //최대 레벨 초과 시 처리 
        {
            RemoveRank(draggedRank);                    //드래그한 계급장만 제거
            return;
        }

        targetRank.SetRankLevel(newLevel);          //타겟 게급장 레벨 업그레이드
        RemoveRank(draggedRank);                    //드래그한 계급장 제거

        if (Random.Range(0, 100) < 60)            //60% 확률로 계급장 합치기 성공 시 랜덤으로 새 계급장 생성
        {
            SpawnNewRank();
        }
    }

    public void RemoveRank(DraggableRank rank)      //계급장 제거 
    {
        if (rank == null) return;

        if (rank.currentCell != null)           //칸에서 제거
        {
            rank.currentCell.currentRank = null;            
        }

        Destroy(rank.gameObject);           //게임 오브젝트 제거
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int gridWidth = 7;                           //���� ĭ ��
    public int gridHeight = 7;                          //���� ĭ ��
    public float cellSize = 1.4f;                       //�� ĭ�� ũ��
    public GameObject cellPrefabs;                      //��ĭ ������
    public Transform gridContainer;                     //�׸��带 ���� �θ� ������Ʈ 

    public GameObject rankPrefabs;                      //����� ������
    public Sprite[] rankSprites;                        //�� ������ ����� �̹���
    public int maxRankLevel = 7;                       //�ִ� ����� ���� 

    public GridCell[,] grid;                            //��� ĭ�� �����ϴ� 2���� �迭

    void InitializeGrid()                   //�׸��� �ʱ�ȭ 
    {
        grid = new GridCell[gridWidth, gridHeight];         //2���� �迭 ����

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

                grid[x,y] = cell;       //�迭�� ����
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
       
        if (cell == null || !cell.IsEmpty()) return null;  //����ִ� ĭ�� �ƴϸ� ���� ����

        level = Mathf.Clamp(level, 1, maxRankLevel);            //���� ���� Ȯ��

        Vector3 rankPosition = new Vector3(cell.transform.position.x, cell.transform.position.y, 0f);   //����� ��ġ ���� 

        //�巡�� ������ ����� ������Ʈ �߰�
        GameObject rankObj = Instantiate(rankPrefabs, rankPosition, Quaternion.identity, gridContainer);    
        rankObj.name = "Rank_Lvel" + level;

        DraggableRank rank = rankObj.AddComponent<DraggableRank>();
        rank.SetRankLevel(level);

        cell.SetRank(rank);

        return rank;

    }

    private GridCell FindEmptyCell()            //����ִ� ĭ ã��
    {
        List<GridCell> emptyCells = new List<GridCell>();       //�� ĭ���� ������ ����Ʈ

        for(int x = 0; x < gridWidth; x++)                  //��� ĭ�� �˻� 
        {
            for(int y = 0; y < gridHeight; y++)
            {
                if(grid[x,y].IsEmpty())         // ĭ�̶�� ����Ʈ�� �߰�
                {
                    emptyCells.Add(grid[x,y]);
                }
            }
        }

        if(emptyCells.Count == 0)               //��ĭ�� ������ null �� ��ȯ
        {
            return null;
        }

        return emptyCells[Random.Range(0, emptyCells.Count)];       //�����ϰ� �� ĭ �ϳ� ���� 

    }

    public bool SpawnNewRank()      //�� ����� ����
    {
        GridCell emptyCell = FindEmptyCell();       //1. ����ִ� ĭ ã��
        if(emptyCell == null) return false;         //2. ����ִ� ĭ�� ������ ����

        int rankLevel = Random.Range(0, 100) < 80 ? 1 : 2;  //80% Ȯ���� ���� 1, 20%Ȯ���� ���� 2

        CreateRankInCell(emptyCell, rankLevel);     //3. ����� ���� �� ����

        return true;
    }

    public GridCell FindClosestCell(Vector3 position)       //���� ����� ĭ ã��
    {
        for (int x = 0; x < gridWidth; x++)                 //1. ���� ��ġ�� ���Ե� ĭ Ȯ��
        {
            for(int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y].ContainsPosition(position))
                {
                    return grid[x, y];
                }
            }
        }

        GridCell closestCell = null;                        //2. ���ٸ� ���� ����� ĭ ã��
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

        if( closestDistance > cellSize * 2)         //�ʹ� �ָ� null ��ȯ
        {
            return null;
        }

        return closestCell;
    }

    public void MergeRanks(DraggableRank draggedRank , DraggableRank targetRank)
    {
        if(draggedRank == null || targetRank == null || draggedRank.rankLevel != targetRank.rankLevel)  //���� ������ �ƴϸ� ��ġ�� ����
        {
            if (draggedRank != null) draggedRank.ReturnToOriginalPosition();
            return;
        }      

        int newLevel = targetRank.rankLevel + 1;   //�� ���� ���
        if (newLevel > maxRankLevel)                //�ִ� ���� �ʰ� �� ó�� 
        {
            RemoveRank(draggedRank);                    //�巡���� ����常 ����
            return;
        }

        targetRank.SetRankLevel(newLevel);          //Ÿ�� �Ա��� ���� ���׷��̵�
        RemoveRank(draggedRank);                    //�巡���� ����� ����

        if (Random.Range(0, 100) < 60)            //60% Ȯ���� ����� ��ġ�� ���� �� �������� �� ����� ����
        {
            SpawnNewRank();
        }
    }

    public void RemoveRank(DraggableRank rank)      //����� ���� 
    {
        if (rank == null) return;

        if (rank.currentCell != null)           //ĭ���� ����
        {
            rank.currentCell.currentRank = null;            
        }

        Destroy(rank.gameObject);           //���� ������Ʈ ����
    }
}

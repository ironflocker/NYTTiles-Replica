using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CustomGrid : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject cellPrefab;
    public GridLayoutGroup gridLayoutGroup;

    private RectTransform rectTransform;
    public List<LayerDistributor> gridCells; 

    private float currentGridWidth;
    private float currentGridHeight;

    /* Custom Events */
    public Action<float, float> OnGridGenerated;
    public Action AllCellsEmptyEvent;

    public float CurrentGridWidth
    {
        get { return currentGridWidth; }
        set { currentGridWidth = value; }
    }

    public float CurrentGridHeight
    {
        get { return currentGridHeight; }
        set { currentGridHeight = value; }
    }

    [Header("Grid Configuration")]
    [SerializeField] private int Rows;
    [SerializeField] private int Cols;
    [SerializeField] private Vector2 CellPadding;

    public void InitializeGrid(int rows, int columns, Vector2 cellPadding, TilePack pack, List<List<string>> level)
    {
        rectTransform = GetComponent<RectTransform>();
        Rows = rows;
        Cols = columns;
        CellPadding = cellPadding;

        if (cellPrefab == null)
        {
            Debug.LogError("Cell Prefab is not assigned!");
            return;
        }

        if (rectTransform == null)
        {
            Debug.LogError("RectTransform is not found!");
            return;
        }
        Debug.Log(rows + " " + columns + " ");
        CreateGrid(rows, columns, cellPadding, pack, level);
        //CanvasManager.instance.AllignWithResolution(gridLayoutGroup.cellSize.x, columns);
    }

    void CreateGrid(int rows, int columns, Vector2 cellPadding, TilePack pack, List<List<string>> level)
    {
        if (rectTransform == null) return;

        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;
        gridLayoutGroup.spacing = cellPadding;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        
        float smallestDimension = Mathf.Min(parentWidth / columns, parentHeight / rows);
        float cellSize = smallestDimension - Mathf.Min(cellPadding.x, cellPadding.y);

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);

        int combinationIndex = 0;
        for (int row = 1; row < rows + 1; row++)
        {
            for (int col = 1; col < columns + 1; col++)
            {
                GameObject newCell = Instantiate(cellPrefab, transform);
                newCell.name = "Cell_" + row + "x" + col;
                RectTransform cellRectTransform = newCell.GetComponent<RectTransform>();

                if (cellRectTransform == null)
                {
                    Debug.LogError("Cell Prefab does not have RectTransform component!");
                    continue;
                }

                // 1:1 Aspect Ratio
                cellRectTransform.sizeDelta = new Vector2(cellSize, cellSize);

                int gridIndex = (row - 1) * columns + (col - 1); 
                int rowFlat = gridIndex / columns;
                int colFlat = gridIndex % columns;
                LayerDistributor GridCell = newCell.GetComponent<LayerDistributor>();
                
                GridCell.DistributeLayers(pack, pack.GetSpritePackFromLayerConfig(level, gridIndex), pack.BackgroundSprites[(rowFlat + colFlat) % 2]);
                combinationIndex++;

               if(!gridCells.Contains(GridCell)) gridCells.Add(GridCell);
            }
        }

        CurrentGridWidth = gridLayoutGroup.cellSize.x * columns;
        CurrentGridHeight = gridLayoutGroup.cellSize.y * rows;
        OnGridGenerated?.Invoke(CurrentGridWidth, CurrentGridHeight);
    }

    public bool CheckAllCellsAreEmpty()
    {
        foreach (LayerDistributor cell in gridCells)
        {
            if (cell.GetCellLayerCount() != 0)
            {
                return false; 
            }
        } 

        Debug.Log("AllCellsEmpty!!!!!");
        AllCellsEmptyEvent?.Invoke();
        return true; 
    }
    public void ClearLevel(bool isTutorial, bool tutorialCompleteFlag)
    {
        StartCoroutine(LevelEndingRoutine(isTutorial, tutorialCompleteFlag));
    }
    private IEnumerator LevelEndingRoutine(bool isTutorial, bool tutorialCompleteFlag)
    {
        // Eğer isTutorial true ise, tutorialCompleteFlag'in true olmasını bekle
        if (isTutorial)
        {
            yield return new WaitUntil(() => tutorialCompleteFlag == true);
        }

        // Hücre küçültme sekansını oluştur
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < gridCells.Count; i++)
        {
            LayerDistributor cell = gridCells[i];
            // Hücre küçülme animasyonunu sıraya ekle
            sequence.Insert(i * 0.1f, cell.transform.DOScale(Vector3.zero, 0.5f));
        }

        // Tüm animasyonlar tamamlandığında EndGame'i çağır
        sequence.OnComplete(() =>
        {
            GameManager.instance.EndGame(true);
        });
    }

    void OnRectTransformDimensionsChange()
    {
        Debug.Log("OnRectTransformDimensionsChange called!");
        if (rectTransform == null || cellPrefab == null) return;
    }
}



using UnityEngine;

public class GridController : MonoBehaviour
{

    #region Variables

    const int _MIN_CELL_SIZE = 40;
    const int _MAX_CELL_SIZE = 70;

    [SerializeField] GameObject _colPrefab;
    [SerializeField] GameObject _cellPrefab;

    Transform[,] _gridPositions;
    int _cellSize;

    #endregion


    #region Properties
    public Transform[,] GridPositions => _gridPositions;
    public int CellSize => _cellSize;

    #endregion


    #region Methods

    /// <summary>
    /// Given the number of Rows and Cells, generates a Grid of cells
    /// </summary>
    public void GenerateGrid(int numOfCols, int numOfRows)
    {
        _gridPositions = new Transform[numOfCols, numOfRows];

        _cellSize = GetCellSize(numOfCols);

        for(int col = 0; col < numOfCols; col++)
        {
            /// Generate Grid Column
            GameObject colParent = Instantiate(_colPrefab, this.transform);

            for(int row = 0; row < numOfRows; row++)
            {
                /// Generate Grid Cell
                GameObject cell = Instantiate(_cellPrefab, colParent.transform);
                cell.GetComponent<RectTransform>().sizeDelta = new Vector2(_cellSize,_cellSize);

                /// Get cell Position 
                _gridPositions[col, row] = cell.transform;
            }
        }
    }


    /// <summary>
    /// Returns the size of the square cells
    /// </summary>
    private int GetCellSize(int numOfCols)
    {        
        float rectTransformWidth = GetComponent<RectTransform>().rect.width;
        /// As min size is used in 20 columns, get the desired size for the given amount of cols. Clamp it between Min and Max size
        return (int) Mathf.Clamp(((rectTransformWidth / numOfCols) * _MIN_CELL_SIZE / (rectTransformWidth / 20)), _MIN_CELL_SIZE, _MAX_CELL_SIZE);        
    }

    #endregion
    
}

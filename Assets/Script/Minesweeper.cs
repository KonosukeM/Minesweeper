using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class Minesweeper : MonoBehaviour, IPointerClickHandler
{
    // 行
    [SerializeField]
    private int _rows = 1;

    // 列
    [SerializeField]
    private int _columns = 1;

    // 地雷の数
    [SerializeField]
    private int _mineCount = 1;

    // 行列展開数
    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup = null;

    // セルプレハブ
    [SerializeField]
    private Cell _cellPrefab = null;

    // 経過時間テキスト
    [SerializeField]
    private TextMeshProUGUI _timeText = null;

    // ゲームクリアテキスト
    [SerializeField]
    private TextMeshProUGUI _clearText = null;

    // ゲームオーバーテキスト
    [SerializeField]
    private TextMeshProUGUI _overText = null;

    // 2次元配列用のセル
    private Cell[,] _cells;

    // 初手地雷防止用
    private bool _firstClick = false;

    // ゲームクリアフラグ
    private bool _gameClear = false;

    // ゲームオーバーフラグ
    private bool _gameOver = false;

    void Start()
    {
        // 展開機能
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = _columns;

        // parentにプレハブセルの動きを持たせる/プレハブからセルを生成する
        var parent = _gridLayoutGroup.gameObject.transform;
        _cells = CreateCells(_rows, _columns, parent);

        // 地雷を設置する
        InitializeCells(_mineCount);

        // テキストの非表示
        _clearText.gameObject.SetActive(false);
        _overText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // ゲームクリアの処理
        if(_gameClear) 
        { 
            _clearText.gameObject.SetActive(true);
            return;
        }

        // ゲームオーバーの処理
        if(_gameOver) 
        { 
            _overText.gameObject.SetActive(true);
            return;
        }

    }

    // セルを初期化
    private void InitializeCells(int mineCount)
    {
        // 設置する地雷が配列の長さを超えていた時の処理
        if (mineCount >= _cells.Length)
        {
            // 地雷数が間違っていた時のデバックログ
            Debug.LogError($"地雷数はセル数より少なく設定してください\n" +
                $"地雷数={mineCount}, セル数={_cells.Length}");
            return;
        }

        // すべてのセルの状態を None にする。
        ClearCells();

        // 地雷をランダムに配置する処理
        for (var i = 0; i < mineCount; i++)
        {
            var r = Random.Range(0, _rows);
            var c = Random.Range(0, _columns);
            var cell = _cells[r, c];

            // ランダムに選ばれたセルが地雷かどうか
            if (cell.CellState == CellState.Mine)
            {
                Debug.Log("重複したので再抽選");
                i--;
            }
            else { cell.CellState = CellState.Mine; }
        }

        // 地雷の数を数える
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                var cell = _cells[r, c];
                cell.CellState = GetMineCount(r, c);
            }
        }
    }

    // すべてのセルの状態を None にする。
    private void ClearCells()
    {
        foreach (var cell in _cells)
        {
            cell.CellState = CellState.None;
        }
    }

    // プレハブセルを親オブジェクトに依存
    private Cell[,] CreateCells(int rows, int columns, Transform parent)
    {
        var cells = new Cell[_rows, _columns];
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < columns; c++)
            {
                var cell = Instantiate(_cellPrefab);
                cell.transform.SetParent(parent);
                cells[r, c] = cell;
            }
        }
        return cells;
    }

    // 周囲8近傍のセルの地雷数を数える処理
    private CellState GetMineCount(int row, int column)
    {
        var cell = _cells[row, column];
        if (cell.CellState == CellState.Mine) { return CellState.Mine; }

        // 周囲のセルの地雷の数を数える
        var count = 0;

        var up = row - 1;
        var down = row + 1;
        var left = column - 1;
        var right = column + 1;

        if (IsMine(up, left)) { count++; } // 左上
        if (IsMine(up, column)) { count++; } // 上
        if (IsMine(up, right)) { count++; } // 右上
        if (IsMine(row, left)) { count++; } // 左
        if (IsMine(row, right)) { count++; } // 右
        if (IsMine(down, left)) { count++; } // 左下
        if (IsMine(down, column)) { count++; } // 下
        if (IsMine(down, right)) { count++; } // 右下

        return (CellState)count;
    }

    // 指定の行番号と列番号のセルが地雷かどうかを判定するメソッド
    private bool IsMine(int row, int column)
    {
        // 指定の行番号と列番号のセルが地雷かどうかを返す。存在しない行番号・列番号なら常に false を返す。
        var r = _cells.GetLength(0);
        var c = _cells.GetLength(1);
        if (row < 0 || row >= r || column < 0 || column >= c) { return false; }
        if (_cells[row, column].CellState == CellState.Mine) { return true; }
        else
        {
            return false;
        }
    }

    // クリック検知
    public void OnPointerClick(PointerEventData eventData)
    {
        // 選択したセル
        var cell = eventData.pointerCurrentRaycast.gameObject.GetComponent<Cell>();

        if (!cell) { return; } // セルのみにしか反応しない

        if (TryGetCellIndex(cell, out int row, out int column)) {
            // 左クリックを押したとき
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _cells[row, column].Cellopenjudg();
                // ゲームオーバー判定
                if (cell.CellState == CellState.Mine) { _gameOver = true; }
            }

            // 右クリックを押したとき
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                _cells[row, column].Cellclosejudg();
            } 
        }
    }

    // 選択したセルの配列番号を返す
    private bool TryGetCellIndex(Cell cell, out int row, out int column)
    {
        for (var r = 0; r < _cells.GetLength(0); r++)
        {
            for (var c = 0; c < _cells.GetLength(1); c++)
            {
                if (_cells[r, c] == cell)
                {
                    row = r;
                    column = c;
                    return true;
                }
            }
        }

        row = column = -1;
        return false;
    }
}
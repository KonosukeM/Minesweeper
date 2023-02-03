using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CellState
{
    None = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,

    Mine = -1,
}

public class Cell : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _view = null;

    [SerializeField]
    private CellState _cellState = CellState.None;

    [SerializeField]
    public GameObject _cellcover = null;

    [SerializeField]
    public GameObject _mine = null;

    public CellState CellState
    {
        get => _cellState;
        set
        {
            _cellState = value;
            OnCellStateChanged();
        }
    }

    private void OnValidate()
    {
        OnCellStateChanged();
    }

    private void Update()
    {
        MouseClick();
    }

    private void OnCellStateChanged()
    {
        if (_view == null) { return; }

        if (_cellState == CellState.None)
        {
            _view.text = "";
        }
        else if (_cellState == CellState.Mine)
        {
            _view.text = "X";
            _view.color = Color.red;
        }
        else
        {
            _view.text = ((int)_cellState).ToString();
            _view.color = Color.blue;
        }
    }

    public void MouseClick()
    {
        // 左クリックを押したとき
        if(Input.GetMouseButtonDown(0)) 
        {
            _cellcover.SetActive(false);
        }

        // 右クリックを押したとき
        if(Input.GetMouseButtonDown(1)) 
        {
            _mine.SetActive(true);
        }
    }
}
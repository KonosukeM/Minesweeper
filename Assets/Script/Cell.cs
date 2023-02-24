using System;
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

    Boolean change = false;

    public CellState CellState
    {
        get => _cellState; // �l���擾����
        set // �l��ݒ肷�� set value �̓Z�b�g
        {
            _cellState = value;
            OnCellStateChanged();
        }
    }

    private void OnValidate()
    {
        OnCellStateChanged();
    }

    // �J�����Z���̎�ނ�������Ԃ��֐�
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

    // ���N���b�N�ŌĂ΂��֐�
    public void Cellopenjudg() 
    {
        _cellcover.SetActive(false);
    }

    // �E�N���b�N�ŌĂ΂��֐��i���j
    public void Cellclosejudg() 
    {
        if (!change)
        {
            _mine.SetActive(true);
            change = true;
        } 
        else if (change)
        { 
            _mine.SetActive(false);
            change = false;
        }
    }
}
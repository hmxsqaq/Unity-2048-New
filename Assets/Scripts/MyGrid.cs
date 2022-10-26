using System;
using UnityEngine;

public class MyGrid
{
    private int _numberindex;//实际数字对应Prefab的Index
    private int _number;//实际数字
    private int _row;//行号
    private int _column;//列号
    private int _index;//0-15位置Index
    private bool _isMerge = false;//是否合并过
    private GameObject _gameObject = null;//代表的GameObject

    public MyGrid(int number,int row,int column)
    {
        this._number = number;
        this._row = row;
        this._column = column;
        IndexSet();
        NumberIndexSet();
    }

    public int NumberIndex
    {
        get => _numberindex;
    }
    public int Number
    {
        get => _number;
        set
        {
            _number = value;
            NumberIndexSet();
        } 
    }
    
    public int Row
    {
        get => _row;
        set
        {
            _row = value;
            IndexSet();
        }
    }
    public int Column
    {
        get => _column;
        set 
        {
            _column = value;
            IndexSet();
        }
    }
    public int Index
    {
        get => _index;
    }
    
    public bool IsMerge
    {
        get => _isMerge;
        set => _isMerge = value;
    }

    public GameObject GameObject
    {
        get => _gameObject;
        set => _gameObject = value;
    }

    private void IndexSet()
    {
        _index = _row * 4 + _column;
    }
    private void NumberIndexSet()
    {
        _numberindex = (int)Math.Log(_number, 2) - 1;
    }
}

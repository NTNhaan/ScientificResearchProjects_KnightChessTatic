using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// check tất cả các image mà giống nhau
// lưu trữ bằng gamepieces thay vì gameobject
public class GamePieces : MonoBehaviour
{
    // -----------Properties
    private int _x;
    private int _y;
    private Grid.PieceType _type;
    private Grid _grid;
    private MovablePiece movableComponent;
    private ItemPieces itemComponent;
    //-----------Constructor
    public int X
    {
        get { return _x; }
        set
        {
            if (IsMoveable())
            {
                _x = value;
            }
        }
    }
    public int Y
    {
        get { return _y; }
        set
        {
            if (IsMoveable())
            {
                _y = value;
            }
        }
    }
    public Grid.PieceType Type
    {
        get { return _type; }
    }
    public Grid GridRef
    {
        get { return _grid; }
    }
    public MovablePiece MovableComponent
    {
        get { return movableComponent; }
    }
    public ItemPieces ItemComponent
    {
        get { return itemComponent; }
    }
    private ClearablePiece clearablePiece;
    public ClearablePiece ClearableComponent
    {
        get { return clearablePiece; }
    }

    private void Awake()
    {
        movableComponent = GetComponent<MovablePiece>();
        itemComponent = GetComponent<ItemPieces>();
        clearablePiece = GetComponent<ClearablePiece>();
    }
    public void Init(int x, int y, Grid grid, Grid.PieceType type)
    {
        _x = x;
        _y = y;
        _grid = grid;
        _type = type;
    }
    private void OnMouseEnter() => _grid.EnterPiece(this);
    private void OnMouseDown() => _grid.PressPiece(this);
    private void OnMouseUp() => _grid.ReleasePiece();
    public bool IsMoveable()
    {
        return movableComponent != null;
    }
    public bool IsItemed()
    {
        return itemComponent != null;
    }
    public bool IsClearable()
    {
        return ClearableComponent != null;
    }
}
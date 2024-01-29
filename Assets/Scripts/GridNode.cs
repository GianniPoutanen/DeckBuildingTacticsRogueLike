
using Unity.VisualScripting;
using UnityEngine;

public class GridNode : IHeapItem<GridNode>
{
    private Vector3Int _pos;
    public Vector3Int Position
    {
        get
        {
            return new Vector3Int(_pos.x, _pos.y, 0);
        }
        set
        {
            _pos = value;
        }
    }

    public int GCost;
    public int HCost;
    public GridNode Parent;

    public int FCost { get { return GCost + HCost; } }

    public GridNode(Vector3Int position)
    {
        Position = position;
    }

    public int HeapIndex { get; set; }

    public int CompareTo(GridNode other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
            compare = HCost.CompareTo(other.HCost);
        return compare;
    }

    public override bool Equals(object obj)
    {
        return (obj as GridNode).Position.Equals(this.Position);
    }

    public override int GetHashCode()
    {
        return (Position.x * 7) + (Position.y * 11);
    }

}
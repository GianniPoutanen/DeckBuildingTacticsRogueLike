
using Unity.VisualScripting;
using UnityEngine;

public class GridNode : IHeapItem<GridNode>
{
    public Vector3Int Position;
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
        return -compare;
    }

    public override bool Equals(object obj)
    {
        if (obj is GridNode)
        {
            return this.Position.Equals((obj as GridNode).Position);
        }
        return false;
    }
}
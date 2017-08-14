using UnityEngine;
using System.Collections.Generic;

public class NodeOccupant
{
    public List<Node> path;
    public Node owner;
    public GameObject graphic;
    public bool isPathingDirty;
    private bool _doesBlockPathing = true;
    public bool doesBlockPathing
    {
        get { return _doesBlockPathing; }
        set
        {
            _doesBlockPathing = value;
            isPathingDirty = true;
            if (graphic != null && graphic.GetComponentInChildren<SpriteRenderer>())
            {
                graphic.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, _doesBlockPathing ? 1.0f : 0.5f);//this should be handled elsewhere
            }
        }
    }
    private Node _target;
    public Node target
    {
        get { return _target; }
        set
        {
            _target = value;
            isPathingDirty = true;
        }
    }

    public Node GetNextNode()
    {
        if (path == null || path.Count < 1)
        {
            return null;
        }
        return path[0];
    }
}


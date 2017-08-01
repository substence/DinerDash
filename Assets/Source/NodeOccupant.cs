using UnityEngine;
using System.Collections.Generic;

public class NodeOccupant
{
    public List<Node> path;
    public Node owner;
    public GameObject graphic;

    public Node GetNextNode()
    {
        if (path == null  || path.Count < 1)
        {
            return null;
        }
        return path[0];
    }
}


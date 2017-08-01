using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public GameObject graphic;
    public NodeOccupant occupant;
    public bool isPathable = true;
    public int x;
    public int y;
    public List<Node> neighbors;
    public NodeOccupant GetFirstOccupant() {return occupant; }//getter since occupant could potentially be a list
}


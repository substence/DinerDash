using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NodeMap
{
    public GameObject defaultNodeGraphic;
    public GameObject defaultUnpathableGraphic;
    public delegate void EventHandler(int x, int y);
    public event EventHandler OnNodeClicked;
    private int width;
    private int height;
    private int size;
    private Node[,] nodes;
    private Hashtable gameObjectsToNodes;

    public void Initialize(int width, int height, int size)
    {
        if (nodes != null)
        {
            ClearMap();
        }

        this.width = width;
        this.height = height;
        this.size = size;

        nodes = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodes[x, y] = new Node();
            }
        }

        gameObjectsToNodes = new Hashtable();
    }

    public void Build()
    {
        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
                Node node = GetNodeAt(x, y);
                if (node == null)
                {
                    continue;
                }
                GameObject graphicType = node.isPathable ? defaultNodeGraphic : defaultUnpathableGraphic;
                GameObject gameObject = (GameObject)GameObject.Instantiate(graphicType, new Vector3(x, y, 0), Quaternion.identity);
                EventDispatcher eventDispatcher = gameObject.AddComponent<EventDispatcher>();
                gameObjectsToNodes.Add(gameObject, node);
                node.x = x;
                node.y = y;
                if (eventDispatcher)
                {
                    eventDispatcher.MouseDown += OnNodeMouseDown;
                }
            }
        }
    }

    //shortcut helper method, so consumers don't need to know about Nodes or inner workings
    public void SetPathingAt(int x, int y, bool isPathable)
    {
        Node node = GetNodeAt(x, y);
        if (node != null)
        {
            node.isPathable = isPathable;
        }
    }

    //shortcut helper method, so consumers don't need to know about Nodes or inner workings
    public void SetOccupantAt(int x, int y, GameObject occupant)
    {
        Node node = GetNodeAt(x, y);
        if (node != null && node.isPathable)
        {
            node.occupant = occupant;
            PlaceOccupantInNode(occupant, node);
        }
    }

    public void MoveOccupantToNodeAt(int x, int y, GameObject occupant)
    {
        Node node = GetNodeAt(x, y);
        if (node != null && node.isPathable)
        {
            node.occupant = occupant;
            PlaceOccupantInNode(occupant, node);
        }
    }

    private void PlaceOccupantInNode(GameObject occupant, Node node)
    {
        //occupant.transform.position = ;
        occupant.transform.position = new Vector3(node.x, node.y, 0);
        //occupant.transform.position.Set(node.x * size, node.y * size, 0);
    }

    private void ClearMap()
    {
        //todo
    }

    void OnNodeMouseDown(GameObject gameObject)
    {
        if (OnNodeClicked != null)
        {
            Node node = GetNodeFromGameObject(gameObject);
            if (node != null)
            {
                OnNodeClicked(node.x, node.y);
            }
        }
    }

    void MoveOccupantTo(Node from, Node to)
    {
        GameObject occupant = from.occupant;
        from.occupant = null;
        to.occupant = occupant;
    }

    Node GetNodeAt(int x, int y)
    {
        return nodes[x, y];
    }

    Node GetNodeFromGameObject(GameObject gameObject)
    {
        /*NodeComponent nodeComponent = gameObject.GetComponent<NodeComponent>();
        if (nodeComponent)
        {
            return nodeComponent.node;
        }
        return null;*/
        return (Node)gameObjectsToNodes[gameObject];
    }
}
public class Node
{
    public GameObject graphic;
    public GameObject occupant;
    public bool isPathable = true;
    public int x;
    public int y;
}

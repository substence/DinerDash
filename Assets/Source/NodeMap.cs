using UnityEngine;
using System.Collections.Generic;

public class NodeMap
{
    public GameObject defaultNodeGraphic;
    public GameObject defaultUnpathableGraphic;
    public delegate void EventHandler(int x, int y);
    public event EventHandler OnNodeClicked;
    public int width { get; private set; }
    public int height { get; private set; }
    private int size;
    private Node[,] nodes;
    private Dictionary<GameObject, Node> gameObjectsToNodes;

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
                nodes[x, y].neighbors = new List<Node>();
            }
        }

        gameObjectsToNodes =  new Dictionary<GameObject, Node>();

        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
                /*//4-way, can't travel diagonal
                if (x > 0)
                    nodes[x, y].neighbors.Add(nodes[x - 1, y]);
                if (x < width - 1)
                    nodes[x, y].neighbors.Add(nodes[x + 1, y]);
                if (y > 0)
                    nodes[x, y].neighbors.Add(nodes[x, y - 1]);
                if (y < height - 1)
                    nodes[x, y].neighbors.Add(nodes[x, y + 1]);*/

                //8-way
                if (x > 0)
                {
                    nodes[x, y].neighbors.Add(nodes[x - 1, y]);
                    if (y > 0)
                        nodes[x, y].neighbors.Add(nodes[x - 1, y - 1]);
                    if (y < height - 1)
                        nodes[x, y].neighbors.Add(nodes[x - 1, y + 1]);
                }
                if (x < width - 1)
                {
                    nodes[x, y].neighbors.Add(nodes[x + 1, y]);
                    if (y > 0)
                        nodes[x, y].neighbors.Add(nodes[x + 1, y - 1]);
                    if (y < height - 1)
                        nodes[x, y].neighbors.Add(nodes[x + 1, y + 1]);
                }
                if (y > 0)
                    nodes[x, y].neighbors.Add(nodes[x, y - 1]);
                if (y < height - 1)
                    nodes[x, y].neighbors.Add(nodes[x, y + 1]);
            }
        }
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
        foreach (Node node in nodes)
        {
            ValidateNode(node);
        }        
    }

    public void Update()
    {
        foreach (Node node in nodes)
        {
            NodeOccupant occupant = node.GetFirstOccupant();
            if (occupant != null)
            {
                UpdateOccupant(node, occupant);
            }
        }
    }

    //This probably shouldn't do any rendering changes, perhaps NodeRenderer?
    private void UpdateOccupant(Node node, NodeOccupant occupant)
    {
        //Calculate path to target
        Node targetNode = occupant.target;
        if (targetNode != null && occupant.isPathingDirty)
        {
            occupant.path = NodePathing.GetPath(nodes, node, targetNode, occupant);
            occupant.isPathingDirty = false;//perhaps the setter for 'path' should do this automatically?
        }

        //Move towards next node
        Node nextNode = occupant.GetNextNode();
        if (nextNode != null)
        {
            Vector3 currentPosition = occupant.graphic.transform.position;
            Vector3 targetPosition = GetWorldPositionOfNode(nextNode);

            occupant.graphic.transform.position = Vector3.Lerp(currentPosition, targetPosition, 5f * Time.deltaTime);

            //if close enough to target node, update path
            if (Vector3.Distance(currentPosition, targetPosition) < 0.1f)
            {
                occupant.path.RemoveAt(0);
                node.occupant = null;
                SetNodeOccupant(nextNode, occupant);
            }
        }
        else
        {
            occupant.graphic.transform.position = GetWorldPositionOfNode(node);
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
    public void SetNodeOccupantAt(int x, int y, NodeOccupant occupant)
    {
        Node node = GetNodeAt(x, y);
        if (node != null && node.isPathable)
        {
            SetNodeOccupant(node, occupant);
        }
    }

    //this doesn't clear occupants, so they could end up in an unpathable node
    public void ClearMap()
    {
        foreach (Node node in nodes)
        {
            node.isPathable = true;
            NodeOccupant occupant = node.GetFirstOccupant();
            if (occupant != null)
            {
                occupant.path = null;
            }
        }

        foreach (GameObject gameObject in gameObjectsToNodes.Keys)
        {
            GameObject.Destroy(gameObject);
        }

        gameObjectsToNodes = new Dictionary<GameObject, Node>();
    }

    public Node GetNodeAt(int x, int y)
    {
        return nodes[x, y];
    }

    private void ValidateNode(Node node)
    {
        NodeOccupant occupant = node.GetFirstOccupant();
        if (!node.isPathable && occupant != null)
        {
            Node validNode = NodePathing.FindClosestPathableNode(this, node);
            if (validNode != null)
            {
                node.occupant = null;
                SetNodeOccupant(validNode, occupant);
            }
        }
    }

    private void SetNodeOccupant(Node node, NodeOccupant occupant)
    {
        node.occupant = occupant;
        occupant.owner = node;
    }

    private void OnNodeMouseDown(GameObject gameObject)
    {
        if (OnNodeClicked != null)
        {
            Node clickedNode = GetNodeFromGameObject(gameObject);
            if (clickedNode != null)
            {
                OnNodeClicked(clickedNode.x, clickedNode.y);
            }
        }
    }

    private Node GetNodeFromGameObject(GameObject gameObject)
    {
        return (Node)gameObjectsToNodes[gameObject];
    }

    private Vector3 GetWorldPositionOfNode(Node node)
    {
        return new Vector3(node.x, node.y, 0.0f);
    }

    private Node GetNodeFromOccupant(NodeOccupant occupant)
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
                if (node.GetFirstOccupant() == occupant)
                {
                    return node;
                }
            }
        }
        return null;
    }
}

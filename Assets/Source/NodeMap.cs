using UnityEngine;
using System.Collections.Generic;

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
                nodes[x, y].neighbors = new List<Node>();//optimize

            }
        }

        gameObjectsToNodes =  new Dictionary<GameObject, Node>();

        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
                if (x > 0)
                    nodes[x, y].neighbors.Add(nodes[x - 1, y]);
                if (x < width - 1)
                    nodes[x, y].neighbors.Add(nodes[x + 1, y]);
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
        occupant.transform.position = new Vector3(node.x, node.y, 0);
    }

    private void ClearMap()
    {
        //todo
        //nodes = new Node[width, height];
        //gameObjectsToNodes = new Dictionary<GameObject, Node>();
    }

    void OnNodeMouseDown(GameObject gameObject)
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

    public void DebugPath(int fromX, int fromY, int toX, int toY)
    {
        Node from = GetNodeAt(fromX, fromY);
        Node to = GetNodeAt(toX, toY);
        List<Node> path = NodePathing.GetPath(nodes, from, to);
        for (int i = 0; i < path.Count-1; i++)
        {
            /*Vector3 start = new Vector3(path[i].x * 100, path[i].y * 100, -0.5f);//node position to world position
            Vector3 end = new Vector3(path[i+1].x * 100, path[i+1].y * 100, -0.5f);*/
            Vector3 start = new Vector3(0, 0, -0.5f);//node position to world position
            Vector3 end = new Vector3(1, 1, -0.5f);
            Debug.DrawLine(start, end, Color.red, 10.0f);
        }
    }

    private Node GetNodeAt(int x, int y)
    {
        return nodes[x, y];
    }

    public Node GetNodeFromGameObject(GameObject gameObject)//shouldn't be public?
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
    public List<Node> neighbors;
}

public class NodePathing
{
    //Dijkstra
    public static List<Node> GetPath(Node[,] map, Node from, Node to)
    {
        Dictionary<Node, float> distance = new Dictionary<Node, float>();
        Dictionary<Node, Node> previousNode = new Dictionary<Node, Node>();
        List<Node> uncheckedNodes = new List<Node>();//Q

        //initialize default values
        distance[from] = 0;
        previousNode[from] = null;
        foreach (Node node in map)
        {
            if (node != from)
            {
                distance[node] = Mathf.Infinity;
                previousNode[node] = null;
            }
            uncheckedNodes.Add(node);
        }

        //loop until all nodes are checked
        while (uncheckedNodes.Count > 0)
        {
            Node closestNode = null;

            foreach (Node potentialClosestNode in uncheckedNodes)
            {
                if (closestNode == null || distance[potentialClosestNode] < distance[closestNode])
                {
                    closestNode = potentialClosestNode;
                }
            }

            if (closestNode == to)
            {
                break;
            }

            uncheckedNodes.Remove(closestNode);

            foreach (Node node in closestNode.neighbors)
            {
                float alt = distance[closestNode] + Vector2.Distance(new Vector2(closestNode.x, closestNode.y), new Vector2(node.x, node.y));

                if (alt < distance[node])
                {
                    distance[node] = alt;
                    previousNode[node] = closestNode;
                }
            }
        }

        if (previousNode[to] == null)
        {
            return null;
            //no route found, do something
        }

        List<Node> path = new List<Node>();

        while (to != null)
        {
            path.Add(to);
            to = previousNode[to];
        }

        //currentPath.Reverse();

        return path;
    }
}

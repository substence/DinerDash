using UnityEngine;
using System.Collections.Generic;

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
                float alt = distance[closestNode] + GetPathingCost(closestNode, node);

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

        path.Reverse();

        return path;
    }

    private static float GetPathingCost(Node from, Node to)
    {
        float cost = 1;//eventually node types will have a different pathing cost by default

        if (!to.isPathable)
            return Mathf.Infinity;

        if (from.x != to.x && from.y != to.y)
        {
            // nudge diagonal movement to break ties
            cost += 0.001f;
        }

        return cost;
    }

    public static Node FindClosestPathableNode(NodeMap map, Node from)
    {
        for (int x = from.x; x < map.width; x++)
        {
            for (int y = from.y; y < map.height; y++)
            {
                Node node = map.GetNodeAt(x, y);
                if (node.isPathable)
                {
                    return node;
                }
            }
        }
        return null;
    }
}



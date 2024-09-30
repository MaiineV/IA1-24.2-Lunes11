using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    public LayerMask nodeMask;

    private Node fromNode;
    private Node toNode;

    private HashSet<Node> closeNodes;
    private PriorityQueue<Node> openNodes;

    private void Awake()
    {
        Instance = this;
    }

    //A*
    public List<Node> GetPath(Vector3 from, Vector3 to)
    {
        var fromColliderArray = Physics.OverlapSphere(from, 1.5f, nodeMask);
        if (fromColliderArray.Length > 0)
        {
            fromNode = fromColliderArray[0].GetComponent<Node>();
        }

        var toColliderArray = Physics.OverlapSphere(to, 1.5f, nodeMask);
        if (toColliderArray.Length > 0)
        {
            toNode = toColliderArray[0].GetComponent<Node>();
        }

        closeNodes = new HashSet<Node>();
        openNodes = new PriorityQueue<Node>();

        var actualNode = fromNode;
        actualNode.Weight = 0;

        var watchdog = 100000;

        while (actualNode != null && actualNode != toNode && watchdog > 0)
        {
            watchdog--;

            foreach (var node in actualNode.neighbours)
            {
                if (closeNodes.Contains(node)) continue;

                var heuristic = actualNode.Weight + 1 +
                    Vector3.Distance(node.transform.position, toNode.transform.position);

                if (node.Weight > heuristic)
                {
                    node.Weight = heuristic;
                    node.previous = actualNode;
                }

                if (!openNodes.Contains(node)) openNodes.Enqueue(node);
            }

            closeNodes.Add(actualNode);

            actualNode = openNodes.Dequeue();
        }

        var finalPath = new List<Node>();
        actualNode = toNode;
        while (actualNode != fromNode && actualNode.previous != null)
        {
            finalPath.Add(actualNode);
            actualNode = actualNode.previous;
        }

        finalPath.Reverse();
        return finalPath;
    }
}

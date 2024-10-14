using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//A*
//Theta*
public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    public LayerMask nodeMask;

    private Node fromNode;
    private Node toNode;

    private HashSet<Node> closeNodes;
    private PriorityQueue<Node> openNodes;

    private struct PathRequestData
    {
        public Vector3 fromPoint;
        public Vector3 toPoint;
        public Action<List<Node>> callbackPath;
    }

    private Queue<PathRequestData> queuePath = new Queue<PathRequestData>();
    private bool isCalculating = false;

    private Action OnResetNodes = delegate { };

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!(queuePath.Count > 0) || isCalculating) return;

        OnResetNodes();
        OnResetNodes = delegate() { };
        isCalculating = true;
        var actualData = queuePath.Dequeue();
        StartCoroutine(Path(actualData.fromPoint, actualData.toPoint, actualData.callbackPath));
    }

    public void RequestPath(Vector3 from, Vector3 to, Action<List<Node>> callback)
    {
        queuePath.Enqueue(new PathRequestData { fromPoint = from, toPoint = to, callbackPath = callback });
    }

    private IEnumerator Path(Vector3 from, Vector3 to, Action<List<Node>> callback)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var actualRadious = 1.5f;
        var fromColliderArray = Physics.OverlapSphere(from, actualRadious, nodeMask);

        while (fromColliderArray.Length <= 0)
        {
            actualRadious *= 2;
            fromColliderArray = Physics.OverlapSphere(from, actualRadious, nodeMask);
        }

        if (fromColliderArray.Length > 0)
        {
            fromNode = GetClosestNode(fromColliderArray, from);
        }

        actualRadious = 1.5f;
        var toColliderArray = Physics.OverlapSphere(to, actualRadious, nodeMask);

        while (toColliderArray.Length <= 0)
        {
            actualRadious *= 2;
            toColliderArray = Physics.OverlapSphere(to, actualRadious, nodeMask);
        }

        if (toColliderArray.Length > 0)
        {
            toNode = GetClosestNode(toColliderArray, to);
        }

        closeNodes = new HashSet<Node>();
        openNodes = new PriorityQueue<Node>();

        var actualNode = fromNode;
        actualNode.Weight = 0;

        var watchdog = 100000;

        var counter = 0;

        while (actualNode != null && actualNode != toNode && watchdog > 0)
        {
            OnResetNodes += actualNode.OnResetWeight;
            watchdog--;

            foreach (var node in actualNode.neighbours)
            {
                if (closeNodes.Contains(node)) continue;
                OnResetNodes += node.OnResetWeight;
                var heuristic = actualNode.Weight +
                    Vector3.Distance(node.transform.position, actualNode.transform.position) +
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

            if (counter > 5000)
            {
                yield return null;
                counter = 0;
            }

            counter++;
        }

        #region Theta en funcion aparte
        //var finalPath = ThetaStar();
        #endregion

        #region Theta en A*
        var finalPath = new List<Node>();
        actualNode = toNode;
        var seeingNode = toNode.previous;
        finalPath.Add(actualNode);

        watchdog = 10000;
        counter = 0;

        while (seeingNode != null && seeingNode.previous != null && watchdog > 0 &&
            actualNode != fromNode && actualNode.previous != null)
        {
            watchdog--;
            var dir = seeingNode.previous.transform.position -
                actualNode.transform.position;

            if (!Physics.Raycast(actualNode.transform.position, dir, dir.magnitude, LayerMask.GetMask("Wall")))
            {
                seeingNode = seeingNode.previous;
            }
            else
            {
                finalPath.Add(seeingNode);
                actualNode = seeingNode;
                seeingNode = seeingNode.previous;
            }

            counter++;
            if (counter > 1000)
            {
                yield return null;
                counter = 0;
            }
        }

        if (!finalPath.Contains(fromNode))
            finalPath.Add(fromNode);
        #endregion

        finalPath.Reverse();
        stopWatch.Stop();
        UnityEngine.Debug.Log(stopWatch.Elapsed);
        UnityEngine.Debug.Log(stopWatch.ElapsedTicks);
        callback(finalPath);
        isCalculating = false;
    }

    public List<Node> ThetaStar()
    {
        var path = new List<Node>();
        var actualNode = toNode;
        var seeingNode = toNode.previous;
        path.Add(actualNode);

        var watchdog = 10000;

        while (seeingNode != null && seeingNode.previous != null && watchdog > 0 &&
            actualNode != fromNode && actualNode.previous != null)
        {
            watchdog--;
            var dir = seeingNode.previous.transform.position -
                actualNode.transform.position;

            if (!Physics.Raycast(actualNode.transform.position, dir, dir.magnitude, LayerMask.GetMask("Wall")))
            {
                seeingNode = seeingNode.previous;
            }
            else
            {
                path.Add(seeingNode);
                actualNode = seeingNode;
                seeingNode = seeingNode.previous;
            }
        }

        if (!path.Contains(fromNode))
            path.Add(fromNode);

        return path;
    }

    public Node GetClosestNode(Collider[] colliders, Vector3 point)
    {
        Collider closest = colliders[0];
        float minDist = Vector3.Distance(colliders[0].transform.position, point);

        for (int i = 1; i < colliders.Length; i++)
        {
            var acutalDistance = Vector3.Distance(colliders[i].transform.position, point);
            if (acutalDistance < minDist)
            {
                minDist = acutalDistance;
                closest = colliders[i];
            }
        }

        return closest.GetComponent<Node>();
    }

    public static bool OnSight(Vector3 from, Vector3 to)
    {
        var dir = to - from;
        return !Physics.Raycast(from, dir, dir.magnitude, LayerMask.GetMask("Wall"));

        //A*
        //public List<Node> GetPath(Vector3 from, Vector3 to)
        //{
        //    var stopWatch = new Stopwatch();
        //    stopWatch.Start();
        //    var fromColliderArray = Physics.OverlapSphere(from, 1.5f, nodeMask);
        //    if (fromColliderArray.Length > 0)
        //    {
        //        fromNode = GetClosestNode(fromColliderArray, from);
        //    }

        //    var toColliderArray = Physics.OverlapSphere(to, 1.5f, nodeMask);
        //    if (toColliderArray.Length > 0)
        //    {
        //        toNode = GetClosestNode(toColliderArray, to);
        //    }

        //    closeNodes = new HashSet<Node>();
        //    openNodes = new PriorityQueue<Node>();

        //    var actualNode = fromNode;
        //    actualNode.Weight = 0;

        //    var watchdog = 100000;

        //    while (actualNode != null && actualNode != toNode && watchdog > 0)
        //    {
        //        watchdog--;

        //        foreach (var node in actualNode.neighbours)
        //        {
        //            if (closeNodes.Contains(node)) continue;

        //            var heuristic = actualNode.Weight + 
        //                Vector3.Distance(node.transform.position, actualNode.transform.position) +
        //                Vector3.Distance(node.transform.position, toNode.transform.position);

        //            if (node.Weight > heuristic)
        //            {
        //                node.Weight = heuristic;
        //                node.previous = actualNode;
        //            }

        //            if (!openNodes.Contains(node)) openNodes.Enqueue(node);
        //        }

        //        closeNodes.Add(actualNode);

        //        actualNode = openNodes.Dequeue();
        //    }

        //    var finalPath = new List<Node>();
        //    actualNode = toNode;
        //    while (actualNode != fromNode && actualNode.previous != null)
        //    {
        //        finalPath.Add(actualNode);
        //        actualNode = actualNode.previous;
        //    }

        //    finalPath.Reverse();
        //    stopWatch.Stop();
        //    UnityEngine.Debug.Log(stopWatch.Elapsed);
        //    UnityEngine.Debug.Log(stopWatch.ElapsedTicks);
        //    return finalPath;
        //}

    }
}
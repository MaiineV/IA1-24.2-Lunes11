using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//A*
//Theta*

public class Player : MonoBehaviour
{
    [SerializeField] Transform target;

    private bool isFollowing;
    private bool isWaitingForPath;

    private List<Node> actualPath = new List<Node>();

    [SerializeField] private float speed;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) isFollowing = !isFollowing;

        if(isFollowing && !isWaitingForPath)
        {
            FollowPath();
        }
    }

    private void FollowPath()
    {
        if(Pathfinding.OnSight(transform.position, target.position))
        {
            var dir = target.position - transform.position;

            transform.forward = dir;
            transform.position += transform.forward * speed * Time.deltaTime;
            return;
        }


        if(actualPath.Count > 0 && 
            Pathfinding.OnSight(target.position, actualPath[actualPath.Count - 1].transform.position) && 
            Pathfinding.OnSight(transform.position, actualPath[0].transform.position))
        {
            var dir = actualPath[0].transform.position - transform.position;

            transform.forward = dir;
            transform.position += transform.forward * speed * Time.deltaTime;

            if(dir.magnitude < 1f)
            {
                actualPath.RemoveAt(0);
            }
        }
        else
        {
            isWaitingForPath = true;
            Pathfinding.Instance.RequestPath(transform.position, target.position, PathCallback);
        }
    }

    private void PathCallback(List<Node> path)
    {
        actualPath = path;
        isWaitingForPath = false;
    }

    private void OnDrawGizmos()
    {
        if (actualPath.Count <= 0) return;

        foreach (Node node in actualPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(node.transform.position, 1.2f);
        }
    }
}

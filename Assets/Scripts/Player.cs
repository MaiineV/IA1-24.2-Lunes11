using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

//A*
//Theta*

public class Player : MonoBehaviour
{
    [SerializeField] Transform target;

    private bool isFollowing;
    private bool isWaitingForPath;

    private List<Node> actualPath = new List<Node>();

    [SerializeField] private float speed;
    [SerializeField] private float obstacleRange;

    [SerializeField] private float viewRange;

    [SerializeField] private LayerMask _obstacleMask;

    private int _obstacleCount = 0;

    private Vector3 _obstacleDir;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) isFollowing = !isFollowing;

        if (isFollowing && !isWaitingForPath)
        {
            FollowPath();
        }
    }

    private void FollowPath()
    {
        if (_obstacleCount == 0)
        {
            _obstacleDir = ObstacleAvoidance().normalized;
        }

        if (Pathfinding.FieldOfView(transform, target, 75f))
        {
            var dir = target.position - transform.position;

            transform.forward = dir.normalized + _obstacleDir;
            transform.position += transform.forward * speed * Time.deltaTime;
            return;
        }


        if (actualPath.Count > 0 &&
            Pathfinding.LineOfSight(target.position, actualPath[actualPath.Count - 1].transform.position) &&
            Pathfinding.LineOfSight(transform.position, actualPath[0].transform.position))
        {
            var dir = actualPath[0].transform.position - transform.position;

            transform.forward = dir.normalized + _obstacleDir;

            transform.position += transform.forward * speed * Time.deltaTime;

            if (dir.magnitude < 1f)
            {
                actualPath.RemoveAt(0);
            }
        }
        else
        {
            isWaitingForPath = true;
            Pathfinding.Instance.RequestPath(transform.position, target.position, PathCallback);
        }

        _obstacleCount++;
        if (_obstacleCount > 2) _obstacleCount = 0;
    }

    private Vector3 ObstacleAvoidance()
    {
        var obstacles = Physics.OverlapSphere(transform.position, obstacleRange, _obstacleMask);
        Debug.Log(obstacles.Length);

        if (obstacles.Length <= 0) return Vector3.zero;

        var obstacleDir = Vector3.zero;

        foreach (var obstacle in obstacles)
        {
            Debug.Log(obstacle.gameObject.name);
            obstacleDir += transform.position - obstacle.transform.position;
        }

        obstacleDir.y = 0f;

        return obstacleDir;
    }

    private void PathCallback(List<Node> path)
    {
        actualPath = path;
        isWaitingForPath = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, obstacleRange);

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, viewRange);
        //var angleDir = new Vector3(transform.position.x, transform.position.y + 75f / 2f, transform.position.z);
        //Gizmos.DrawLine(transform.position, transform.position + transform.forward * viewRange + Vector3.)

        if (actualPath.Count <= 0) return;

        foreach (Node node in actualPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(node.transform.position, 1.2f);
        }
    }
}

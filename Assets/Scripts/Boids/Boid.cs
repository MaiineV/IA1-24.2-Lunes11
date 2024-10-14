using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float speed;

    public Transform target;
    public Transform danger;

    private Vector3 fleeVector;
    private Vector3 seekVector;


    [Range(0,1)]public float rotationSpeed;
    [Range(0,1)]public float fleeWeight;
    [Range(0,1)]public float seekWeight;

    [Range(0,1)]public float separationWeight;
    [Range(0,1)]public float cohesionWeight;
    [Range(0,1)]public float allignWeight;

    GameObject[] closeBoids;

    void Awake()
    {
        var z = Random.Range(-1f, 1f);
        var x = Random.Range(-1f, 1f);

        transform.forward = new Vector3(x, 0, z);
    }

    void Update()
    {
        closeBoids = Physics.OverlapSphere(transform.position, 5)
            .Select(x => x.gameObject)
            .ToArray();

        //Seek();
        //Flee();
        //transform.forward = fleeVector * fleeWeight + seekVector * seekWeight;
        //transform.position += transform.forward * speed * Time.deltaTime;

        transform.forward = Separation().normalized * separationWeight
            + Cohesion().normalized * cohesionWeight
            + Allign().normalized * allignWeight;

        transform.position += transform.forward * speed * Time.deltaTime;
        BoidManager.Instance.CheckBounds(this);
    }

    public void Seek()
    {
        var dir = target.position - transform.position;
        dir.y = 0;
        seekVector = transform.forward * (1 - rotationSpeed) + dir * rotationSpeed;
    }

    public void Flee()
    {
        var dir = transform.position - danger.position;
        dir.y = 0;
        fleeVector = transform.forward * (1 - rotationSpeed) + dir * rotationSpeed;
    }

    public Vector3 Cohesion()
    {
        var dir = Vector3.zero;

        foreach (var t in closeBoids)
        {
            dir += t.transform.position - transform.position;
        }

        dir.y = 0;
        return dir;
    }

    public Vector3 Allign()
    {
        var dir = Vector3.zero;

        foreach (var t in closeBoids)
        {
            dir += t.transform.forward;
        }
        dir.y = 0;

        return dir;
    }

    public Vector3 Separation()
    {
        var dir = Vector3.zero;

        foreach (var t in closeBoids)
        {
            dir +=  transform.position- t.transform.position;
        }

        dir.y = 0;

        return dir;
    }
}

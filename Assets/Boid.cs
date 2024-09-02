using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        var z = Random.Range(-1f, 1f);
        var x = Random.Range(-1f, 1f);

        transform.forward = new Vector3(x, 0, z);
    }

    void Update()
    {
        Seek();
        Flee();
        transform.forward = fleeVector * fleeWeight + seekVector * seekWeight;
        transform.position += transform.forward * speed * Time.deltaTime;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public static BoidManager Instance;

    public float maxX;
    public float maxZ;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckBounds(Boid boid)
    {
        if (boid.transform.position.x > maxX)
            boid.transform.position = 
                new Vector3(
                    boid.transform.position.x - maxX * 2 * .9f, 
                    0, 
                    boid.transform.position.z);
       
        else if (boid.transform.position.x < -maxX)
            boid.transform.position = 
                new Vector3(
                    boid.transform.position.x + maxX * 2 * .9f, 
                    0, 
                    boid.transform.position.z);
        
        else if (boid.transform.position.z > maxZ)
            boid.transform.position = 
                new Vector3(
                    boid.transform.position.x, 
                    0, 
                    boid.transform.position.z - maxZ * 2 * .9f);

        else if (boid.transform.position.z < -maxZ)
            boid.transform.position = 
                new Vector3(
                    boid.transform.position.x, 
                    0, 
                    boid.transform.position.z + maxZ * 2 * .9f);
    }
}

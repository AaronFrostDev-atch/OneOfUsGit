using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ride : MonoBehaviour
{

    bool onRide;
    Vector3 velocity;
    Collider rideCollider;
    Vector3 lastPosition;



    // In late update since we need to check for collisions before moving
    void LateUpdate()
    {

        // Match our velocity to the ride
        if (onRide)
        {
            velocity = (rideCollider.transform.position - lastPosition) / Time.deltaTime;
            transform.position += velocity * Time.deltaTime;

            lastPosition = rideCollider.transform.position;
        }
    }


    // When we enter a ride zone
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.Contains("Rd"))
        {
            lastPosition = col.transform.position;
            onRide = true;
        }
    }


    // When we stay on a ride zone
    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag.Contains("Rd"))
        {

            onRide = true;

            rideCollider = col;

        }
    }

    // When we leave a ride zone
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag.Contains("Rd"))
        {
            onRide = false;
        }
    }
}

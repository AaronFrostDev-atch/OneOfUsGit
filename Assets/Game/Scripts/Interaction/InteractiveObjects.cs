using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObjects : Bolt.EntityBehaviour<IDynamicObjectState>
{
    public bool canHold = true;


    public bool colliding;


    bool usingHoldPhysics = false;

    bool hasSpringJoint = false;




    private void Update()
    {
        if (GetComponent<Rigidbody>())
        {
            if (entity.GetState<IDynamicObjectState>().Held)
            {
                //if (!usingHoldPhysics)
                //{
                //UseHoldPhysics(true);
                //}


                GetComponent<Rigidbody>().useGravity = false;
            }
            else
            {


                //UseHoldPhysics(false);
                GetComponent<Rigidbody>().useGravity = true;
            }


            
        }
    }



    //
    // INTERACTIVE OBJECT FUNCTIONS
    //




    // If object is attached to network
    public override void Attached()
    {
        // Syncronise Transforms at all times
        state.SetTransforms(state.DynamicObjectTransform, transform);
    }


    public void SetVelocity(Vector3 vel)
    {
        GetComponent<Rigidbody>().velocity = vel;
    }
    public void AddForce(Vector3 force)
    {
        GetComponent<Rigidbody>().AddForce(force);
    }
    public void Hold(Vector3 pos, Quaternion rot)
    {

        Vector3 direction = pos - transform.position;
        float distance = Vector3.Distance(pos, transform.position);


        GetComponent<Rigidbody>().velocity = (50 * direction) / GetComponent<Rigidbody>().mass;
        GetComponent<Rigidbody>().useGravity = false;

        
    }

    public void Drop(Vector3 velocity)
    {

        GetComponent<Rigidbody>().useGravity = true;
        AddForce(velocity);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.gameObject.tag.Contains("Lg") & !collision.transform.gameObject.tag.Contains("Rd"))
        {
            if (collision.gameObject.GetComponent<PlayerMove>() != null)
            {
                colliding = true;
            }
            else
            {
                colliding = false;
            }
        }
    }


    private void OnTriggerStay(Collider other)
    {
        // Interactive objects ignore invisible zones meant for player.
        if (!other.transform.gameObject.tag.Contains("Lg") & !other.transform.gameObject.tag.Contains("Rd"))
        {
            if(other.gameObject.GetComponent<PlayerMove>() != null)
            {
                colliding = true;
            }
            else
            {
                colliding = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        colliding = false;
    }




}

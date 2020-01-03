using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : Bolt.EntityBehaviour<IPState>
{
    
    
    GameObject playerCam;
    GameObject currentObjectInView;
    public GameObject currentInteractionObject;
    GameObject canvas;
    PlayerUI ui;


    // VARIABLES
    public float interactRange = 4f;
    public float throwForce = 10f;

    // TEMPORARY STATE FOR HOLD
    bool holding = false;
    bool pickedUp = false;
    float holdDistance = 0f;
    

    RaycastHit hit;


    private void Start()
    {
        playerCam = Camera.main.gameObject;

        canvas = GameObject.Find("Canvas");
        ui = canvas.GetComponent<PlayerUI>();

    }


    public void Look(bool e, bool edown, bool mouse0)
    {

        

        // Are we looking at anything in interaction range
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactRange, 9, QueryTriggerInteraction.UseGlobal))
        {
            // Are we looking at an interactive object
            if (hit.transform.GetComponent<InteractiveObjects>())
            {
                currentObjectInView = hit.transform.gameObject;

                ///
                ///  INTERACTION MECHANICS
                ///
                



                // Physical Item pickup
                if (currentObjectInView.GetComponent<InteractiveObjects>().canHold & !currentObjectInView.GetComponent<BoltEntity>().GetState<IDynamicObjectState>().Held)
                {
                    RaycastHit hit;
                    bool hitMe = false;
                    foreach (Collider item in Physics.OverlapSphere(currentObjectInView.transform.position + Vector3.up, 1.2f))
                    {
                        if(item.gameObject == this.gameObject)
                        {
                            hitMe = true;
                        }
                        
                    }

                    if (!hitMe)
                    {
                        PickUp(edown);
                    }


                    

                }


                ///
                ///
                ///


            }
            else
            {
                currentObjectInView = null;
            }
        }
        else
        {
            currentObjectInView = null;
        }



        HoldActions(e, edown, mouse0);


        ManageUI();

    }



    void HoldObj(GameObject obj)
    {
        Vector3 holdPosition = playerCam.transform.position + playerCam.transform.forward * holdDistance + Vector3.up * .5f;
        Quaternion holdRot = transform.rotation;

        SendHoldEvent(obj.GetComponent<BoltEntity>(), holdPosition, holdRot);
    }
    void DropObj(GameObject obj)
    {
        ClearInteractionVars();

        SendDropEvent(obj.GetComponent<BoltEntity>(), Vector3.zero);
    }
    void ThrowObj(GameObject obj)
    {
        ClearInteractionVars();

        SendDropEvent(obj.GetComponent<BoltEntity>(), ((playerCam.transform.forward * throwForce) / obj.GetComponent<Rigidbody>().mass));
    }

    void ClearInteractionVars()
    {
        holding = false;
        pickedUp = false;
        currentInteractionObject = null;
    }


    private void PickUp(bool edown)
    {
        if (!holding & edown)
        {
            Ray ray = new Ray();
            ray.origin = currentObjectInView.transform.position;
            ray.direction = Vector3.up;
            RaycastHit hit;

            // Its directly under something
            if (Physics.Raycast(ray, out hit, 3f))
            {

                // If its not us
                if (hit.transform.gameObject != this.gameObject)
                {
                    // Pick up

                    pickedUp = true;


                    // Set object to hold
                    currentInteractionObject = currentObjectInView;


                    // Set distance to hold
                    holdDistance = Vector3.Distance(playerCam.transform.position, currentInteractionObject.transform.position);
                }
            }
            else
            {
                // Pick up

                pickedUp = true;


                // Set object to hold
                currentInteractionObject = currentObjectInView;


                // Set distance to hold
                holdDistance = Vector3.Distance(playerCam.transform.position, currentInteractionObject.transform.position);
            }




        }
    }

    private void HoldActions(bool e, bool edown, bool mouse0)
    {
        if (pickedUp)
        {
            if (holding)
            {
                if (!edown & !currentInteractionObject.GetComponent<InteractiveObjects>().colliding & (Vector3.Distance(currentInteractionObject.transform.position, this.transform.position) < interactRange + 5f))
                {

                    if (mouse0)
                    {
                        ThrowObj(currentInteractionObject);
                    }

                    if (holding)
                    {
                        HoldObj(currentInteractionObject);
                    }
                }
                else
                {
                    DropObj(currentInteractionObject);
                }
            }

            if (edown & holding == false & pickedUp == true)
            {
                holding = true;
                HoldObj(currentInteractionObject);

            }
        }
    }




    // Events and their Overloads


    // Hold
    void SendHoldEvent(BoltEntity ent, Vector3 position, Quaternion rotation)
    {
        var objMoved = ObjectMoved.Create(Bolt.GlobalTargets.OnlyServer);

        objMoved.Hold = true;
        objMoved.Push = false;

        objMoved.Entity = ent;
        objMoved.Position = position;
        objMoved.Rotation = rotation;

        objMoved.Send();
    }


    void SendDropEvent(BoltEntity ent, Vector3 velocity)
    {
        var objMoved = ObjectMoved.Create(Bolt.GlobalTargets.OnlyServer);

        objMoved.Hold = false;
        objMoved.Push = false;

        objMoved.Velocity = velocity;
        objMoved.Entity = ent;

        objMoved.Send();
    }


    void ManageUI()
    {
        if(currentObjectInView != null || holding)
        {
            ui.ShowPointer(true);
        }
        else
        {
            ui.ShowPointer(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (holding)
        {
            if (other.GetComponent<BoltEntity>() != null)
            {
                if(other.gameObject == currentInteractionObject)
                {
                    DropObj(currentInteractionObject);
                }
            }
            
        }
    }



}

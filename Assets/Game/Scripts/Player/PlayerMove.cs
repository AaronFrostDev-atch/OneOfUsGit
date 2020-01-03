

using UnityEngine;
using System.Collections;
using Bolt;




public class PlayerMove : Bolt.EntityBehaviour<IPState>
{
    // COMPONENTS DEFINED

    CharacterController playerCC;


    // ADJUSTABLE VARIABLES DEFINED

    public float moveSpeed = 3f;
    public float sprintMultiplier = 1.5f;
    public float fallAcceleration = 2f;
    public float maxFallSpeed = 10f;
    public float jumpHeight = 1;
    public float airControl = 2.5f;
    public float slideAcceleration = .03f;
    public bool canDiagSprint = true;


    // DEFINE CAMERA GAME OBJECT

    public GameObject PlayerCam;





    // TEMPORARY VARIABLES DEFINED

    [SerializeField]
    private float fallSpeed = 0;

    Vector3 moveDirection;

    Vector3 slideDirection;

    Vector3 collisionPoint;

    Ray groundRay;

    public float moveX;
    public float moveY;

    float inputX;
    float inputY;


    float diagMultiplier = 1 / Mathf.Sqrt(2);
    bool movingDiagonally;

    float slopeAngle;

    public bool sliding;
    public bool collidingWithPlayer = false;

    bool hitCeiling = false;



    GameObject slideGameObject;
    Transform slideTransform;
    public GameObject climbPoint;

    float slideVel;
    bool haveGottenVelY;
    bool sprinting = false;
    bool diagSprinting = false;



    public void Start()
    {



        // ASSIGN COMPONENTS

        playerCC = GetComponent<CharacterController>();


        // CREATE SLIDE TRANSFORM

        slideGameObject = new GameObject("SlideGameObject");
        slideGameObject.transform.SetParent(transform);
        slideTransform = slideGameObject.transform;

        

        // ASSIGN CAMERA TO CAMERA GAMEOBJECT IN CHILDREN

        PlayerCam = transform.GetComponentInChildren<Camera>().gameObject;


        Physics.IgnoreCollision(GetComponent<Collider>(), playerCC);


    }


        
    public void Look(float rotx, float roty)
    {
        // CAMERA LOOK


        // CREATE ROTATIONAL VECTOR FOR PLAYER

        Quaternion playerRotation = Quaternion.Euler(0, roty * GameData.UserSettings.mouseSensitivity, 0);


        // ROTATE PLAYER - SPHERICALLY INTERPOLATED FOR SMOOTHING

        transform.localRotation = Quaternion.Slerp(transform.localRotation, playerRotation, GameData.UserSettings.mouseSmoothing);



        // CREATE ROTATIONAL VECTOR FOR CAMERA


        Quaternion camRotation = Quaternion.Euler(Mathf.Clamp(rotx * GameData.UserSettings.mouseSensitivity, -90, 90), 0, 0);


        // ROTATE CAMERA - SPHERICALLY INTERPOLATED FOR SMOOTHING

        PlayerCam.transform.localRotation = Quaternion.Slerp(PlayerCam.transform.localRotation, camRotation, GameData.UserSettings.mouseSmoothing);
    }



    public void Move(float xinput, float yinput, bool jumpinput)
    {




        








        // PLAYER MOVE



        // COLLECT INPUT

        inputX = xinput;
        inputY = yinput;



        // CHECK FOR DIAGONAL MOVEMENT, (well use this later)

        if (inputX != 0 & inputY != 0)
        {
            movingDiagonally = true;
        }
        else
        {
            movingDiagonally = false;
        }

        


        // SET MOVE X AND MOVE Y

        if (isGrounded())
        {
            // If we're grounded, movement is just going to be the immediate input.

            moveX = inputX;
            moveY = inputY;

            if (moveY == 1 & Input.GetKey(KeyCode.LeftShift))
            {

                sprinting = true;
                moveY = inputY * sprintMultiplier;
                if(Mathf.Abs(moveX) > .7f)
                {
                    diagSprinting = true;
                    moveX = inputX * sprintMultiplier;
                }
                else
                {
                    diagSprinting = false;
                }
            }
            else
            {
                sprinting = false;
                diagSprinting = false;
            }


        }
        else
        {



            // If we're airbourne 
            // This was implemented since you could technically be "not moving diagonally" since both buttons werent down, but you could still move too fast since jumps retain momentum
            if (!sprinting)
            {
                if (Mathf.Abs(moveX) + Mathf.Abs(moveY) <= 2 * diagMultiplier) // If our total velocity(sum of absolute values) is less than or equal to our maximum velocity(2 times diagonal speed), add input to movement
                {
                    moveX += inputX * Time.deltaTime * airControl;
                    moveY += inputY * Time.deltaTime * airControl;
                    moveX = Mathf.Clamp(moveX, -1f, 1f);
                    moveY = Mathf.Clamp(moveY, -1f, 1f);
                }
            }
            else
            {

                if (diagSprinting)
                {
                    if (Mathf.Abs(moveX) + Mathf.Abs(moveY) <= 2 * diagMultiplier * sprintMultiplier) // If our total velocity(sum of absolute values) is less than or equal to our maximum velocity(2 times diagonal speed), add input to movement
                    {
                        moveX += inputX * Time.deltaTime * airControl;
                        moveY += inputY * Time.deltaTime * airControl;
                    }
                }
                else
                {
                    if (Mathf.Abs(moveY) <= sprintMultiplier) // If our total velocity(sum of absolute values) is less than or equal to our maximum velocity(2 times diagonal speed), add input to movement
                    {
                        moveX += inputX * Time.deltaTime * airControl;
                        moveY += inputY * Time.deltaTime * airControl;
                    }

                }
            }


            // IF WE STOPPED PRESSING IN MID AIR, SLOW DOWN A LITTLE

            if (inputX == 0)
            {
                if (moveX > 0)
                {
                    moveX -= airControl * Time.deltaTime;
                }
                else if (moveX < 0)
                {
                    moveX += airControl * Time.deltaTime;
                }

            }
            if (inputY == 0)
            {
                if (moveY > 0)
                {
                    moveY -= airControl * Time.deltaTime;
                }
                else if (moveY < 0)
                {
                    moveY += airControl * Time.deltaTime;
                }
            }

        }








        // HANDLE DIAGONAL MOVEMENT SPEEDS

        if (movingDiagonally)
        {
            if (moveY > 1 & canDiagSprint)
            {
                moveX = Mathf.Clamp(moveX, -diagMultiplier * sprintMultiplier, diagMultiplier * sprintMultiplier);
                moveY = Mathf.Clamp(moveY, -diagMultiplier * sprintMultiplier, diagMultiplier * sprintMultiplier);
            }
            else
            {
                moveX = Mathf.Clamp(moveX, -diagMultiplier, diagMultiplier);
                moveY = Mathf.Clamp(moveY, -diagMultiplier, diagMultiplier);
            }
        }
        else
        {
            if (moveY <= -1f)
            {
                moveX = Mathf.Clamp(moveX, -1f, 1f);
                moveY = Mathf.Clamp(moveY, -1f, 1f);
            }
        }

        moveY = Mathf.Clamp(moveY, -sprintMultiplier, sprintMultiplier);
        moveX = Mathf.Clamp(moveX, -sprintMultiplier, sprintMultiplier);




        HandleSliding();


        if (!sliding)
        {

            // IF NOT SLIDING, MOVE THE PLAYER WITH INPUT AND GRAVITY




            haveGottenVelY = false; // We are not sliding, need to reset this value



            // CALCULATE MOVEMENT VECTOR (JUMP IS HANDLED WITHIN FallMultiplier() )

            moveDirection = Vector3.forward * moveY * moveSpeed + Vector3.right * moveX * moveSpeed + Vector3.up * FallMultiplier(jumpinput);


            // CHANGE VECTOR FROM GLOBAL TO LOCAL SPACE
            moveDirection = transform.TransformDirection(moveDirection);


            if (collidingWithPlayer)
            {
                // PUSH AWAY FROM PLAYER
                moveDirection = new Vector3(-moveDirection.x, moveDirection.y, -moveDirection.z);

            }


            playerCC.Move(moveDirection * 6 * Time.deltaTime);




        }
        else
        {

            // IF SLIDING, MOVE THE PLAYER ALONG SLIDE DIRECTION



            if (!haveGottenVelY) // if this is the first frame we are sliding
            {
                slideVel = playerCC.velocity.y; // get our vertical velocity
                slideVel = Mathf.Abs(slideVel);
                haveGottenVelY = true; // tell computer to no longer get our vertical velocity so we can lerp it
            }


            slideVel = Mathf.Lerp(slideVel, 180, slideAcceleration); // accelerate the velocity of the slide from our previous Yveolcity to max fall speed which is around 180 (I reccomend slide acceleration of .005)
            slideDirection = -slideTransform.up * slideVel; // set slide vector


            // APPLY MOVEMENT 

            playerCC.Move(slideDirection * Time.deltaTime);
        }
    }


    RaycastHit groundHit;


    void HandleSliding()
    {

        groundRay.origin = collisionPoint + Vector3.up * .02f;
        groundRay.direction = Vector3.down;


        
        slideTransform.rotation = transform.rotation;


        // IF WE ARE ON A SURFACE, DETERMINE IF WE SHOULD BE SLIDING

        if (Physics.Raycast(groundRay, out groundHit, .1f)){


            

            slopeAngle = Vector3.Angle(transform.up, groundHit.normal); // The angle of the slope is the angle between up and the normal of the slope

            if(slopeAngle <= playerCC.slopeLimit) // if angles arent too high
            {
                sliding = false;
                
            }
            else if(slopeAngle > playerCC.slopeLimit) // if angles are too high
            {

                if (groundHit.transform.gameObject.tag.Contains("Sl") & playerCC.velocity.y <= 0)
                {
                    sliding = true;

                    Vector3 groundCross = Vector3.Cross(groundHit.normal, Vector3.up);
                    slideTransform.rotation = Quaternion.FromToRotation(transform.up, Vector3.Cross(groundCross, groundHit.normal)); // collect the transform of the ground we need to slide down
                }


            }



        }

    }


    public LayerMask groundDetectLayerMask;


    public bool isGrounded()
    {

        float sphereRadius = .5f;
        float maxDistance = .581f;
        
        Vector3 direction = Vector3.down;
        Vector3 origin = transform.position;
        
        

        RaycastHit hit;

        if (Physics.SphereCast(origin, sphereRadius, direction, out hit, maxDistance, groundDetectLayerMask))
        {
            if (hit.transform.gameObject != this.gameObject & !hit.transform.tag.Contains("Lg"))
            {
                slopeAngle = Vector3.Angle(transform.up, hit.normal); // The angle of the slope is the angle between up and the normal of the slope

                if (slopeAngle <= playerCC.slopeLimit) // if angles arent too high
                {
                    return true;

                }
                else
                {
                    return false;
                }
                
            }
            else
            {
                return false;
            }


            
        }
        else
        {
            return false;
        }
    }






    // THIS ALGORITHIM CHECKS IF WE JUST FELL OFF OF A LEDGE, AND IT ADJUSTS DOWNWARDS VELOCITY IN FIGURE B TO COMPENSATE FOR FIGURE A (Below)
    bool jumped = false;

    public bool ledgeGrabbed = false;


    float FallMultiplier(bool jumpinput)
    {
        // IF PLAYER IS GROUNDED, RESET FALL SPEED

        if (isGrounded())
        {

            jumped = false;

            // IF SPACEBAR PRESSED, JUMP

            if (jumpinput)
            {
                Ray ray = new Ray();
                ray.origin = transform.position;
                ray.direction = Vector3.down;
                RaycastHit hit;


                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    if (hit.transform.gameObject.tag.Contains("Sl"))
                    {
                        float slopeAngle =  Vector3.Angle(transform.up, groundHit.normal);
                        if (slopeAngle <= playerCC.slopeLimit)
                        {
                            fallSpeed = jumpHeight / 2;
                            jumped = true;
                        }
                        else
                        {
                            sliding = true;
                        }
                    }
                    else
                    {
                        fallSpeed = jumpHeight / 2;
                        jumped = true;
                    }
                }
                else
                {
                    fallSpeed = jumpHeight / 2;
                    jumped = true;
                }
                
            }

        }
        else // IF PLAYER IS IN THE AIR, RAISE FALL SPEED UNTIL IT REACHES MAX (DO FALL ACCELERATION)
        {
            if (!jumped)
            {
                // FIGURE B

                fallSpeed = .2f; 
                jumped = true;
            }
            else if (fallSpeed < maxFallSpeed)
            {
                fallSpeed += fallAcceleration * Time.deltaTime;
            }


            if (ledgeGrabbed)
            { 
                // Jump again if we are in a ledge collider and press jump
                fallSpeed = jumpHeight / 2;


                // Make sure this only happens once
                ledgeGrabbed = false;
            }

            float sphereRadius = .5f;
            float maxDistance = .581f;

            Vector3 direction = Vector3.up;
            Vector3 origin = transform.position;



            RaycastHit hit;

            // If we hit a ceiling, stop velocity
            if (Physics.SphereCast(origin, sphereRadius, direction, out hit, maxDistance, groundDetectLayerMask))
            {

                if (!hit.transform.gameObject.tag.Contains("Lg"))
                {
                    if (!hitCeiling)
                    {
                        fallSpeed = 0;
                        hitCeiling = true;
                    }

                }


            }
            else
            {
                hitCeiling = false;
            }

            
        }
        

        return fallSpeed;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {



        
        collisionPoint = hit.point;






        // PUSH NON KINEMATIC RIGIDBODYS



        Rigidbody rb = hit.transform.GetComponent<Rigidbody>();


        float pushPower = 2.0f;


        // no rigidbody
        if (rb == null || rb.isKinematic) { return; }
        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3) { return; }
        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.
        // Apply the push


        // if it is a bolt entity
        if (rb.GetComponent<BoltEntity>() != null) {

            if (BoltNetwork.IsServer)
            {
                rb.velocity = pushDir * pushPower;
            }
            else
            {
                var objMoved = ObjectMoved.Create(Bolt.GlobalTargets.OnlyServer);

                objMoved.Push = true;
                objMoved.Entity= rb.GetComponent<BoltEntity>();
                objMoved.Velocity = pushDir * pushPower;
     

                objMoved.Send();
            }
        }
        else
        {
            rb.velocity = pushDir * pushPower;
        }

        
    }


    public bool canLedgeGrab = true;
    








    void OnTriggerStay(Collider col)
    {
        if (entity.IsOwner)
        {





            if (col.transform.tag.Contains("Lg"))
            {

                if (Input.GetKey(KeyCode.Space) & Input.GetKey(KeyCode.W) & !Input.GetKey(KeyCode.S))
                {
                    ledgeGrabbed = true;
                    canLedgeGrab = false;


                }
                
            }



            if (col.gameObject.tag.Contains("Pl") & col.gameObject != this.gameObject)
            {
                collidingWithPlayer = true;
            }
            else
            {
                collidingWithPlayer = false;
            }
        }
        




    }

    void OnTriggerExit(Collider col)
    {
        if (entity.IsOwner)
        {
            if (col.gameObject.tag.Contains("Lg"))
            {
                // Make sure we can't spam jumps while in a ledge
                canLedgeGrab = true;
            }


            if (col.gameObject.tag.Contains("Pl"))
            {
                collidingWithPlayer = false;
            }
        }

    }



    public void Respawn()
    {
        transform.position = GameData.SpawnPositions.spawns[0];
        fallSpeed = 0;

        
    }


}

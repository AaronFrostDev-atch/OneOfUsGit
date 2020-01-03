using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;



public class PlayerController : Bolt.EntityBehaviour<IPState>
{


    float _x;
    float _y;
    bool _jump;
    bool _e;
    bool _edown;
    bool _mouse0;
    bool _escdown;
    public float _rotx;
    float _roty;
    public float lookClamp = 90f;
    bool paused = false;
    bool stopMoveInput = false;

    bool respawning = false;





    PlayerMove _motor;
    Interact _interact;
    GameObject canvas;


    void Awake()
    {
        _motor = GetComponent<PlayerMove>();
        _interact = GetComponent<Interact>();
        canvas = GameObject.Find("Canvas");
    }

    public override void Attached()
    {
        state.SetTransforms(state.PTransform, transform);
    }

    // GET INPUT
    void PollKeys()
    {
        _edown = Input.GetKeyDown(KeyCode.E);
        _e = Input.GetKey(KeyCode.E);
        _mouse0 = Input.GetKeyDown(KeyCode.Mouse0);


        // Movement
        _x = Input.GetAxisRaw("Horizontal");
        _y = Input.GetAxisRaw("Vertical");
        _jump = Input.GetKey(KeyCode.Space);




        // Look
        _roty += Input.GetAxis("Mouse X");

        _rotx -= Input.GetAxis("Mouse Y");


        // Clamp vertical look
        _rotx = Mathf.Clamp(_rotx, -lookClamp / GameData.UserSettings.mouseSensitivity, lookClamp / GameData.UserSettings.mouseSensitivity);


    }

    void PollPauseKey()
    {
        _escdown = Input.GetKeyDown(KeyCode.Escape);
    }




    private void FixedUpdate()
    {




    }

    private void Update()
    {


        if (entity.IsOwner)
        {


            PollPauseKey();



            if (_escdown)
            {
                paused = !paused;
            }


            if (paused)
            {

                // Clear move input 
                if (!stopMoveInput)
                    StopMoveInput();

                // Do pause initialize stuff



                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;

                canvas.GetComponent<PauseMenu>().Pause();

            }
            else
            {
                canvas.GetComponent<PauseMenu>().UnPause();

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                // Take move input

                stopMoveInput = false;
                PollKeys();

                // Camera look when not paused
                _motor.Look(_rotx, _roty);


            }




            // Move
            if (transform.position.y < GameData.WorldLimits.worldVoidDepth)
            {
                // Resets transform position
                _motor.Respawn();
                respawning = true;
            }


            // Don't move if we need to respawn, (you cant do both at the same time)
            if (!respawning)
            {
                _motor.Move(_x, _y, _jump);
            }
            respawning = false;

            // Interactions
            _interact.Look(_e, _edown, _mouse0);

        }
    }













    private void StopMoveInput()
    {
        stopMoveInput = true;


        _edown = false;
        _e = false;
        _mouse0 = false;

        _x = 0;
        _y = 0;
        _jump = false;

    }




}

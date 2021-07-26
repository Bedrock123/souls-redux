using UnityEngine;
using Rewired;

public class InputHandler : MonoBehaviour
{
    [Header("Movement")]
    public float playerHorizontalInput;
    public float playerVerticalInput;

    [Header("Action Buttons")]
    public bool b_Input;
    public bool a_Input;
    public bool x_Input;
    public bool y_Input;

    [Header("Action Bumpers Right")]
    public bool rb_Input;
    public bool rt_Input;

    [Header("Action Bumpers Left")]
    public bool lb_Input;
    public bool lt_Input;

    [Header("Misc Controls")]
    public bool strong_attack_Input;
    public bool lockon_Input;

    [Header("Action Timers")]
    public float b_Input_Timer;
    public float rt_Input_Timer;
    public float lt_Input_Timer;

    float delta;
    StateManager stateManager;
    CameraManager cameraManager;
    private Player player;


    private void Awake()
    {
        stateManager = GetComponent<StateManager>();
        stateManager.Init();

        // Get the player object form the playe index
        player = ReInput.players.GetPlayer(0);

        // Reference the camera in the singleton
        cameraManager = CameraManager.singelton;

        // INit the camera manager with this.transform as the target
        cameraManager.Init(stateManager);

    }

    void FixedUpdate()
    {
        // Set delta as time
        delta = Time.fixedDeltaTime;

        // get all the inputs from rewqired
        GetInput();

        // Update the state manage and pass in the input state values
        UpdateStates();

        // Update the statement from the input handler via the Time.Delta time
        stateManager.FixedTick(delta);

        // Update the camera manager
        cameraManager.Tick(delta);

        ResetInputAndStates();
    }

    private void Update()
    {
        delta = Time.deltaTime;
        stateManager.Tick(delta);
    }

    void GetInput()
    {
        // Get the player movement
        playerHorizontalInput = player.GetAxis("Move Horizontal");
        playerVerticalInput = player.GetAxis("Move Vertical");

        // Get player actions
        b_Input = player.GetButton("B_Input");
        x_Input = player.GetButtonDown("X_Input");
        a_Input = player.GetButtonDown("A_Input");
        y_Input = player.GetButtonDown("Y_Input");

        // Attacks
        rb_Input = player.GetButtonDown("RB_Input");
        lb_Input = player.GetButtonDown("LB_Input");

        // Misc Inputs
        strong_attack_Input = player.GetButton("Strong Attack");
        lockon_Input = player.GetButtonDown("LockOn");

        // Strong attacks
        rt_Input = (strong_attack_Input && rb_Input) || player.GetButtonDown("RT_Input");
        lt_Input = strong_attack_Input && lb_Input || player.GetButtonDown("LT_Input");

        // Disable weak attacks if strong attacks on
        rb_Input = rb_Input && !rt_Input;
        lb_Input = lb_Input && !lt_Input;

        if (b_Input)
            b_Input_Timer += delta;
    }

    void UpdateStates()
    {
        // Apply the horixon and vertical to the state manager from input
        stateManager.horizontal = playerHorizontalInput;
        stateManager.vertical = playerVerticalInput;

        // Get the forward direction vector
        Vector3 vertical = playerVerticalInput * cameraManager.transform.forward;
        Vector3 horizontal = playerHorizontalInput * cameraManager.transform.right;

        // This will move based on the camera angle
        stateManager.moveDirection = (vertical + horizontal).normalized;

        // Works as row acess to tell you there is movement in the controll
        float movementAnimationSpeed = Mathf.Abs(playerHorizontalInput) + Mathf.Abs(playerVerticalInput);

        // Clamp that movement between 0 and 1
        stateManager.moveAmount = Mathf.Clamp01(movementAnimationSpeed);

        // If the run input is flagged
        if (b_Input && b_Input_Timer > 0.2f)
        {
            // Only set state manage to run if our movemen taount is greater then 1
            stateManager.run = (stateManager.moveAmount > 0);
        } else
        {
            stateManager.run = false;
        }

        // If we held the button for less then .5 seconds then roll
        if (b_Input == false && b_Input_Timer > 0 && b_Input_Timer < 0.2f)
            stateManager.rollInput = true;
    

        // Update the state manager with the triggers
        stateManager.rt = rt_Input;
        stateManager.rb = rb_Input;
        stateManager.lb = lb_Input;
        stateManager.lt = lt_Input;

        // Handle the two handed states
        if (y_Input)
        {
            stateManager.isTwoHanded = !stateManager.isTwoHanded;
            stateManager.HandleTwoHanded();
        }

        // Handle lock on
        if (lockon_Input)
        {
            // Flip the lock on and off
            stateManager.lockOn = !stateManager.lockOn;

            // If there is not target currently then simple just set lock to false
            if (stateManager.lockOnTarget == null)
            {
                stateManager.lockOn = false;
            }

            // Set teh state managet enenty target transofrm as teh camera lock on target
            cameraManager.lockOnTarget = stateManager.lockOnTarget;

            // Set the lock on transform as well
            stateManager.lockOnTransform = cameraManager.lockOnTransform;

            // Turn on the camera manager lock on flag
            cameraManager.lockOn = stateManager.lockOn;
        } else
        {
            if (stateManager.lockOn == false)
            {
                cameraManager.lockOn = false;
            }
        }

    }

    void ResetInputAndStates()
    {
        if (b_Input == false)
            b_Input_Timer = 0;

        if (stateManager.rollInput)
            stateManager.rollInput = false;

        if (stateManager.run)
            stateManager.run = false;
    }
}

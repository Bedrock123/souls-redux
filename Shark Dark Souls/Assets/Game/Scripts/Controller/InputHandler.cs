using UnityEngine;
using Rewired;

public class InputHandler : MonoBehaviour
{
    [Header("Rewired Inputs")]
    public float playerHorizontalInput;
    public float playerVerticalInput;

    public bool b_Input;
    public bool a_Input;
    public bool x_Input;
    public bool y_Input;

    bool rb_Input;
    float rt_Axis;
    bool rt_Input;

    bool lb_Input;
    bool lt_Axis;
    float lt_Input;

    float delta;

    StateManager stateManager;

    CameraManager cameraManager;

    private Player player;

    private void Awake()
    {
        // Get the player object form the playe index
        player = ReInput.players.GetPlayer(0);

        // Reference the camera in the singleton
        cameraManager = CameraManager.singelton;

        Debug.Log(cameraManager);

        // INit the camera manager with this.transform as the target
        cameraManager.Init(transform);

    }

    // Start is called before the first frame update
    void Start()
    {
        stateManager = GetComponent<StateManager>();
        stateManager.Init();
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
        rt_Axis = player.GetAxis("rt_Input");
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
        if (b_Input)
        {
            // Only set state manage to run if our movemen taount is greater then 1
            stateManager.run = (stateManager.moveAmount > 0);
        }
        else
        {
            // If not then set to false
            stateManager.run = false;
        }

    }
}

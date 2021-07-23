using UnityEngine;
using Rewired;

public class CameraManager : MonoBehaviour
{
    [Header("Input")]
    public float cameraHorizintalInput;
    public float cameraVerticalInput;
    bool usedRightAxis;

    [Header("Speeds")]
    public bool lockOn;
    public float followSpeed = 9;
    public float targetSpeed = 2;

    [Header("Targets")]
    public Transform target;
    public EnemyTarget lockOnTarget;
    public Transform lockOnTransform;

    [HideInInspector]
    public Transform cameraPivotTransform;
    [HideInInspector]
    public Transform cameraTransform;
    StateManager stateManager;

    [Header("Angles")]
    public float minimumPivotAngle = -35;
    public float maximumPivotAngle = 35;
    float turnSmoothing = 0.1f;

    float smoothX;
    float smoothY;
    float smoothXVelocity;
    float smoothYVelocity;
    public float lookAngle;
    public float tiltAngle;

    private Player player;

    public static CameraManager singelton;

    public void Init(StateManager _stateManager)
    {
        // Set the tarrget on init.
        target = _stateManager.transform;
        stateManager = _stateManager;

        // Set the camera transform & pivtor transform
        cameraTransform = Camera.main.transform;
        cameraPivotTransform = cameraTransform.parent.transform;
    }

    public void Tick(float _delta)
    {
        // Handle camera movement
        cameraHorizintalInput = player.GetAxis("Move Camera Horizontal");
        cameraVerticalInput = player.GetAxis("Move Camera Vertical");

        // If there is a lock on target
        if (lockOnTarget != null)
        {
            // If there is not lock on transform set one
            if (lockOnTransform == null)
            {
                lockOnTransform = lockOnTarget.GetTarget();
                stateManager.lockOnTransform = lockOnTransform;
            }

            // If the left stick X axis is greater then .6 then 
            if (Mathf.Abs(cameraHorizintalInput) > 0.6f)
            {
                // If we have noa already used 
                if (!usedRightAxis)
                {
                    // Switch the axis
                    lockOnTransform = lockOnTarget.GetTarget((cameraHorizintalInput > 0));
                    stateManager.lockOnTransform = lockOnTransform;
                    usedRightAxis = true;
                }
            }
        }

        // If the right axis was used
        if (usedRightAxis)
        {
            // If the stick is based in its normal position
            if (Mathf.Abs(cameraHorizintalInput) < 0.6f)
            {
                // Set use right axis to false
                usedRightAxis = false;
            }
        }

        // Follows the player head base
        FollowTarget(_delta);

        // Handles the camera rotation
        HandleRotations(_delta, cameraVerticalInput, cameraHorizintalInput, targetSpeed);
    }

    void FollowTarget(float _delta)
    {
        // Create the follow speed based on the time detla and the public follow speed
        float _followSpeed = _delta * followSpeed;

        // Lerp the current position tothe target position
        Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, _followSpeed);

        // Set the transform of the parent game object to the target position
        transform.position = targetPosition;
    }

    void HandleRotations(float _delta, float _vertical, float _horizontal, float _targetSpeed)
    {
        // If turn smoothing is on
        if (turnSmoothing > 0)
        {
            // Take both inputs and smooth them out by the turn smoothing propery and set it to the smoothed x and y values
            smoothX = Mathf.SmoothDamp(smoothX, _horizontal, ref smoothXVelocity, turnSmoothing);
            smoothY = Mathf.SmoothDamp(smoothY, _vertical, ref smoothYVelocity, turnSmoothing);
        } else
        {
            // If there is no smoothing then just assign the raw values to smoothing
            smoothX = _horizontal;
            smoothY = _vertical;
        }

        // Handle teh tile up and down
        // Turns the camera pivot up and down which needs to have a clamp
        tiltAngle += smoothY * _targetSpeed;

        // Clamp the angle so it does go full 360 degrees
        tiltAngle = Mathf.Clamp(tiltAngle, minimumPivotAngle, maximumPivotAngle);

        // Rotate the local rotation of the camera pivot
        cameraPivotTransform.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

        // Hand 
        // If lock on do semton else
        if (lockOn && lockOnTarget != null)
        {
            // Get the direction towards the target
            Vector3 directionTowardsLockOnTarget = lockOnTransform.position - transform.position;

            // Normalize the direction
            directionTowardsLockOnTarget.Normalize();

            //directionTowardsLockOnTarget.y = 0;

            if (directionTowardsLockOnTarget == Vector3.zero)
                directionTowardsLockOnTarget = transform.forward;

            // Get the rotaiton from the direectio of the player
            Quaternion targetRotation = Quaternion.LookRotation(directionTowardsLockOnTarget);

            // Slper the two rotations together
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _delta * 9);

            // Reset the look angle so it does not jump
            lookAngle = transform.eulerAngles.y;

            return;
        }

        // The lock on overrides the left and right look angle
        // Turn the camera on the look angle which is the main camera holder attached to the player
        lookAngle += smoothX * _targetSpeed;

        // Rotates the main camera player boast transform left and right as it rotates on the X axis
        transform.rotation = Quaternion.Euler(0, lookAngle, 0);


    }

    private void Awake()
    {
        // There will only be one camera in scene so set as singletoin
        singelton = this;

        // Get the player object form the playe index
        player = ReInput.players.GetPlayer(0);
    }
}

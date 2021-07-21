using UnityEngine;

public class StateManager : MonoBehaviour
{
    [Header("Init")]
    public GameObject activeModel;

    [Header("Inputs")]
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public Vector3 moveDirection;

    [Header("Stats")]
    public float moveSpeed = 2; // Counts for walking and jogging
    public float runSpeed = 3.5f; // Only counts for sprinting speed
    public float rotateSpeed = 5;

    [Header("Stats")]
    public bool run;

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Rigidbody playerRigidbody;

    public float delta;

    public void Init()
    {
        // Finds and sets the animator to rigid body
        SetUpAnimator();

        // Get the rigid body from the player gameobject
        playerRigidbody = GetComponent<Rigidbody>();

        // Set the rigid body default paras
        playerRigidbody.angularDrag = 999;
        playerRigidbody.drag = 4;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;


    }

    void SetUpAnimator()
    {
        // If there is not active model then just search in children
        if (activeModel != null)
        {
            animator = GetComponentInChildren<Animator>();

            if (animator == null)
            {
                Debug.Log("No model found");
            }
        }
        else
        {
            // Set active model from the animator game object - assuming only the GFX model would have that added
            activeModel = animator.gameObject;
        }

        // If tehre is active model then just set the animator from that active model
        if (animator == null)
            // If there is an active model set then find it and set the animator, same as get component from child
            animator = activeModel.GetComponent<Animator>();

        animator.applyRootMotion = false;
    }

    public void FixedTick(float _delta)
    {
        delta = _delta;

        // Prevents us from sliding when moving
        playerRigidbody.drag = (moveAmount > 0) ? 0 : 4;

        // Get the move or run speed based on the flag
        float targetSpeed = moveSpeed;

        if (run)
            targetSpeed = runSpeed;

        // We are updating the values in the input handler before we are calling this tick int he statemanager
        playerRigidbody.velocity = moveDirection * (targetSpeed * moveAmount);

        // Create a new vector3 so we dont mess with move direction
        Vector3 targetDirection = moveDirection;

        // Always set target direction to 0
        targetDirection.y = 0;

        // If the direction direction is just forward, then just set it to the transform.forward
        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        // Get the target rotation from look rotation with target direction
        Quaternion tr = Quaternion.LookRotation(targetDirection);

        // Sleprt the current rotation to the target rotation
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * rotateSpeed * moveAmount);

        // Set the current transform rotation to the target rotation
        transform.rotation = targetRotation;


        // Passes in the move amount to the animations
        HandleMovementAnimations();
    }

    void HandleMovementAnimations()
    {
        animator.SetFloat("Vertical", moveAmount, 0.4f, delta);
    }


}

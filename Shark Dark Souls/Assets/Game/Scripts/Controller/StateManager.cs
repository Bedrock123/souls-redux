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
    public bool rt, rb, lt, lb;
    public bool twoHanded;
    public bool rollInput;

    [Header("Stats")]
    public float moveSpeed = 2; // Counts for walking and jogging
    public float runSpeed = 3.5f; // Only counts for sprinting speed
    public float rotateSpeed = 5;
    public float toGround = 0.5f; // Offset to ground
    public float rollSpeed = 1;
    

    [Header("States")]
    public bool run;
    public bool lockOn;
    public bool onGround;
    public bool inAction;
    public bool canMove;
    public bool isTwoHanded;
    
    [Header("Other")]
    public EnemyTarget lockOnTarget;
    public Transform lockOnTransform;
    public AnimationCurve rollCurve;


    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Rigidbody playerRigidbody;
    [HideInInspector]
    public AnimatorHook animatorHook;
    [HideInInspector]
    public ActionManager actionManager;
    [HideInInspector]
    public InventoryManager inventoryManager;

    [HideInInspector]
    public float delta;
    [HideInInspector]
    public LayerMask ignoreLayers;

    float _actionDelay;

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

        // Get all the current invneoty from th player
        inventoryManager = GetComponent<InventoryManager>();
        inventoryManager.Init();

        // Set up the action slots
        actionManager = GetComponent<ActionManager>();
        actionManager.Init(this);

        // Assign the animator hook to the active model
        animatorHook = activeModel.AddComponent<AnimatorHook>();
        animatorHook.Init(this);

        // Add in the small helper to avoid on collision animator errors
        activeModel.AddComponent<Helper>();

        // Set ignore layers
        gameObject.layer = 8;
        ignoreLayers = ~(1 << 9);

        // Set ground to true
        animator.SetBool("OnGround", true);


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


        
        // Detect the bumper actions
        DetectAction();

        // If we are in action turn everything off for running
        if (inAction)
        {
            animator.applyRootMotion = true;

            _actionDelay += delta;

            // Ensure we ar in action for over .3 secnds before doing something
            if (_actionDelay > 0.3f)
            {
                inAction = false;
                _actionDelay = 0;
            } else
            {
                return;
            }

        }
        

        // In action equals the opoostie of the animato can move
        canMove = animator.GetBool("CanMove");

        if (!canMove)
            return;

        // If we cannot move then check also for handle rolls
        animatorHook.CloseRoll();
        HandleRolls();

        animator.applyRootMotion = false;

        // Prevents us from sliding when moving
        // If we are not on the ground then set zero so we fall fast down
        playerRigidbody.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;

        // Get the move or run speed based on the flag
        float targetSpeed = moveSpeed;

        if (run)
            targetSpeed = runSpeed;

        // If we are not on ground
        if (onGround)
            // We are updating the values in the input handler before we are calling this tick int he statemanager
            playerRigidbody.velocity = moveDirection * (targetSpeed * moveAmount);

        // If we are running lock on = false;
        if (run)
        {
            lockOn = false;
            
        }


        // Create a new vector3 so we dont mess with move direction
        Vector3 targetDirection = (lockOn == false) ? moveDirection
            : (lockOnTransform != null) ?
            lockOnTransform.position - transform.position :
            moveDirection;

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

        // Set the animator lock to the normal lock on input variabnle
        animator.SetBool("LockOn", lockOn);

        // If lock then handle movement thrugh the lock animatinos
        if (lockOn)
        {
            // If we are locked in then strife left and right and backwards
            HandleLockOnAnimations(moveDirection);
        } else
        {
            // Passes in the move amount to the animations
            HandleMovementAnimations();
        }
    }

    public void Tick(float _delta)
    {
        delta = _delta;

        // Check if the player is on the ground or not.
        onGround = OnGround();

        // Set the animator bool for OnGround
        animator.SetBool("OnGround", onGround);
    }

    public void HandleRolls()
    {
        if (!rollInput)
            return;

        float _vertical = vertical;
        float _horizontal = horizontal;

        _vertical = (moveAmount > 0.3f) ? 1 : 0;
        _horizontal = 0;


        if (_vertical != 0)
        {
            if (moveDirection == Vector3.zero)
                moveDirection = transform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = targetRotation;

            // Sets the params for rolling based on the animation curve
            animatorHook.InitForRoll();

            // Handle teh roll
            animatorHook.rootMotionMultiplier = rollSpeed;
        } else
        {
            animatorHook.rootMotionMultiplier = 1.3f;
        }

        // Snap the vertical and horitonal in place
        //if (lockOn == false)
        //{
        //    _vertical = (moveAmount > 0.3f) ? 1 : 0;
        //    _horizontal = 0;

        //}
        //else
        //{
        //    if (Mathf.Abs(_vertical) < 0.3f)
        //        _vertical = 0;

        //    if (Mathf.Abs(_horizontal) < 0.3f)
        //        _horizontal = 0;
        //}


        // Set the animator floats
        animator.SetFloat("Vertical", _vertical);
        animator.SetFloat("Horizontal", _horizontal);

        canMove = false;
        inAction = true;
        animator.CrossFade("Rolls", 0.2f);

     
    }

    public void DetectAction()
    {

        // If we can move then do nothing, cause when we are in action we cannot move
        if (canMove == false)
            return;

        // If no buttons are pres also do null.
        if (rb == false && rt == false && lt == false && lb == false)
            return;

        string targetAnimation = null;

        // Pass the state manager to get the action managert
        Action actionSlot = actionManager.GetActionSlot(this);

        // If there is no action reutn nothing
        if (actionSlot == null)
            return;

        targetAnimation = actionSlot.targetAnimation;

        if (string.IsNullOrEmpty(targetAnimation))
            return;

        canMove = false;
        inAction = true;
        animator.CrossFade(targetAnimation, 0.2f);

    }

    void HandleMovementAnimations()
    {
        animator.SetBool("Running", run);
        animator.SetFloat("Vertical", moveAmount, 0.4f, delta);
    }

    void HandleLockOnAnimations(Vector3 moveDirection)
    {
        // Get the location direction based on the world space move directino
        Vector3 relativeDirection = transform.InverseTransformDirection(moveDirection);

        // Get the horizontal and vertical movement from the x and the z
        float horizontal = relativeDirection.x * moveAmount;
        float vertical = relativeDirection.z * moveAmount;

        // Set the animator floats
        animator.SetFloat("Vertical", vertical, 0.2f, delta);
        animator.SetFloat("Horizontal", horizontal, 0.2f, delta);
    }

    public bool OnGround()
    {
        bool grounded = false;

        // Get the current origin of the player and raise it up by on ground fofset
        Vector3 origin = transform.position + (Vector3.up * toGround);

        // Create a down direction vector
        Vector3 downDirection = -Vector3.up;

        // Set the distance we should be shoting down the current ray + the offset
        float distance = toGround + 0.3f;


        RaycastHit hit;
        if (Physics.Raycast(origin, downDirection, out hit, distance, ignoreLayers))
        {
            // If the ray cast has hit comething
            // Set grounded to true
            grounded = true;

            // Set teh current target position to the current hit position
            Vector3 targetPosition = hit.point;

            // Set thr transform position to the target position
            transform.position = targetPosition;
        }

        // Retutn grounded status
        return grounded;
    }

    public void HandleTwoHanded()
    {
        animator.SetBool("TwoHandedWeapon", isTwoHanded);

        if (isTwoHanded)
        {
            actionManager.AssignWeaponActions(true);
        } else
        {
            actionManager.AssignWeaponActions(false);
        }
    }

}

using UnityEngine;

public class AnimatorHook : MonoBehaviour
{
    Animator animator;
    StateManager stateManager;

    public float rootMotionMultiplier;
    bool rolling;
    float rollTime;

    public void Init(StateManager _stateManager)
    {
        stateManager = _stateManager;
        animator = _stateManager.animator;
    }

    public void InitForRoll()
    {
        rolling = true;
        rollTime = 0;
    }

    public void CloseRoll()
    {
        if (rolling == false)
            return;

        rootMotionMultiplier = 1; // reset the root motion multiplier
        rollTime = 0; 
        rolling = false;
    }

    // Turn on root motion right before we play the attack animations
    private void OnAnimatorMove()
    {
        // If we can move then do nothing
        if (stateManager.canMove)
            return;

        // rest the drag because we will move super fast to the new lcoation 
        stateManager.playerRigidbody.drag = 0;

        // Sub division error handler
        if (rootMotionMultiplier == 0)
            rootMotionMultiplier = 1;

        // If we are rolling currently
        if (rolling == false)
        {
            // Get the delta between our current position and the root player
            Vector3 delta = animator.deltaPosition;

            // Alwasy reset the y position
            delta.y = 0;

            Vector3 velocityVector = (delta * rootMotionMultiplier) / stateManager.delta;

            // Assign the velocity to the player to rigid body
            stateManager.playerRigidbody.velocity = velocityVector;
        } else
        {
            rollTime += stateManager.delta / 0.65f;

            if (rollTime > 1)
            {
                rollTime = 1;
            }

            // Sample the curve to get the z value change
            float zValue = stateManager.rollCurve.Evaluate(rollTime);

            // Gives us the relative on the z axis for the roll
            Vector3 velocityVector = Vector3.forward * zValue;

            // Appli the reltive velocity to world space
            Vector3 relativeVelocity= transform.TransformDirection(velocityVector);

            // Conver with root motion multiplier
            Vector3 velocityVector2 = (relativeVelocity * rootMotionMultiplier); 

            stateManager.playerRigidbody.velocity = velocityVector2;
        }
         

    }
}

using UnityEngine;

public class AnimatorHook : MonoBehaviour
{
    Animator animator;
    StateManager stateManager;

    public void Init(StateManager _stateManager)
    {
        stateManager = _stateManager;
        animator = _stateManager.animator;
    }

    // Turn on root motion right before we play the attack animations
    private void OnAnimatorMove()
    {
        // If we can move then do nothing
        if (stateManager.canMove)
            return;

        // rest the drag because we will move super fast to the new lcoation 
        stateManager.playerRigidbody.drag = 0;

        float multiplier = 1;

        // Get the delta between our current position and the root player
        Vector3 delta = animator.deltaPosition;

        // Alwasy reset the y position
        delta.y = 0;

        Vector3 velocityVector = (delta * multiplier) / stateManager.delta;

        // Assign the velocity to the player to rigid body
        stateManager.playerRigidbody.velocity = velocityVector;

    }
}

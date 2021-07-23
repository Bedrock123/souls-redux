using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    public int index;
    public List<Transform> targets = new List<Transform>();
    public List<HumanBodyBones> humanoidBones = new List<HumanBodyBones>();

    Animator enemyAnimator;

    Transform targetTransform;

    private void Start()
    {
        // Get the eemeyu animator
        enemyAnimator = GetComponent<Animator>();

        // If they are not human then just cancel out
        if (enemyAnimator.isHuman == false)
            return;


        // Add in the the targets as the bones transforms
        for (int i = 0; i < humanoidBones.Count; i++)
        {
            targets.Add(enemyAnimator.GetBoneTransform(humanoidBones[i]));
        }
    }

    // Gets the position of the 
    public Transform GetTarget(bool negative = false)
    {


        // If there are no targets then just set the target to the transform
        if (targets.Count == 0)
            return transform;

        int targetIndex = index;

        // If we are going up in value then go to the next target
        if (negative == false)
        {

            // If index is less then the count then add one more
            if (index < targets.Count - 1)
            {
                index++;
            }
            else
            {
                // Othere wise reset
                targetIndex = 0;
            }
        } else
        {
            if (index <= 0)
            {
                index = targets.Count - 1;
                targetIndex = targets.Count - 1;
            } else
            {
                index--;
            }
        }

        targetTransform = targets[targetIndex];

        return targetTransform;
    }

    private void OnDrawGizmos()
    {
        if (targetTransform != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(targetTransform.position, .1f);
        }
    }

}

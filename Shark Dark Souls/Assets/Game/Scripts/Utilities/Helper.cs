using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{

    //[Range(-1, 1)] public float vertical;
    //[Range(-1, 1)] public float horizontal;

    //Animator animator;

    //public bool enableRootMotion;
    //public bool useItem;
    //public bool interacting;
    //public bool lockon;

    //public bool playAnimation;
    //public string[] oh_attacks;
    //public string[] tw_attacks;


    //public bool twoHanded;


    //// Start is called before the first frame update
    //void Start()
    //{
    //    animator = GetComponent<Animator>() ;
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //    enableRootMotion = !animator.GetBool("CanMove");
    //    animator.applyRootMotion = enableRootMotion;

    //    if (!lockon)
    //    {
    //        horizontal = 0;
    //        vertical = Mathf.Clamp01(vertical);
    //    }

    //    animator.SetBool("LockOn", true);

    //    if (enableRootMotion)
    //        return;

    //    interacting = animator.GetBool("Interacting");

    //    if (useItem == true)
    //    {
    //        animator.Play("use_item");
    //        useItem = false;
    //    }

    //    if (interacting)
    //    {
    //        playAnimation = false;
    //        vertical = Mathf.Clamp(vertical, 0, 0.5f);
    //    }

    //    animator.SetBool("TwoHandedWeapon", twoHanded);

    //    if (playAnimation)
    //    {
    //        string targetAnimation;

    //        if (!twoHanded)
    //        {
    //            int random = Random.Range(0, oh_attacks.Length);
    //            targetAnimation = oh_attacks[random];

                
    //        } else
    //        {
    //            int random = Random.Range(0, tw_attacks.Length);
    //            targetAnimation = tw_attacks[random];

    //        }

    //        if (vertical > 0.5f)
    //        {
    //            targetAnimation = "oh_attack_3";
    //        }

    //        vertical = 0;
            
    //        animator.CrossFade(targetAnimation, 0.4f);
    //        playAnimation = false;
    //    }

    //    animator.SetFloat("Vertical", vertical);
    //    animator.SetFloat("Horizontal", horizontal);



        
    //}
    public void OpenDamageColliders() { }
    public void CloseDamageColliders() { }
}

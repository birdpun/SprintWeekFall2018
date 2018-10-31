using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerWhale : MonoBehaviour
{
    public Animator animator;

    public void Jump()
    {
        animator.SetBool("jumping", true);
        Debug.Log("Jump");
    }
}

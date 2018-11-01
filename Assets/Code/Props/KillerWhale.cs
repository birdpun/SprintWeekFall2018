using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerWhale : MonoBehaviour
{
    public Animator animator;

    public void Jump()
    {
        animator.Play("Jump");
    }

    public void StopJump()
    {
        //if(animator.GetBool("jumping") == true)
        {
            Destroy(gameObject);
            Instantiate(GameManager.Whale, new Vector3(54.6f, -2f, 3f), Quaternion.identity);
        }
    }
}

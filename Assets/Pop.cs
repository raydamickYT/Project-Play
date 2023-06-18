using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pop : MonoBehaviour
{
    float Timer;
    public Animator animator;

    // if object touched, start destroy animation
    void OnCollisionEnter2D(Collision2D other){
        if (other.gameObject.CompareTag("Player")){
            animator.SetFloat("Touched", 1);
            Timer += Time.deltaTime;
        }
    }

}

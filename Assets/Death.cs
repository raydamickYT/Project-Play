using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    public GameObject player;
    public Animator animator;
    public Animator Camera;
    void OnTriggerEnter2D (Collider2D other){
        if (other.gameObject.CompareTag("Player")){
            Destroy(other.gameObject);
            animator.SetBool("Dead", true);
        }
            
        
    }
}

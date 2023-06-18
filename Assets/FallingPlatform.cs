using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private float fallDelay = 0.5f;
    private float DestroyDelay = 3f;
    [SerializeField] Pop platformScript;
    Vector3 position;
    PlayerMovement script;
    [SerializeField] GameObject player;

    private void Awake()
    {
        position = transform.position;
        script = player.GetComponent<PlayerMovement>();
    }

    [SerializeField] private Rigidbody2D rb;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !script.isLicking)
        {
            StartCoroutine(Fall(collision));
        }
    }

    private IEnumerator Fall(Collision2D collision)
    {
        yield return new WaitForSeconds(fallDelay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        StartCoroutine(pos(collision));
    }

    private IEnumerator pos(Collision2D collision)
    {
        yield return new WaitForSeconds(DestroyDelay);
        platformScript.animator.SetFloat("Touched", 0);
        transform.position = position;
        rb.bodyType = RigidbodyType2D.Static;
    }

}

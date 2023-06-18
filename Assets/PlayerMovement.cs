using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //add animator
    public Animator animator;
    //create the variables needed to move the player
    private float jumpingPower = 16f;// the higher the number the higher the jump
    private float speed = 10f;
    float negative, positive, previousPosition, currentPosition; //the higher the number the higher the speed
    private float Timer, timerCheck, horizontal;
    public bool temp = false, isLicking = false;
    private bool isFacingRight = true; //bool to check if the player is facing right
    private bool isIce, isSliding, isMovingRight, isMovingLeft, Slowed;

    private Vector3 respawnPoint;
    public GameObject fallDetector;
    private System.Random rand = new System.Random();

    //reference script

    //private fields to reference the rigidbody, groundcheck and groundlayer
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask SpecialLayer;

    private void Awake() //oonly gets ran once before the game starts
    {
        respawnPoint = transform.position; //save the position before the game starts
    }

    // Update is called once per frame
    void Update()
    {
        animate();
        if (isIce && Mathf.Abs(horizontal) == 0f)
        {
            isSliding = true;
        }
        else if (isIce && Mathf.Abs(horizontal) != 0f)
        {
            isSliding = false;
        }

        if (Input.GetButtonDown("Jump") && IsGrounded()) // if the jump button (space) is pressed and IsGrounded is true, start this loop.
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower); //set the rigidbody velocity.y to jumping power

        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f) //if the jump button is released and the Y velocity is bigger than 0
        {
            //change the rigidbody velocity to velocity.y * .5 (so the character starts to fall)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        Flip(); //reference to Flip function
        timer();

        //will make sure the FallDetector empty follows the x position of the player, to prevent making a HUGE box collider under the level
        fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y);
    }

    void animate()
    {
        //set newHorizontal float made in animator to input from horizontal keys
        animator.SetFloat("Speed", Mathf.Abs(horizontal));
        if (!IsGrounded())
        {
            if (Input.GetButtonDown("Jump") && !isLicking)
            {
                animator.SetFloat("JumpFL", 0);
                animator.SetFloat("Lick", 1);
                isLicking = true;
            }
            else if (Input.GetButtonDown("Jump") && isLicking)
            {
                animator.SetFloat("JumpFL", 1);
                animator.SetFloat("Lick", 0);
                isLicking = false;
            }
            else if (!isLicking)
            {
                animator.SetFloat("JumpFL", 1);
            }
        }
        else if (IsGrounded())
        {
            animator.SetFloat("JumpFL", 0);

        }
    }

    private void FixedUpdate()  //called every fixed framerate frame.
    {
        movement();
    }

    void isMoving() //word naar verwezen in movement()
    {
        if (horizontal == 1) //als movement key is ingedrukt, dan bewegen we!
        {
            isMovingRight = true;
            isMovingLeft = false;
        }
        else if (horizontal == -1) //als movement key is ingedrukt dan bewegen we
        {
            isMovingLeft = true;
            isMovingRight = false;
        }
        else if (previousPosition == currentPosition) //als de vorige positie gelijk is aan de huidige positie dan bewgen we niet
        {
            isMovingLeft = false;
            isMovingRight = false;
        }
    }
    void movement() //wordt naar verwezen in FixedUpdate
    {
        currentPosition = transform.position.x;
        isMoving();
        if (isSliding)
        {
            if (previousPosition == currentPosition)
            {
                horizontal = Input.GetAxisRaw("Horizontal"); /// om er voor te zorgen dat we toch kunnen bewegen als we stilstaan
                                                             /// heb ik toegevoegd dat hij ook de input pakt als we geen keys hebben ingedrukt
            }
            if (isMovingRight || horizontal == 1)
            {
                positive = positive * 0.95f; //maak positive kleiner met stappen van 5 procent (exponentiele daling)
                rb.velocity = new Vector2(positive, rb.velocity.y);
                if (positive > 1 && horizontal == 0 && !temp) //als pos groter is dan 0.3 EN  er geen keys zijn ingedrukt(nietwaar)
                {
                    horizontal = 0;
                    temp = Input.GetKey(KeyCode.RightArrow); //check of de arrow key ingedrukt word
                }
                else if (positive < 1 || temp) /// als positive kleiner is dan o.3 en horizontaal wel is ingedrukt (welwaar)
                                               /// ik gebruik hiervoor een temp variabele zodat ik apart van mijn main movement var kan checken of de key word ingedrukt
                {
                    temp = false; //reset temp var
                    horizontal = Input.GetAxisRaw("Horizontal"); //get input and parce the return value into our horizontal variable
                }
            }
            else if (isMovingLeft || horizontal == -1)
            {
                negative = negative * 0.95f;
                rb.velocity = new Vector2(negative, rb.velocity.y);
                if (negative < -1 && horizontal == 0 && !temp)
                {
                    horizontal = 0;
                    temp = Input.GetKey(KeyCode.LeftArrow);
                }
                else if (negative > -1 || temp)
                {
                    temp = false;
                    horizontal = Input.GetAxisRaw("Horizontal"); //get input and parce the return value into our horizontal variable
                }
            }
        }
        else if (!isSliding)
        {
            horizontal = Input.GetAxisRaw("Horizontal"); //get input and parce the return value into our horizontal variable
            positive = speed;
            negative = -1 * speed;
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y); //set the x component of the rigbody velocity to horizontal var * speed var
        }
        previousPosition = currentPosition;
    }
    private void timer() ///this function can be re used for timers
    {
        if (Timer >= 1)
        {
            Timer += Time.deltaTime;
            if (Timer >= timerCheck)
            {
                Slowed = false;
                speed = 10f;
                negative = speed * -1;
                positive = speed;
                Timer = 0;
            }
        }
    }


    //function die de falldetection triggered
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "FallDetector")
        {
            transform.position = respawnPoint; //if we collide with the fall detection, we teleport back to our saved respawn point
        }
        if (collision.tag == "CheckPoint")
        {
            respawnPoint = transform.position;
            Destroy(collision.gameObject);
        }
        if (collision.tag == "Gum")
        {
            Slowed = true;
            positive = 2f;
            negative = -2f;
            speed = speed * 0.2f;
            Timer += 1;
            timerCheck = 4;
            Destroy(collision.gameObject);
        }
    }

    //function to check collsion with other objects.
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            if (isLicking)
            {
                if (!Slowed)
                {
                    positive = 10f;
                    negative = -10f;
                    speed = 10f;
                }
                isIce = true;
            }
            else if (!isLicking) transform.position = respawnPoint;
        }
        //loop for speed boost
        if (collision.gameObject.CompareTag("SpeedBooster")) //if player touches plane tagged as speedbooster
        {
            if (isLicking)
            {
                if (!Slowed)
                {
                    speed = 20f; //speed doubles
                }
            }
            else if (!isLicking) transform.position = respawnPoint;
        }
        //loop for special pad
        if (collision.gameObject.CompareTag("SpecialPad")) //if player touches plane tagged as speedbooster
        {
            if (isLicking)
            {
                if (!Slowed)
                {
                    speed = 20f; //speed doubles
                }
            }
            else if (!isLicking) transform.position = respawnPoint;
        }
        //loop for ending the speedboost
        if (collision.gameObject.CompareTag("Ground")) //if player touches a plane tagged as ground
        {
            if (!isLicking)
            {
                if (!Slowed)
                {
                    speed = 10f; //speed doubles
                }
            }
            else if (isLicking) transform.position = respawnPoint;
        }

        //loop for ending the speedboost
        if (collision.gameObject.tag == "JumpPad") //if player touches a plane tagged as ground
        {
            if (!isLicking)
            {
                if (!Slowed)
                {
                    speed = 10f; //speed doubles
                }
            }
            else if (isLicking) transform.position = respawnPoint;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            isIce = false;
            isSliding = false;
        }
    }

    private bool IsGrounded() //check if the player is grounded
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip() //check if the cube is facing the right way
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight; //if either is true, the var isFacingRight is set to false
            Vector3 localScale = transform.localScale; // get the localscale of the character
            localScale.x *= -1f; //the local scale is flipped
            transform.localScale = localScale; //and assign it back to the character
        }
    }
}

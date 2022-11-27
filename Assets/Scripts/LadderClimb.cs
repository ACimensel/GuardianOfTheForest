using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderClimb : MonoBehaviour
{
    private float vertical;
    private float speed = 8f; //TODO expose to inspector
    private bool isTouchingLadder = false;
    private bool isClimbing = false;
    private Rigidbody2D rb;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        vertical = Input.GetAxisRaw("Climb");

        if (isTouchingLadder && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
            gameObject.layer = LayerMask.NameToLayer("Climbing");
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, vertical * speed);
        }
        else
        {
            rb.gravityScale = 3f; // TODO get this scale onStart
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isTouchingLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isTouchingLadder = false;
            isClimbing = false;
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Guardian : MonoBehaviour
{
    [SerializeField] GameObject boltPrefab;
    [SerializeField] GameObject teleportPrefab;
    [SerializeField] GameObject lastRespawnLocation;
    [SerializeField] AudioClip[] meleeSounds;
    [SerializeField] Transform groundCheck; // A position marking where to check if the player is grounded.
    [SerializeField] LayerMask whatIsGround; // A mask determining what is ground to the character
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] GameManager gameManager;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float flashTime = 0.2f;
    [SerializeField] float hitStaggerTime = 0.2f;
    [SerializeField] float invulnerabilityTime = 3f;
    [SerializeField] public float rangedCooldownTime = 3f;
    [SerializeField] int currentHealth = 4;
    [SerializeField] int maxHealth = 4;
    [SerializeField] int meleeDamage = 10;

    public TextMeshProUGUI orbCountText;
    public HealthBar healthBar;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public bool isDamageEnabled = true;
    public bool isRangedEnabled = true;

    [Header("Events")] [Space]
    public UnityEvent OnLandEvent;

    private Renderer rend;
    private Color startColor;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 localScale;
    private float checkRadius = .1f; // Radius of the overlap circle to determine if grounded
    private float dirX = 0f;
    private float fallTolerance = -3f; // used to not play falling animation when walking down slopes
    private float attackStaggerTime = 0.5f;
    private bool isMovementEnabled = true;
    private bool isGrounded = false; // Whether or not the player is grounded.
    private bool facingRight = true;
    private Coroutine attackCoroutine = null;
    private Vector3 velocity = Vector3.zero;
    private int orbCount = 0;

    [Header("Jumping Parameters")]
    [Space]
    [SerializeField] float jumpForce = 15f;
    [SerializeField] float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Wall Jumping Parameters")]
    [Space]
    [SerializeField] Transform frontCheck;
    [SerializeField] float wallSlidingSpeed = -0.3f;
    [SerializeField] float xWallForce;
    [SerializeField] float yWallForce;
    [SerializeField] float delayMove;
    private bool isTouchingFront = false;
    private bool wallSliding = false;
    private bool wallJumping;

    // var myQueue = new Queue<GameObject>();
    // myQueue.Enqueue(5);
    // V = myQueue.Dequeue();  // returns 100


    public enum AttackStates
    {
        NONE = 0,
        MELEE1,
        MELEE2,
        MELEE3,
        RANGED,
    }


    void Awake()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        localScale = transform.localScale;

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
        
        orbCountText.text = orbCount.ToString();
    }


    void Update()
    {
        if (isMovementEnabled)
        {
            dirX = Input.GetAxisRaw("Horizontal") * moveSpeed;

            if (isGrounded)
                coyoteTimeCounter = coyoteTime;
            else
                coyoteTimeCounter -= Time.deltaTime;

            if (Input.GetButtonDown("Jump") && coyoteTimeCounter > 0f)
            {
                Debug.Log("JUMP");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetBool("isJumping", true);

                StartCoroutine("SetIsGroundedFalseLate");
            }

            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            { // long press jump
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                coyoteTimeCounter = 0f;
            }
        }

        SetAnimationState();
    }


    void FixedUpdate()
    {
        if (isMovementEnabled)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(dirX, rb.velocity.y);
            // And then smoothing it out and applying it to the character
            if (isGrounded)
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, 0.03f);
            else
                rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, 0.15f);
        }

        // Check if guardian is on the ground and if just landed
        bool wasGrounded = isGrounded;
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, checkRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isGrounded = true;

                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();
                }
            }
        }

        // Is guardian wall sliding? play right animation
        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, whatIsGround);
        if (isTouchingFront && !isGrounded)
        {
            wallSliding = true;
            animator.SetBool("isWallSliding", true);
        }
        else
        {
            wallSliding = false;
            animator.SetBool("isWallSliding", false);
        }

        // if sliding, limit y velocity
        if (wallSliding)
        {
            rb.velocity = new Vector2(0f, Mathf.Clamp(rb.velocity.y, wallSlidingSpeed, float.MaxValue));
        }

        // wall jump
        if (wallJumping)
        {
            if (facingRight)
            {
                rb.AddForce(new Vector3(-xWallForce, yWallForce, 0f));
                facingRight = false;
            }
            else
            {
                rb.AddForce(new Vector3(xWallForce, yWallForce, 0f));
                facingRight = true;
            }
            wallJumping = false;
        }
    }


    void LateUpdate()
    {
        CheckWhereToFace();
    }


    public void OnLanding()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);

        Debug.Log("LAND");
        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", false);
    }


    void SetAnimationState()
    {
        float absX = Mathf.Abs(dirX);
        animator.SetFloat("Speed", absX);

        // Slide
        if (Input.GetButton("Slide"))
        {
            animator.SetBool("isSliding", true);
            Debug.Log("SLIDE WEEE");
        }
        else
        {
            animator.SetBool("isSliding", false);
        }

        // Transition from jumping to falling animation
        if (rb.velocity.y < fallTolerance)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
        }

        // Revive
        if (Input.GetButtonDown("Revive") && animator.GetBool("isDead"))
        {
            Reset();
        }

        // Use teleport skill
        Vector2 player = this.gameObject.transform.position;
        if (Input.GetButtonDown("Skill_Teleport"))
        {
            Debug.Log("TELEPORT");
            GameObject teleport = Instantiate(teleportPrefab, new Vector3(player.x, player.y - 0.309f, 0f), Quaternion.identity);
        }

        // Ranged attack
        if (Input.GetButtonDown("Ranged Attack") && animator.GetInteger("nextAttackState") == (int)AttackStates.NONE && !wallSliding && isRangedEnabled)
        {
            DisableMovement(false);

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine("WaitForAttackFinish");

            animator.SetInteger("nextAttackState", (int)AttackStates.RANGED);
            animator.SetTrigger("RangedAttack");

            if (facingRight)
            {
                GameObject bolt = Instantiate(boltPrefab, new Vector3(player.x + 0.4f, player.y + 0.2f, 0f), Quaternion.identity);
                bolt.SendMessage("SetVelocity", "right");
            }
            else
            {
                GameObject bolt = Instantiate(boltPrefab, new Vector3(player.x - 0.4f, player.y + 0.2f, 0f), Quaternion.identity);
                Vector3 scale = bolt.transform.localScale;
                bolt.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                bolt.SendMessage("SetVelocity", "left");
            }

            // cooldown
            isRangedEnabled = false;
            StartCoroutine("RangedCooldown");
        }

        // Melee attack
        if (Input.GetButtonDown("Melee Attack") && animator.GetInteger("nextAttackState") != (int)AttackStates.RANGED && animator.GetInteger("nextAttackState") != (int)AttackStates.MELEE3 && !wallSliding)
        {
            DisableMovement(false);

            AnimatorClipInfo[] animCurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            string animationName = animCurrentClipInfo[0].clip.name;

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine("WaitForAttackFinish");

            if (animationName == "Guardian_melee1")
            {
                animator.SetInteger("nextAttackState", (int)AttackStates.MELEE2);
                animator.SetTrigger("MeleeAttack1");
                animator.SetTrigger("shakeCamera");
            }
            else if (animationName == "Guardian_melee2")
            {
                animator.SetInteger("nextAttackState", (int)AttackStates.MELEE3);
                animator.SetTrigger("MeleeAttack2");
            }
            else if (animationName != "Guardian_melee3")
            {
                animator.SetInteger("nextAttackState", (int)AttackStates.MELEE1);
                animator.SetTrigger("MeleeAttack1");
            }

            //TODO need to refactor how melee hits work
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.GetComponent<Deer>() != null)
                {
                    enemy.GetComponent<Deer>().TakeDamage(meleeDamage);
                }
                else if (enemy.GetComponent<Crow>() != null)
                {
                    enemy.GetComponent<Crow>().TakeDamage(meleeDamage);
                }

            }
        }

        // Wall jump
        if (Input.GetButtonDown("Jump") && wallSliding)
        {
            wallJumping = true;
            StartCoroutine("DisableThenEnableAirMove");
        }
    }


    public void PlayMeleeAttackSounds()
    {
        AudioSource audioSrc = GetComponent<AudioSource>();
        audioSrc.clip = meleeSounds[Random.Range(0, meleeSounds.Length)];
        audioSrc.Play();
    }


    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


    void CheckWhereToFace()
    {
        if (dirX > 0)
            facingRight = true;
        else if (dirX < 0)
            facingRight = false;

        if (((facingRight) && (localScale.x < 0)) || ((!facingRight) && (localScale.x > 0)))
            localScale.x *= -1;

        transform.localScale = localScale;
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        string layerName = LayerMask.LayerToName(col.gameObject.layer);
        Debug.Log("Hit by layer: " + layerName);

        if (layerName == "EnemyAttack" && isDamageEnabled)
        {
            isDamageEnabled = false;
            DisableMovement();

            Debug.Log("DAMAGED");
            currentHealth--;
            healthBar.SetHealth(currentHealth);


            if (currentHealth > 0)
            {
                if (col.gameObject.transform.position.x < this.gameObject.transform.position.x)
                    rb.AddForce(new Vector2(300f, 100f));
                else
                    rb.AddForce(new Vector2(-300f, 100f));

                StartCoroutine("BecomeInvulnerable");
                StartCoroutine("EnableMovementDelayed");
            }
            else
            {
                Die();
            }
        }
        else if (layerName == "Drop")
        {
            Die();
        }
        else if (layerName == "Essence")
        {
            Reset();
        }
    }

    void OnCollisionEnter2D(Collision2D col){
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        if (layerName == "Orb"){
            orbCount += 5;
            orbCountText.text = orbCount.ToString();
        }

    }

    void Die()
    {
        rb.velocity = Vector2.zero;
        isMovementEnabled = false;
        dirX = 0;

        SetAllCollidersAndRbStatus(false);
        animator.SetBool("isDead", true);

        currentHealth = 0;
        healthBar.SetHealth(currentHealth);
        gameManager.GameOver();
    }


    void Reset()
    {
        isDamageEnabled = true;
        animator.SetBool("isDead", false);

        Vector3 treePos = lastRespawnLocation.transform.position;
        this.gameObject.transform.position = new Vector3(treePos.x + 0.5f, treePos.y + 1.5f, treePos.z);

        EnableMovement();
        SetAllCollidersAndRbStatus(true);

        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
        gameManager.Revive();
    }


    void DisableMovement(bool stagger = true)
    {
        animator.SetBool("isStaggered", stagger);

        isMovementEnabled = false;
        dirX = 0;

        if (isGrounded)
            rb.velocity = new Vector2(0f, rb.velocity.y);
    }


    void EnableMovement(bool wasStaggered = true)
    {
        if (wasStaggered)
            animator.SetBool("isStaggered", false);
        isMovementEnabled = true;
    }


    void SetAllCollidersAndRbStatus(bool active)
    {
        GetComponent<Rigidbody2D>().isKinematic = !active;

        foreach (Collider2D c in GetComponents<Collider2D>())
            c.enabled = active;
    }


    IEnumerator BecomeInvulnerable()
    {
        animator.SetTrigger("Hurt");
        Coroutine flash = StartCoroutine("Flash");

        yield return new WaitForSeconds(invulnerabilityTime);

        StopCoroutine(flash);
        startColor.a = 1f;
        rend.material.color = startColor;
        isDamageEnabled = true;
    }


    IEnumerator Flash()
    {
        startColor.a = 0.6f;
        float delta = 0.2f;

        while (true)
        {
            delta *= -1;
            startColor.a += delta;
            rend.material.color = startColor;
            yield return new WaitForSeconds(flashTime);
        }
    }


    IEnumerator EnableMovementDelayed()
    {
        yield return new WaitForSeconds(hitStaggerTime);

        EnableMovement();
    }


    IEnumerator WaitForAttackFinish()
    {
        yield return new WaitForSeconds(attackStaggerTime);

        EnableMovement(false);
        animator.SetInteger("nextAttackState", (int)AttackStates.NONE);
    }


    IEnumerator SetIsGroundedFalseLate()
    {
        yield return new WaitForSeconds(0.02f);

        isGrounded = false;
    }


    IEnumerator DisableThenEnableAirMove()
    {
        DisableMovement(false);

        yield return new WaitForSeconds(delayMove);

        EnableMovement(false);
    }

    IEnumerator RangedCooldown()
    {
        yield return new WaitForSeconds(rangedCooldownTime);
        isRangedEnabled = true;
    }
}
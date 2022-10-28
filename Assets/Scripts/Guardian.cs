using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Guardian : MonoBehaviour
{
    [SerializeField] GameObject boltPrefab;
    [SerializeField] GameObject teleportPrefab;
    [SerializeField] GameObject lastRespawnLocation;
    [SerializeField] AudioClip[] meleeSounds;
    [SerializeField] private Transform m_GroundCheck; // A position marking where to check if the player is grounded.
    [SerializeField] private LayerMask m_WhatIsGround; // A mask determining what is ground to the character
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 800f;
    [SerializeField] float flashTime = 0.2f;
    [SerializeField] float hitStaggerTime = 0.2f;
    [SerializeField] float invulnerabilityTime = 3f;
    [SerializeField] float rangedCooldownTime = 3f;
    [SerializeField] int currentHealth = 4;
    [SerializeField] int maxHealth = 4;
    [SerializeField] int meleeDamage = 10;

    public HealthBar healthBar;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;

    // var myQueue = new Queue<GameObject>();
    // myQueue.Enqueue(5);
    // V = myQueue.Dequeue();  // returns 100

    const float k_GroundedRadius = .1f; // Radius of the overlap circle to determine if grounded
    private Renderer rend;
    private Color startColor;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 localScale;
    private float dirX = 0f;
    private float fallTolerance = -3f;
    private float attackStaggerTime = 0.5f;
    private bool isMovementEnabled = true;
    private bool m_Grounded = false; // Whether or not the player is grounded.
    private bool facingRight = true;
    private bool isDamageEnabled = true;
    private bool isRangedEnabled = true;
    private Coroutine attackCoroutine = null;

    enum AttackStates
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
    }

    void Update()
    {
        if (isMovementEnabled)
        {
            dirX = Input.GetAxisRaw("Horizontal") * moveSpeed;

            if (Input.GetButtonDown("Jump") && m_Grounded)
            {
                Debug.Log("JUMP");
                rb.AddForce(Vector2.up * jumpForce);
                animator.SetBool("isJumping", true);

                StartCoroutine("SetIsGroundedFalseLate");
                // m_Grounded = false;
            }
        }

        SetAnimationState();
    }

    void FixedUpdate()
    {
        if (isMovementEnabled)
            rb.velocity = new Vector2(dirX, rb.velocity.y);


        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;

                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();
                }
            }
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

        if (Input.GetButtonDown("Revive") && animator.GetBool("isDead"))
        {
            Reset();
        }

        Vector2 player = this.gameObject.transform.position;
        if (Input.GetButtonDown("Skill_Teleport"))
        {
            Debug.Log("TELEPORT");
            GameObject teleport = Instantiate(teleportPrefab, new Vector3(player.x, player.y - 0.309f, 0f), Quaternion.identity);
        }

        AnimatorClipInfo[] animCurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string animationName = animCurrentClipInfo[0].clip.name;

        if (Input.GetButtonDown("Ranged Attack") && animator.GetInteger("nextAttackState") == (int)AttackStates.NONE && isRangedEnabled)
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

        if (Input.GetButtonDown("Melee Attack") && animator.GetInteger("nextAttackState") != (int)AttackStates.RANGED && animator.GetInteger("nextAttackState") != (int)AttackStates.MELEE3)
        {
            DisableMovement(false);

            // TODO move into its own script which gets called in attack states
            AudioSource audioSrc = GetComponent<AudioSource>();
            audioSrc.clip = meleeSounds[Random.Range(0, meleeSounds.Length)];
            audioSrc.Play();

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine("WaitForAttackFinish");

            if (animationName == "Guardian_melee1")
            {
                animator.SetInteger("nextAttackState", (int)AttackStates.MELEE2);
                animator.SetTrigger("MeleeAttack1");
            }
            else if (animationName == "Guardian_melee2")
            {
                animator.SetInteger("nextAttackState", (int)AttackStates.MELEE3);
                animator.SetTrigger("MeleeAttack2");
            }
            else
            {
                animator.SetInteger("nextAttackState", (int)AttackStates.MELEE1);
                animator.SetTrigger("MeleeAttack1");
            }

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Deer>().TakeDamage(meleeDamage);
            }
        }
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

        if (layerName == "Enemy" && isDamageEnabled)
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

    void Die()
    {
        rb.velocity = Vector2.zero;
        isMovementEnabled = false;
        dirX = 0;

        SetAllCollidersAndRbStatus(false);
        animator.SetBool("isDead", true);

        currentHealth = 0;
        healthBar.SetHealth(currentHealth);
    }

    void Reset()
    {
        animator.SetBool("isDead", false);

        Vector3 treePos = lastRespawnLocation.transform.position;
        this.gameObject.transform.position = new Vector3(treePos.x + 0.5f, treePos.y + 1.5f, treePos.z);

        EnableMovement();
        SetAllCollidersAndRbStatus(true);

        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
    }

    void DisableMovement(bool stagger = true)
    {
        if (stagger)
            animator.SetBool("isStaggered", true);

        isMovementEnabled = false;
        dirX = 0;

        if (m_Grounded)
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

        m_Grounded = false;
    }

    IEnumerator RangedCooldown()
    {
        yield return new WaitForSeconds(rangedCooldownTime);
        isRangedEnabled = true;
    }

}
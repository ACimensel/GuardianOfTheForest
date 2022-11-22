﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Guardian : MonoBehaviour
{
    [SerializeField] GameObject boltPrefab;
    [SerializeField] GameObject teleportPrefab;
    [SerializeField] GameObject lastRespawnLocation;
    [SerializeField] GameObject tilesToTurnOff;
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
    [SerializeField] public float teleportCooldownTime = 5f;
    [SerializeField] int meleeDamage = 10;

    public TextMeshProUGUI orbCountText;
    public HealthBar healthBar;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public bool isDamageEnabled = true;
    public bool isRangedEnabled = true;
    public bool isTeleportEnabled = true;
    public bool isMovementEnabled = true;
    public Queue<GameObject> teleportQueue = new Queue<GameObject>();

    private PersistantData PD;
    private Renderer rend;
    private Color startColor;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 localScale;
    private float checkRadius = .1f; // Radius of the overlap circle to determine if grounded
    private float dirX = 0f;
    private float fallTolerance = -3f; // used to not play falling animation when walking down slopes
    private float attackStaggerTime = 0.5f;
    private bool isGrounded = false; // Whether or not the player is grounded.
    private bool facingRight = true;
    private Coroutine attackCoroutine = null;
    private Vector3 velocity = Vector3.zero;
    private int orbCount = 0;

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;

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

    [Header("Ground Sliding Parameters")]
    [Space]
    [SerializeField] float slideSpeed = 30f;
    [SerializeField] float slideDuration = 0.5f;
    [SerializeField] public float slideCooldown = 2f;
    private Coroutine slideCoroutine = null;
    private float coroutineInvervalTime = 0.1f;
    public float slideCooldownCounter = 0f;


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
        PD = GameObject.Find("PersistantData").GetComponent<PersistantData>();
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        localScale = transform.localScale;

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        orbCountText.text = orbCount.ToString();
        healthBar.InitializeHealthBar(PD.guardianMaxHealth, PD.guardianCurrentHealth);
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

        // play right animation if guardian wall sliding
        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, whatIsGround);
        if (isTouchingFront && !isGrounded && dirX != 0f && !animator.GetBool("isJumping"))
        // if (isTouchingFront && !isGrounded && !animator.GetBool("isJumping"))
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


    IEnumerator SlideForXTime(float _dirX)
    {
        isMovementEnabled = false;
        isDamageEnabled = false;
        animator.SetBool("isSliding", true);
        gameObject.layer = LayerMask.NameToLayer("Sliding");

        BoxCollider2D bc2d = GetComponent<BoxCollider2D>();
        bc2d.enabled = false;

        float runningTime = 0f;
        bool isNotTouchingAnything = true; // TODO extend slide if top collider touching ground (use circleoverlap?)

        while (runningTime < slideDuration && isNotTouchingAnything)
        {
            if (!isGrounded) break;

            rb.velocity = (_dirX > 0f) ? new Vector2(slideSpeed, rb.velocity.y) : new Vector2(-slideSpeed, rb.velocity.y);

            yield return new WaitForSeconds(coroutineInvervalTime);
            runningTime += coroutineInvervalTime;
        }

        bc2d.enabled = true;
        isMovementEnabled = true;
        isDamageEnabled = true;
        animator.SetBool("isSliding", false);
        gameObject.layer = LayerMask.NameToLayer("Player");
        slideCoroutine = null;
    }

    void SetAnimationState()
    {
        float absX = Mathf.Abs(dirX);
        animator.SetFloat("Speed", absX);


        // Slide
        if (slideCooldownCounter > 0f)
        {
            slideCooldownCounter -= Time.deltaTime;
        }

        Debug.Log("slideCooldownCounter: " + slideCooldownCounter);
        if (Input.GetButtonDown("Slide") && slideCoroutine == null && isGrounded && isMovementEnabled && slideCooldownCounter <= 0f)
        {

            if (dirX != 0f)
            {
                slideCoroutine = StartCoroutine(SlideForXTime(dirX));
                slideCooldownCounter = slideCooldown;
                Debug.Log("RESETING:");
                Debug.Log("RESET: " + slideCooldownCounter);
            }
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
        if (Input.GetButtonDown("Skill_Teleport") && isGrounded && teleportQueue.Count < 2 && isMovementEnabled && isTeleportEnabled)
        {
            GameObject newTeleport = Instantiate(teleportPrefab, new Vector3(player.x, player.y - 0.24f, 0f), Quaternion.identity);
            teleportQueue.Enqueue(newTeleport);

            if (teleportQueue.Count == 2)
            {
                GameObject oldTeleport = teleportQueue.Peek();

                oldTeleport.GetComponent<TeleportSkill>().UpdateTeleportPair(true, newTeleport.transform.position);
                newTeleport.GetComponent<TeleportSkill>().UpdateTeleportPair(true, oldTeleport.transform.position);
            }
        }

        // Ranged attack
        if (Input.GetButtonDown("Ranged Attack") && animator.GetInteger("nextAttackState") == (int)AttackStates.NONE && !wallSliding && isRangedEnabled && slideCoroutine == null)
        {
            DisableMovement(false);

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            attackCoroutine = StartCoroutine("WaitForAttackFinish");

            animator.SetInteger("nextAttackState", (int)AttackStates.RANGED);
            animator.SetTrigger("RangedAttack");

            // TODO instantiate at the end of cast animation
            if (facingRight)
            {
                GameObject bolt = Instantiate(boltPrefab, new Vector3(player.x + 0.4f, player.y + 0.2f, 0f), Quaternion.identity);
                bolt.SendMessage("SetVelocity", "right");
            }
            else
            {
                GameObject bolt = Instantiate(boltPrefab, new Vector3(player.x - 0.4f, player.y + 0.2f, 0f), Quaternion.identity);
                bolt.SendMessage("SetVelocity", "left");
            }

            // cooldown
            isRangedEnabled = false;
            StartCoroutine("RangedCooldown");
        }

        // Melee attack
        if (Input.GetButtonDown("Melee Attack") && animator.GetInteger("nextAttackState") != (int)AttackStates.RANGED && animator.GetInteger("nextAttackState") != (int)AttackStates.MELEE3 && !wallSliding && slideCoroutine == null)
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
                else if (enemy.GetComponent<A1Boss>() != null)
                {
                    enemy.GetComponent<A1Boss>().TakeDamage(meleeDamage);
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
            PD.guardianCurrentHealth--;
            healthBar.SetHealth(PD.guardianCurrentHealth);


            if (PD.guardianCurrentHealth > 0)
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

    void OnCollisionEnter2D(Collision2D col)
    {
        string layerName = LayerMask.LayerToName(col.gameObject.layer);

        if (layerName == "Orb")
        {
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

        PD.guardianCurrentHealth = 0;
        healthBar.SetHealth(PD.guardianCurrentHealth);
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

        PD.guardianCurrentHealth = PD.guardianMaxHealth;
        healthBar.SetHealth(PD.guardianCurrentHealth);
        gameManager.Revive();

        tilesToTurnOff.SetActive(false);
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


    public void DequeueTeleport()
    {
        if (teleportQueue.Count > 0)
        {
            GameObject oldTeleport = teleportQueue.Dequeue();
            oldTeleport.GetComponent<TeleportSkill>().UpdateTeleportPair(false, Vector3.zero);

            if (teleportQueue.Count == 1)
                teleportQueue.Peek().GetComponent<TeleportSkill>().UpdateTeleportPair(false, Vector3.zero);
            isTeleportEnabled = false;
            StartCoroutine("TeleportCooldown");
        }
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

    IEnumerator TeleportCooldown()
    {
        yield return new WaitForSeconds(teleportCooldownTime);
        isTeleportEnabled = true;
    }
}
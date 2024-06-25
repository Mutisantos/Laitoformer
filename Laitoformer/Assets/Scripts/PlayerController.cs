using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D playerRigidBody;
    public Animator playerAnimator;
    public Transform raycastSource;
    public Transform raycastDownward;
    public GameObject smallColliders;
    public GameObject bigColliders;
    public AudioClip jumpSfx;
    public AudioClip defeatSfx;
    public AudioClip powerDownSfx;
    public float moveSpeed = 4f;
    public float jumpSpeed = 600;
    public float damageCooldown = 1f;
    public float raycastDistance = 0.5f;

    [SerializeField]
    bool isGrounded = true; //Si esta en contacto
    bool isSmall = true;
    [SerializeField]
    bool hasHitCooldown = false;
    public float jumpTimeLimit = 0.3f;
    float jumpAddedForce;
    float playerGravityScale;
    [SerializeField]
    bool isJumpInProcess;
    SpriteRenderer spriteRenderer;
    RaycastHit2D rightRaycastHit2D;
    RaycastHit2D leftRaycastHit2D;
    RaycastHit2D upRaycastHit2D;
    RaycastHit2D downRaycastHit2D;
    [SerializeField]
    LayerMask originalLayer;

    void Awake()
    {
        isSmall = true;
        playerGravityScale = this.playerRigidBody.gravityScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimator.SetBool("isDamaged", false);
        playerAnimator.SetBool("isVictory", false);
        playerAnimator.SetBool("isMorphing", false);
        playerAnimator.SetBool("isSmall", isSmall);
        smallColliders.SetActive(isSmall);
        bigColliders.SetActive(!isSmall);
        originalLayer = gameObject.layer;
    }

    void Update()
    {
        //Deshabilita los inputs mientras la escena no esté activa
        if (GameManager.Instance.IsSceneActive())
        {
            processHorizontalInput();
            processJumpInput();
        }
    }

    private void FixedUpdate()
    {
        if (!isGrounded)
        {
            //Para animacion de subida o de bajada
            playerAnimator.SetFloat("ySpeed", playerRigidBody.velocity.y);
        }
        rightRaycastHit2D = Physics2D.Raycast(this.raycastSource.position, Vector2.right, raycastDistance);
        leftRaycastHit2D = Physics2D.Raycast(this.raycastSource.position, Vector2.left, raycastDistance);
        upRaycastHit2D = Physics2D.Raycast(this.raycastSource.position, Vector2.up, raycastDistance * 2);
        Debug.DrawRay(this.raycastSource.position, Vector2.right * raycastDistance, Color.blue);
        Debug.DrawRay(this.raycastSource.position, Vector2.left * raycastDistance, Color.blue);
        Debug.DrawRay(this.raycastSource.position, Vector2.up * raycastDistance * 2, Color.blue);
        DetectRaycastColissionOnEnemies(rightRaycastHit2D);
        DetectRaycastColissionOnEnemies(leftRaycastHit2D);
        DetectRaycastColissionOnEnemies(upRaycastHit2D);
    }

    private void DetectRaycastColissionOnEnemies(RaycastHit2D raycastHit)
    {
        if (raycastHit.collider != null)
        {
            if (raycastHit.collider.CompareTag("Enemy") && !hasHitCooldown)
            {
                Debug.DrawRay(this.raycastSource.position, Vector2.right * raycastDistance, Color.red);
                Debug.DrawRay(this.raycastSource.position, Vector2.left * raycastDistance, Color.red);
                Debug.DrawRay(this.raycastSource.position, Vector2.up * raycastDistance * 2, Color.red);
                ProcessDamage();
            }
            else
            {
                Debug.DrawRay(this.raycastSource.position, Vector2.right * raycastDistance, Color.green);
                Debug.DrawRay(this.raycastSource.position, Vector2.left * raycastDistance, Color.green);
                Debug.DrawRay(this.raycastSource.position, Vector2.up * raycastDistance * 2, Color.green);
            }
        }
    }
    private void DetectRaycastColissionOnGround(RaycastHit2D raycastHit)
    {
        if (raycastHit.collider != null)
        {
            Debug.DrawRay(this.raycastDownward.position, Vector2.down * raycastDistance / 2, Color.red);
            isGrounded = true;
            playerAnimator.SetBool("isGrounded", true);
        }
    }

    private void processJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            SoundManager.Instance.PlayPlayerSoundeOnce(this.jumpSfx);
            isJumpInProcess = true;
            jumpAddedForce = 0;
            isGrounded = false;
        }
        if (isJumpInProcess)
        {
            //Se irá añadiendo intensidad al salto cuanto más tiempo esté oprimida la tecla de salto
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, jumpSpeed);
            jumpAddedForce += Time.deltaTime;
            playerAnimator.SetBool("isGrounded", false);
        }
        if (Input.GetKeyUp(KeyCode.Space) || jumpAddedForce > jumpTimeLimit)
        {
            //Se limita el salto cuando se suelta el boton o se supera el tiempo dado
            isJumpInProcess = false;
        }
    }

    private void processHorizontalInput()
    {
        if (Input.GetAxis("Horizontal") == 0)
        {//Reposo
            playerAnimator.SetBool("isWalking", false);
            playerRigidBody.velocity = new Vector2(0, playerRigidBody.velocity.y);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {// Izquierda
            FlipSprite("isWalking", true);
            playerRigidBody.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, playerRigidBody.velocity.y);
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {// Derecha
            FlipSprite("isWalking", false);
            playerRigidBody.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, playerRigidBody.velocity.y);
        }
    }

    private void ProcessDamage()
    {
        hasHitCooldown = true;
        if (GameManager.Instance.HasPowerUp())
        {
            StartCoroutine(HitCooldownBig());
        }
        else
        {
            StartCoroutine(HitCooldownSmall());
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Player Collided with:" + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            playerAnimator.SetBool("isGrounded", true);
        }
        if (collision.gameObject.CompareTag("Block"))
        {
            downRaycastHit2D = Physics2D.Raycast(this.raycastDownward.position, Vector2.down, raycastDistance * 2);
            DetectRaycastColissionOnGround(downRaycastHit2D);

        }
        if (collision.gameObject.CompareTag("Deadzone") && !hasHitCooldown)
        {
            hasHitCooldown = true;
            StartCoroutine(HitCooldownSmall());
        }
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            hasHitCooldown = true;
            StartCoroutine(ProcessPowerup());
        }

        if (collision.gameObject.CompareTag("Finish"))
        {
            hasHitCooldown = true;
            StartCoroutine(ProcessVictory());
        }
    }

    private void FlipSprite(string animationsType, bool doFlip)
    {
        playerAnimator.SetBool(animationsType, true);
        spriteRenderer.flipX = doFlip;
    }


    IEnumerator ProcessPowerup()
    {
        //Animar la transformación y detener cualquier posible daño mientras ocurre
        //Debug.Log("Morphing");
        GameManager.Instance.SetSceneActive(false);
        playerAnimator.SetBool("isMorphing", true);
        bigColliders.SetActive(false);
        smallColliders.SetActive(false);
        this.playerRigidBody.gravityScale = 0;
        Vector2 currentSpeed = this.playerRigidBody.velocity;
        this.playerRigidBody.velocity = Vector2.zero;
        yield return new WaitForSecondsRealtime(damageCooldown);
        GameManager.Instance.SetSceneActive(true);
        playerAnimator.SetBool("isMorphing", false);
        playerAnimator.SetBool("isDamaged", false);
        playerAnimator.SetBool("isSmall", false);
        bigColliders.SetActive(true);
        //Retornar la gravedad a la normalidad tambien ahora que los colliders estan en su lugar
        this.playerRigidBody.gravityScale = playerGravityScale;
        hasHitCooldown = false;
        this.playerRigidBody.velocity = currentSpeed;
        StopCoroutine(ProcessPowerup());
    }
    IEnumerator ProcessVictory()
    {
        //Animar la celebración
        Debug.Log("Victory");
        this.playerRigidBody.velocity = Vector2.zero;
        GameManager.Instance.SetSceneActive(false);
        playerAnimator.SetBool("isVictory", true);
        playerAnimator.SetBool("isGrounded", true);
        playerAnimator.SetBool("isWalking", false);
        playerAnimator.SetBool("isDamaged", false);
        GameManager.Instance.ShowVictoryCrossfade(true);
        yield return new WaitForSecondsRealtime(damageCooldown);
        hasHitCooldown = false;
    }

    IEnumerator HitCooldownBig()
    {
        //Animar el daño, deshabilitar temporalmente las colisiones por el cooldown
        //Debug.Log("enemy Colision Big");
        playerAnimator.SetBool("isDamaged", true);
        gameObject.layer = LayerMask.NameToLayer("Ignore Enemies");
        SoundManager.Instance.PlayReverseAudio(this.powerDownSfx);
        yield return new WaitForSecondsRealtime(damageCooldown);
        //Retornar a las animaciones pero ahora con el sprite pequeño
        playerAnimator.SetBool("isDamaged", false);
        playerAnimator.SetBool("isSmall", true);
        yield return new WaitForSecondsRealtime(damageCooldown);
        bigColliders.SetActive(false);
        smallColliders.SetActive(true);
        gameObject.layer = this.originalLayer;
        //Retornar la gravedad a la normalidad tambien ahora que los colliders estan en su lugar
        hasHitCooldown = false;
        GameManager.Instance.ChangePowerUp(false);
    }

    IEnumerator HitCooldownSmall()
    {
        //Procesar la transición de la derrota
        //Debug.Log("enemy Colision Small");
        playerAnimator.SetBool("isDamaged", true);
        this.playerRigidBody.velocity = Vector2.zero;
        GameManager.Instance.SetSceneActive(false);
        SoundManager.Instance.PlayPlayerSoundeOnce(this.defeatSfx);
        yield return new WaitForSecondsRealtime(damageCooldown);
        spriteRenderer.color = UnityEngine.Color.white;
        playerAnimator.SetBool("isDamaged", false);
        playerAnimator.SetBool("isSmall", true);
        GameManager.Instance.ShowCrossfade(true);
        GameManager.Instance.ChangeLives(-1);
        //Se activa siempre como si se volviera a ser pequeño por las caidas
        smallColliders.SetActive(true);
        bigColliders.SetActive(false);
        this.playerRigidBody.gravityScale = playerGravityScale;
        hasHitCooldown = false;
    }


}

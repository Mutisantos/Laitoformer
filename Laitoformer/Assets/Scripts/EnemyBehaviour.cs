using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {

    public AudioClip deadSfx;
    public float speed = 2f;
    public float enemyHeight = 0.3f;
    public Transform raycastPosition;
    public Collider2D bodyCollider;

    SpriteRenderer spriteRenderer;
    Rigidbody2D enemyRigidBody;
    Animator enemyAnimator;
    RaycastHit2D raycastHit2D;

    bool isSightedOnCamera = false;
    bool isFacingRight = false;
    bool detectedWall = false;
    bool isDead = false;

    private void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
    }

    private void Update()
    {
        raycastHit2D = Physics2D.Raycast(this.raycastPosition.position, Vector2.up, enemyHeight);
        Debug.DrawRay(this.raycastPosition.position, Vector2.up * enemyHeight, Color.red);
        if (raycastHit2D.collider != null)
        {
            if (raycastHit2D.collider.CompareTag("PlayerFeet") && !isDead){
                isDead = true;
                StartCoroutine(DisposeEnemy());
            }
        }
        //Deshabilita el movimiento si la escena esta inactiva/pausada
        if (isSightedOnCamera && GameManager.Instance.IsSceneActive())
        {
            enemyRigidBody.velocity = new Vector2(speed, enemyRigidBody.velocity.y);
        }
        else
        {
            enemyRigidBody.velocity = Vector2.zero; 
        }
    }

    private void FlipSprite(bool status)
    {
        spriteRenderer.flipX = status;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MainCamera")
        {
            isSightedOnCamera = true;
        }
        if (collision.gameObject.tag == "Ground" && !detectedWall)
        {
            speed *= -1;
            isFacingRight = !isFacingRight;
            FlipSprite(isFacingRight);
            detectedWall = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            detectedWall = false;
        }
    }

    IEnumerator DisposeEnemy()
    {
        enemyAnimator.SetBool("isDead", true);
        SoundManager.Instance.PlayEffectOnce(this.deadSfx);
        speed *= 0;
        yield return new WaitForSecondsRealtime(0.5f);
        GameManager.Instance.ChangeScore(100);
        DisableEnemy();
    }

    //Elimina el objeto del enemigo por completo
    public void DisableEnemy()
    {
        Destroy(gameObject);
    }

}

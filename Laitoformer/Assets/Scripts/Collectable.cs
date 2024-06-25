using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum CollectableType
{
    BLOCK_COOKIE, FLOATING_COOKIE, POWERUP, DISPOSABLE
}

public class Collectable : MonoBehaviour {

    public AudioClip collectableAudio;
    public float timeAlive;
    public float moveSpeed;
    public Rigidbody2D collectableBody;
    public CollectableType type;

    bool disposed = false;

    void FixedUpdate()
    {
        if (type == CollectableType.BLOCK_COOKIE)
        {
            timeAlive -= Time.deltaTime;
            if (timeAlive <= 0 && !disposed)
            {
                disposed = true;
                GameManager.Instance.ChangeCookies(1);
                StartCoroutine(DisposeCollectable(100));
            }
        }
        if (type == CollectableType.DISPOSABLE)
        {
            timeAlive -= Time.deltaTime;
            if (timeAlive <= 0 )
            {
                StartCoroutine(DisposeCollectable(0));
            }
        }
        if (type == CollectableType.POWERUP)
        {
            collectableBody.velocity = new Vector2(moveSpeed, collectableBody.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Block"))
        {
            moveSpeed *= -1;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (type == CollectableType.POWERUP)
            {
                GameManager.Instance.ChangePowerUp(true);
            }
            if (type == CollectableType.FLOATING_COOKIE)
            {
                GameManager.Instance.ChangeCookies(1);
            }
            SoundManager.Instance.PlayEffectOnce(this.collectableAudio);
            StartCoroutine(DisposeCollectable(100));
        }
    }

    IEnumerator DisposeCollectable(int score)
    {
        GameManager.Instance.ChangeScore(score);
        yield return new WaitForSecondsRealtime(0.01f);
        Destroy(gameObject);
    }



}

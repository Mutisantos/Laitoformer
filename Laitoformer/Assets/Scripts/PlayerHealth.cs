using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    int hp = 3;
    bool hasCoodown = false;
    public Image[] hps;

    public SceneLoader sceneChanger;
    public SpriteRenderer playerRenderer;
    public AudioSource playerAudio;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && !(transform.position.y + 0.03f > collision.transform.position.y))
        {
            playerRenderer.color = UnityEngine.Color.red;
            playerAudio.Play();
            SubstractHp();
        }
    }

    void SubstractHp()
    {
        if (!hasCoodown)
        {
            if (hp > 0)
            {
                hp--;
                hasCoodown = true;

                StartCoroutine(Cooldown());
            }

            if(hp <= 0)
            {
                sceneChanger.LoadSceneBySceneIndex(1);
            }

            EmptyHearts();
        }
    }

    void EmptyHearts()
    {
        for(int i = 0; i < hps.Length; i++)
            if(hp -1 < i)  hps[i].gameObject.SetActive(false);
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        hasCoodown = false;

        playerRenderer.color = UnityEngine.Color.white;

        StopCoroutine(Cooldown());
    }
}

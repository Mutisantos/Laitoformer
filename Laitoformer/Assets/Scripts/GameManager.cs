using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


//Singleton para GameManager
public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public int remainingSeconds = 400;
    public int initialLives = 3;
    public Text livesLabelCounter;
    public Text cookieLabelCounter;
    public Text timeLabelCounter;
    public Text scoreLabelCounter;
    public Animator crossfaderAnimator; //Para animar transiciones
    public Text titleText;
    public Text subtitleText;
    public GameObject player;
    public GameObject cinemachineVirtualCamera;
    public GameObject checkpoint;
    public AudioClip defeatFanfare;
    public AudioClip victoryFanfare;

    private int score = 0;
    private int cookies; //Monedas, pero mejores
    [SerializeField]
    private int lives;
    [SerializeField]
    private float time;
    private static float restartTime = 3f;//Tiempo de espera para la transición de derrota
    private float restartCountdown = restartTime;
    [SerializeField]
    private bool hasPowerUp = false;
    [SerializeField]
    private bool isWaitingOnCrossfade = false;
    [SerializeField]
    private bool isSceneActive = true;
    private bool finishedLevel = false;

    private void MakeSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Awake()
    {
        this.lives = initialLives;
        MakeSingleton(); 
        ResetValues();
    }


    private void Update()
    {
        if (isWaitingOnCrossfade)
        { //Tiempo de transición
            restartCountdown -= Time.deltaTime;
            if (restartCountdown <= 0)
            {
                if (this.lives > 0)
                {
                    this.subtitleText.text = "Presiona cualquier tecla para continuar.";
                }
                else
                {
                    this.subtitleText.text = "Game Over";
                }
                isSceneActive = false;
                if (Input.anyKey)
                {
                    if (this.finishedLevel)
                    {
                        StartCoroutine(WaitAndGoToNextLevel());
                    }else
                    {
                        SoundManager.Instance.PlayBackgroundMusic();
                        crossfaderAnimator.SetBool("levelStarted", false);
                        this.ResetValues();
                        isSceneActive = true;
                        restartCountdown = restartTime;
                        isWaitingOnCrossfade = false;
                    }
                }
            }
        }
        else
        { //Flujo normal del tiempo 
            if (this.time > 0)
            {
                this.time -= Time.deltaTime;
                timeLabelCounter.text = Mathf.FloorToInt(this.time) + "";
            }
            else
            { //Perder porque se agote el tiempo
                this.ShowCrossfade(true);
            }
        }
    }

    public void ResetValues()
    {
        this.time = remainingSeconds;
        this.hasPowerUp = false;
        livesLabelCounter.text = "x" + this.lives;
        scoreLabelCounter.text = this.score + "";
        timeLabelCounter.text = this.time + "";
        cookieLabelCounter.text = "x" + this.cookies;
    }


    public void ChangeScore(int points)
    {
        this.score += points;
        scoreLabelCounter.text = this.score + "";
    }
    public void ChangeCookies(int points)
    {
        this.cookies += points;
        cookieLabelCounter.text = "x" + this.cookies;
    }

    public void ChangeLives(int changeValue)
    {
        this.lives += changeValue;
        livesLabelCounter.text = "x" + this.lives;
    }
    public void ChangePowerUp(bool hasPowerup)
    {
        this.hasPowerUp = hasPowerup;
    }

    public bool IsSceneActive()
    {
        return this.isSceneActive;
    }
    public void SetSceneActive(bool isAcitve)
    {
        this.isSceneActive = isAcitve;
    }


    public bool HasPowerUp()
    {
        return this.hasPowerUp;
    }

    public void ShowCrossfade(bool fadeCrossfade)
    {
        this.isWaitingOnCrossfade = fadeCrossfade;
        SoundManager.Instance.StopBackgroundMusic();
        SoundManager.Instance.PlayEffectOnce(defeatFanfare);
        StartCoroutine(SendPlayerToLastCheckpoint());
        crossfaderAnimator.SetBool("levelStarted", fadeCrossfade);
    }
    public void ShowVictoryCrossfade(bool fadeCrossfade)
    {
        this.isWaitingOnCrossfade = fadeCrossfade;
        SoundManager.Instance.StopBackgroundMusic();
        SoundManager.Instance.PlayEffectOnce(victoryFanfare);
        StartCoroutine(StartCrossFadeAsync());
    }

    IEnumerator StartCrossFadeAsync()
    {
        yield return new WaitForSecondsRealtime(5f);
        crossfaderAnimator.SetBool("levelStarted", this.isWaitingOnCrossfade);
        this.titleText.text = "Nivel Superado";
        this.score += Mathf.FloorToInt(this.time * 10);
        this.subtitleText.text = $"Puntaje: {this.score} \n Galletas: {this.cookies}";
        restartCountdown = restartTime;
        finishedLevel = true;
    }

    IEnumerator WaitAndGoToNextLevel()
    {
        yield return new WaitForSecondsRealtime(2f);
        SoundManager.Instance.ChangeBackgroundMusic(2);
        SceneManager.LoadScene(2);
    }
    

        IEnumerator SendPlayerToLastCheckpoint()
    {
        //Debug.Log("enter reset checkpoint");
        cinemachineVirtualCamera.SetActive(false);
        player.transform.SetPositionAndRotation(checkpoint.transform.position, Quaternion.identity);
        yield return new WaitForSecondsRealtime(0.5f);
        cinemachineVirtualCamera.transform.SetPositionAndRotation(player.transform.position, Quaternion.identity);
        cinemachineVirtualCamera.SetActive(true);
    }





}

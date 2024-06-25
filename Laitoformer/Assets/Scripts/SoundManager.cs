using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public AudioSource FxSource;
	public AudioSource PlayerSource;
	public AudioSource MusicSource;
	public static SoundManager Instance = null;

	public AudioClip[] BGSceneMusicList;

	public float LowFxPitch = 0.90f;
	public float HighFxPitch  = 1.10f;


	void Awake() {
		MakeSingleton ();
	}

	private void MakeSingleton() {
		if (Instance != null) {
			Destroy (gameObject);
		} else {
			Instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

    public void PlayEffectOnce(AudioClip clip)
    {
        FxSource.clip = clip;
        FxSource.pitch = 1f;
        FxSource.Play();
    }
    public void PlayPlayerSoundeOnce(AudioClip clip)
    {
        PlayerSource.clip = clip;
        PlayerSource.pitch = 1f;
        PlayerSource.Play();
    }

    public void RandomizeFx (params AudioClip [] clips){
		StartCoroutine(playAsync(clips));
	}

    public void ChangeBackgroundMusic(int index)
    {
        if (BGSceneMusicList.Length > index)
        {
            MusicSource.Stop();
            MusicSource.clip = BGSceneMusicList[index];
            MusicSource.Play();
        }
    }
    public void StopBackgroundMusic()
    {
       MusicSource.Pause();
    }

    public void PlayBackgroundMusic()
    {
        MusicSource.Play();
    }

    public void PlayReverseAudio(AudioClip audioClip)
    {
        FxSource.pitch = -1;
        FxSource.loop = true;
        FxSource.clip = audioClip;
        FxSource.Play();
        StartCoroutine(StopLoop());
    }

    public IEnumerator StopLoop()
    {
        yield return new WaitForSeconds(1f);
        FxSource.loop = false;
    }

    IEnumerator playAsync(AudioClip [] clips)
	{
		for (int i = 0; i < 5; i++)
		{
			int random = Random.Range(0, clips.Length);
			float randomPitch = Random.Range(this.LowFxPitch, this.HighFxPitch);
			FxSource.clip = clips[random];
			FxSource.pitch = randomPitch;
			FxSource.Play();
			yield return new WaitForSeconds(0.5f);
			FxSource.Stop();
		}
	}
}

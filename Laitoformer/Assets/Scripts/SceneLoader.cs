using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadSceneBySceneIndex(int index){
		SoundManager.Instance.ChangeBackgroundMusic(index);
		SceneManager.LoadScene (index);
	}

	public void LoadSceneBySceneIndexKeepMusic(int index)
	{
		SceneManager.LoadScene(index);
	}

	public void FinishGame(){
		Application.Quit ();
	}

}

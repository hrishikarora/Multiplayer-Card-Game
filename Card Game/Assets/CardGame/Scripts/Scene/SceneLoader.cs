using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : BaseSingleton<SceneLoader>
{
    

    protected override void Awake()
    {
        base.Awake();
    }


    public void LoadScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            Debug.Log($"[SceneLoader] Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }
}
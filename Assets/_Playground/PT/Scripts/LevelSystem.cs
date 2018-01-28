using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelSystem : MonoBehaviour {
    static LevelSystem instance;

    void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    IEnumerator ReloadCoroutine()
    {
        int sID = SceneManager.GetActiveScene().buildIndex;
        AsyncOperation ao = null;
        /*
        ao = SceneManager.UnloadSceneAsync(sID);
        while (ao != null && !ao.isDone)
        {
            yield return null;
        }
        */
        string emptySceneID = "_SSCore/Scenes/Empty";
        ao = SceneManager.LoadSceneAsync(emptySceneID, LoadSceneMode.Single);
        while (ao != null && !ao.isDone)
        {
            yield return new WaitForSeconds(1f);
        }

        ao = SceneManager.LoadSceneAsync(sID, LoadSceneMode.Single);
        while (ao != null && !ao.isDone)
        {
            yield return new WaitForSeconds(1f);
        }
    }

    void reloadLevel()
    {
        StartCoroutine(ReloadCoroutine());
    }

    public static void ReloadLevel()
    {
        if (instance)
        {
            instance.reloadLevel();
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void BeforeSceneLoad()
    {
        if (!instance)
        {
            GameObject go = new GameObject("LevelSystem", typeof(LevelSystem));
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AfterSceneLoad()
    {
    }
}

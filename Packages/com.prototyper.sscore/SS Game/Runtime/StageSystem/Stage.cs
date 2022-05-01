using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace SS
{
    [CreateAssetMenu(fileName = "Stage", menuName = "SS/Stage", order = 0)]
    public class Stage : ScriptableObject
    {
        private const string emptySceneName = "Empty";
        public bool canEnter = true;
        public float leaveBlackOutTime = 0.3f;
        public float enterBlackOutTime = 0.3f;

        private StageSystem _system;
        private Coroutine coroutine;
        private bool _isActive = false;

        [SerializeField]
        private string sceneName;
        private string SceneName
        {
            get {
                return (!string.IsNullOrEmpty(sceneName)? sceneName: name);
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
        }

        public virtual void Init(StageSystem system)
        {
            _system = system;
        }

        public virtual bool CanEnter(Stage nextStage)
        {
            return (nextStage && nextStage.canEnter);
        }

        public virtual void OnEnter()
        {
            if (coroutine != null)
                _system.StopCoroutine(coroutine);
            coroutine = _system.StartCoroutine(Prepare());
        }

        IEnumerator Prepare()
        {
            Debug.Log(name + ".Prepare");

            // LoadScene
            
            var ao = SceneManager.LoadSceneAsync(SceneName);
            if (ao != null)
            {
                ao.allowSceneActivation = false;
                while (!ao.isDone)
                {
                    if (ao.progress >= 0.9f)    // 0.9 is magic number
                    {
                        ao.allowSceneActivation = true;
                    }
                    yield return null;
                }
            }

            _isActive = true;
            Debug.Log(name + ".Prepare Done");
        }

        public virtual void Update()
        {
            if (IsActive)
            {

            }
        }

        public virtual void OnLeave()
        {
            if (coroutine != null)
                _system.StopCoroutine(coroutine);
            coroutine = _system.StartCoroutine(Release());
        }

        IEnumerator Release()
        {
            Debug.Log(name + ".Release");

            // Release memory
            var ao = SceneManager.LoadSceneAsync(emptySceneName);
            if (ao != null)
            {
                ao.allowSceneActivation = false;
                while (!ao.isDone)
                {
                    if (ao.progress >= 0.9f)
                    {
                        ao.allowSceneActivation = true;
                    }
                    yield return null;
                }
            }

            // TOCHECK: Unload scene (invalid for now)
            //Resources.UnloadUnusedAssets();

            _isActive = false;
            Debug.Log(name + ".Release Done");
        }
    }
}
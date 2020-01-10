using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace SS
{
    [CreateAssetMenu(fileName = "Stage", menuName = "SS/Stage", order = 0)]
    public class Stage : ScriptableObject
    {
        public bool canEnter = true;

        private StageSystem _system;
        private Coroutine coroutine;
        private bool _isActive = false;
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

            // Release memory
            AsyncOperation ao = SceneManager.LoadSceneAsync("Empty");
            while (!ao.isDone)
            {
                yield return null;
            }
            Resources.UnloadUnusedAssets();

            // LoadScene
            ao = SceneManager.LoadSceneAsync(name);
            while (!ao.isDone)
            {
                yield return null;
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

            AsyncOperation ao = SceneManager.UnloadSceneAsync(name);
            if (ao != null)
            {
                while (!ao.isDone)
                {
                    yield return null;
                }
            }
            _isActive = false;
            Debug.Log(name + ".Release Done");
        }
    }
}
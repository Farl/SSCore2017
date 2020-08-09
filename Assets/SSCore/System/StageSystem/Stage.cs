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

            // LoadScene
            AsyncOperation ao = SceneManager.LoadSceneAsync(name);
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

            // Unload scene
            // Release memory
            AsyncOperation ao = SceneManager.LoadSceneAsync("Empty");
            if (ao != null)
            {
                while (!ao.isDone)
                {
                    yield return null;
                }
            }
            //Resources.UnloadUnusedAssets();

            _isActive = false;
            Debug.Log(name + ".Release Done");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace SS
{
    [CreateAssetMenu(fileName = "Stage", menuName = "SS/Stage", order = 0)]
    public class Stage : ScriptableObject
    {
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

        public virtual void OnEnter()
        {
            if (coroutine != null)
                _system.StopCoroutine(coroutine);
            coroutine = _system.StartCoroutine(Prepare());
        }

        IEnumerator Prepare()
        {
            Debug.Log(name + ".Prepare");

            yield return new WaitForSecondsRealtime(3.0f);

            // Test
            FadeUI.StartFade(Color.black, 3f, 0f, 0f, false);
            yield return new WaitForSecondsRealtime(3.0f);

            AsyncOperation ao = SceneManager.LoadSceneAsync(name);
            while (!ao.isDone)
            {
                yield return null;
            }
            
            _isActive = true;
            Debug.Log(name + ".Prepare Done");

            // Fade
            FadeUI.StartFade(Color.black, 0f, 0f, 3f, false);
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

            // Test
            yield return new WaitForSecondsRealtime(3.0f);

            AsyncOperation ao = SceneManager.UnloadSceneAsync(name);
            while (!ao.isDone)
            {
                yield return null;
            }
            _isActive = false;
            Debug.Log(name + ".Release Done");
        }
    }
}
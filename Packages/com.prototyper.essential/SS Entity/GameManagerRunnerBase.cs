using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class GameManagerRunnerBase : MonoBehaviour
    {
        #region Public

        public GameModeBase gameMode;

        #endregion

        #region Protected Virtual
        protected virtual void OnRunnerAwake()
        {

        }

        protected virtual async void Start()
        {
            await GameManager.RunnerStartAsync();
        }
        #endregion

        #region Private
        private void Awake()
        {
            if (!GameManager.RegisterRunner(this))
            {
                Debug.LogError("Duplicate Runner!", this);
                GameObject.Destroy(gameObject);
            }
            else
            {
                OnRunnerAwake();
            }
        }

        private void Update()
        {
            GameManager.Update();
        }

        private void LateUpdate()
        {
            GameManager.LateUpdate();
        }

        private void OnDestroy()
        {
            GameManager.UnregisterRunner(this);
        }
        #endregion
    }
}

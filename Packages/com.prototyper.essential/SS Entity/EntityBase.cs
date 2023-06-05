using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class EntityBase : MonoBehaviour
    {
        #region Public
        public bool IsUpdate = true;

        public virtual void SetupGameMode(GameModeBase gameMode)
        {

        }


        public virtual void OnRunnerStart()
        {

        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnRestart()
        {

        }
        #endregion

        #region Private
        #endregion

        #region Protected

        protected void Awake()
        {
            GameManager.Register(this);
            OnEntityAwake();
        }

        protected void Start()
        {
            OnEntityStart();
        }

        protected virtual void OnEntityStart()
        {

        }

        protected virtual void OnEntityAwake()
        {

        }

        protected void OnDestroy()
        {
            GameManager.Unregister(this);
            OnEntityDestroy();
        }

        protected virtual void OnEntityDestroy()
        {

        }
        #endregion
    }
}

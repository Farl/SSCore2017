using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace SS
{
    public class GameModeBase : MonoBehaviour
    {
        #region Public

        public bool IsInitialized { get; private set; } = false;

        public async Task Intialize()
        {
            if (IsInitialized)
                return;

            // Do
            await OnIntialize(cancellation.Token).ConfigureAwait(false);

            IsInitialized = true;
        }

        public virtual void Restart()
        {

        }

        public virtual void LateRestart()
        {

        }

        public virtual void LoadSaveData()
        {

        }

        public virtual void ClearSaveData()
        {
        }

        public virtual void SaveSaveData(System.Action<bool> onDone = null)
        {
        }

        public virtual void OnUpdate()
        {

        }

        #endregion

        #region Protected
        protected CancellationTokenSource cancellation = new CancellationTokenSource();

        protected virtual void OnDestroy()
        {
            cancellation.Cancel();
        }

        protected virtual async Task OnIntialize(CancellationToken token)
        {
            await Task.Yield();
        }

        #endregion



        #region Private


        #endregion
    }
}

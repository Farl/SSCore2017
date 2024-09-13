using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class UIEntityAnimatedGameMode<T> : UIEntityAnimated where T : GameModeBase
    {
        protected T mode
        {
            get
            {
                if (_mode == null)
                {
                    _mode = GameManager.CurrentGameMode as T;
                }
                return _mode;
            }
        }
        private T _mode;
    }
}
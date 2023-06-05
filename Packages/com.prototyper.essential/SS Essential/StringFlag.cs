using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class StringFlag
    {
        public System.Action<StringFlag> onFlagChanged = null;
        public System.Action onFlagEmpty = null;
        public System.Action onFlagExist = null;

        private HashSet<string> _flags = new HashSet<string>();

        public void AddFlag(string flag)
        {
            if (IsEmpty)
            {
                onFlagExist?.Invoke();
            }
            _flags.Add(flag);
            onFlagChanged?.Invoke(this);
        }

        public void RemoveFlag(string flag)
        {
            _flags.Remove(flag);
            if (IsEmpty)
            {
                onFlagEmpty?.Invoke();
            }
            onFlagChanged?.Invoke(this);
        }

        public void ClearFlag()
        {
            _flags.Clear();
            onFlagEmpty?.Invoke();
            onFlagChanged?.Invoke(this);
        }

        public bool IsEmpty
        {
            get
            {
                return _flags.Count == 0;
            }
        }
    }
}

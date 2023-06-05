using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public interface ISaveLoadData
    {
        public void OnLoadData();

        public void OnSaveData();

        public void OnClearData();
    }
}

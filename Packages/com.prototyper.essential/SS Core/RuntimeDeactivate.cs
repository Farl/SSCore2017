using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class RuntimeDeactivate : MonoBehaviour
    {
        [SerializeField] List<GameObject> gameObjects;
        [SerializeField] RuntimeDisable.EditorPlayer editorPlayer;

        private void Awake()
        {
            if (RuntimeDisable.Check(editorPlayer))
            {
                foreach (var go in gameObjects)
                {
                    go.SetActive(false);
                }
            }
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class RuntimeDisable : MonoBehaviour
    {
        [System.Flags]
        public enum EditorPlayer
        {
            Editor = 1 << 0,
            Player = 1 << 1,
        }
        [SerializeField] private List<Component> components = new List<Component>();
        [SerializeField] private EditorPlayer editorPlayer;

        public static bool Check(EditorPlayer editorPlayer)
        {
            EditorPlayer currMode = 0;
#if UNITY_EDITOR
            currMode = EditorPlayer.Editor;
#else
            currMode = EditorPlayer.Player;
#endif
            return ((editorPlayer & currMode) != 0);
        }

        private void Awake()
        {
            if (Check(editorPlayer))
            {
                foreach (var component in components)
                {
                    var mono = component as Behaviour;
                    if (mono)
                    {
                        mono.enabled = false;
                    }
                    else
                    {
                        var collider = component as Collider;
                        if (collider)
                        {
                            collider.enabled = false;
                        }
                        else
                        {
                            var renderer = component as Renderer;
                            if (renderer)
                            {
                                renderer.enabled = false;
                            }
                            else
                            {
                                // TODO
                            }
                        }
                    }
                }
            }
        }
    }

}
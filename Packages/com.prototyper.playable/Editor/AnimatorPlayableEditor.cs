using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Animations;

namespace SS
{
    public class AnimatorPlayableEditor : EditorWindow
    {
        const string editorTitle = @"Animator Playable Editor";
        const string version = "0.0.1";

        PlayableGraph playableGraph;
        AnimatorPlayableComponent apComp;
        float crossfadeTime = 0.3f;

        [MenuItem("Tools/SS/Animator Playable Editor")]
        static void Open()
        {
            var w = EditorWindow.CreateWindow<AnimatorPlayableEditor>();
            w.titleContent = new GUIContent($"{editorTitle} {version}");
        }

        private void OnGUI()
        {
            EditorGUILayout.ObjectField(new GUIContent("Target"), apComp, typeof(AnimatorPlayableComponent), false);
            crossfadeTime = EditorGUILayout.FloatField("Crossfade Time", crossfadeTime);
            if (apComp)
            {
                foreach (var clip in apComp.animationClips)
                {
                    if (GUILayout.Button(clip.name))
                    {
                        apComp.Play(clip.name, crossfadeTime);
                    }
                }

                if (GUILayout.Button("Graph"))
                    GraphVisualizerClient.Show(playableGraph);
            }

            if (GUILayout.Button("Search"))
            {
                var graphs = GraphVisualizerClient.GetGraphs();
            }
        }

        private void OnSelectionChange()
        {
            var go = Selection.activeGameObject;
            var comp = go.GetComponent<AnimatorPlayableComponent>();
            if (comp)
            {
                playableGraph = comp.playableGraph;
                apComp = comp;
            }
        }
    }
}
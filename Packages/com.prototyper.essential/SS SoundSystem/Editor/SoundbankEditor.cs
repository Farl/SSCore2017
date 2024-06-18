
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SS
{
    // Draw a custom inspector for Soundbank
    // Support drag and drop to add AudioClip to the Soundbank
    // List SoundSystem.AudioData in Soundbank
    // Automatically manage AudioSource in SoundBank hierarchy (in children of Soundbank object)
    // You can edit name of AudioSources and that is also the event name to play the sound
    // You can edit the volume from list

    [CustomEditor(typeof(Soundbank))]
    public class SoundbankEditor : Editor
    {
        private Soundbank soundbank;

        private void OnEnable()
        {
            soundbank = (Soundbank)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            // Draw a drop area for AudioClip
            var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drag & Drop AudioClip here");

            // Clear data
            if (GUILayout.Button("Clear"))
            {
                ClearData();
            }

            // Handle drag and drop
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            if (obj is AudioClip clip)
                            {
                                AddClip(clip);
                            }
                        }
                    }

                    break;
            }

            // List
            for (int i = 0; i < soundbank.audioDatas.Count; i++)
            {
                var audioData = soundbank.audioDatas[i];

                EditorGUILayout.BeginHorizontal();

                // Name
                EditorGUI.BeginChangeCheck();
                audioData.soundEventID = EditorGUILayout.DelayedTextField(audioData.soundEventID);
                if (EditorGUI.EndChangeCheck() && audioData.audioSource)
                {
                    // Change name of AudioSource
                    Rename(audioData, audioData.soundEventID);
                    CheckUnique();
                }
                
                // AudioSource
                audioData.audioSource = (AudioSource)EditorGUILayout.ObjectField(audioData.audioSource, typeof(AudioSource), true);

                // Volume
                if (audioData.audioSource)
                {
                    audioData.audioSource.volume = EditorGUILayout.Slider(audioData.audioSource.volume, 0, 1);
                }

                // Remove button
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    RemoveClip(i);
                }

                EditorGUILayout.EndHorizontal();
            }
            // Add Empty clip
            if (GUILayout.Button("+"))
            {
                AddClip(null);
            }
            
        }

        private void SetSoundbankDirty()
        {
            EditorUtility.SetDirty(soundbank);
        }

        private void AddClip(AudioClip clip)
        {
            var clipName = clip? clip.name: "Empty";
            var lastAudioData = soundbank.audioDatas.Count > 0? soundbank.audioDatas[soundbank.audioDatas.Count - 1]: null;
            var mixerGroup = (lastAudioData != null && lastAudioData.audioSource)? lastAudioData.audioSource.outputAudioMixerGroup: null;

            // Create AudioSource
            var audioSource = new GameObject(clipName).AddComponent<AudioSource>();
            audioSource.transform.SetParent(soundbank.transform, worldPositionStays: false);
            audioSource.clip = clip;
            audioSource.playOnAwake = false;
            audioSource.outputAudioMixerGroup = mixerGroup;


            var audioData = new SoundSystem.AudioData
            {
                audioSource = audioSource,
                soundEventID = clipName
            };
            soundbank.audioDatas.Add(audioData);
            SetSoundbankDirty();
        }

        private void RemoveClip(int index)
        {
            if (index < 0 || index >= soundbank.audioDatas.Count)
                return;

            var audioData = soundbank.audioDatas[index];
            soundbank.audioDatas.RemoveAt(index);
            SetSoundbankDirty();
        }

        private void Rename(SoundSystem.AudioData audioData, string newName)
        {
            audioData.soundEventID = newName;
            audioData.audioSource.name = newName;
        }

        private bool CheckUnique()
        {
            HashSet<string> ids = new HashSet<string>();
            bool isCollision = false;
            foreach (var audioData in soundbank.audioDatas)
            {
                int i = 1;
                var desireID = audioData.soundEventID;
                while (ids.Contains(audioData.soundEventID))
                {
                    isCollision = true;
                    Rename(audioData, $"{desireID}_{i}");
                    i++;
                }
                ids.Add(audioData.soundEventID);
            }
            if (isCollision)
            {
                SetSoundbankDirty();
            }
            return isCollision;
        }

        private void ClearData()
        {
            // Clear audioSource that is not in the list
            
            // Collect all children
            List<Transform> children = new List<Transform>();
            foreach (Transform child in soundbank.transform)
            {
                children.Add(child);
            }

            // For each child, destroy if it is not in the list
            foreach (var child in children)
            {
                if (soundbank.audioDatas.FindIndex(x => x.audioSource == child.GetComponent<AudioSource>()) < 0)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

    }

}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioVisualization : MonoBehaviour
{
    [SerializeField] private bool showDebugInfo = false;
    [SerializeField] private List<AudioSource> audioSources;
    [SerializeField] private int blockSize = 128;
    [SerializeField] private float updateInterval = 0.1f;
    [SerializeField] private float volumeMultiplier = 1f;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private UnityEvent<float> onOutputLevelChanged;

    private float[] samples;
    private float currentOutputLevel;
    private float timer;
    private Vector3 origScale = Vector3.one;

    void Awake()
    {
        blockSize = Mathf.ClosestPowerOfTwo(blockSize);
        samples = new float[blockSize];
        timer = 0f;
        if (targetTransform)
            origScale = targetTransform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            UpdateOutputLevel();
            timer = 0f;
        }
    }

    void UpdateOutputLevel()
    {
        if (audioSources.Count <= 0)
        {
            AudioListener.GetOutputData(samples, 0);
            float sum = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                sum += Mathf.Abs(samples[i]);
            }
            currentOutputLevel = sum / samples.Length;
        }
        else
        {
            float sum = 0f;
            int activeAudioSources = 0;
            for (int i = 0; i < audioSources.Count; i++)
            {
                if (!audioSources[i] || !audioSources[i].isPlaying)
                {
                    continue;
                }
                activeAudioSources++;
                audioSources[i].GetOutputData(samples, 0);
                for (int j = 0; j < samples.Length; j++)
                {
                    sum += Mathf.Abs(samples[j]);
                }
            }
            if (activeAudioSources > 0)
                currentOutputLevel = sum / (samples.Length * activeAudioSources);
        }
        currentOutputLevel *= volumeMultiplier;

        // Log the current output level (you can remove this in production)
        if (showDebugInfo)
            Debug.Log("Current Audio Output Level: " + currentOutputLevel);
        if (targetTransform)
        {
            targetTransform.localScale = origScale * (currentOutputLevel + 1);
        }
        if (onOutputLevelChanged != null)
            onOutputLevelChanged.Invoke(currentOutputLevel);
    }

    public float GetCurrentOutputLevel()
    {
        return currentOutputLevel;
    }
}
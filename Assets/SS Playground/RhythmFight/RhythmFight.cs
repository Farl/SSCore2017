using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RhythmFight : MonoBehaviour
{
    public static RhythmFight Instance;

    public Image beatImage;

    List<RFCharacter> characters = new List<RFCharacter>();

    public int bpm = 60;

    [System.NonSerialized]
    public float timer = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        if (beatImage)
            beatImage.enabled = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private IEnumerator Start()
    {
        while (true)
        {
            Call();
            yield return new WaitForSeconds(bpm / 60f);
        }
    }

    public void Register(RFCharacter ch)
    {
        characters.Add(ch);
    }

    public void Unregister(RFCharacter ch)
    {
        characters.Remove(ch);
    }

    public RFCharacter GetNearestEnemy(RFCharacter ch)
    {
        if (Instance)
        {
            foreach (var c in characters)
            {
                if (c != ch)
                {
                    return c;
                }    
            }
        }
        return null;
    }

    private IEnumerator BeatCoroutine()
    {
        if (beatImage)
        {
            beatImage.enabled = true;
            yield return new WaitForSeconds(0.1f);
            beatImage.enabled = false;
        }
    }

    private void Call()
    {
        StartCoroutine(BeatCoroutine());
        foreach (var ch in characters)
        {
            ch.Beat();
        }
    }
}

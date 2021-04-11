using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmFight : MonoBehaviour
{
    public static RhythmFight Instance;

    List<RFCharacter> characters = new List<RFCharacter>();

    public float period = 3;

    float timer = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        InvokeRepeating("Call", period, period);
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

    private void Call()
    {
        foreach (var ch in characters)
        {
            ch.Beat();
        }
    }
}

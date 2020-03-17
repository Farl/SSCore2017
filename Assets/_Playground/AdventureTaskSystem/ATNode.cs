using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATNode : MonoBehaviour {

	[SerializeField] private ATNode _parentNode;
    public ATNode parentNode
    {
        get
        {
            return _parentNode;
        }
    }

    public enum Type
    {
        Heavy,
        Fixable,
        Danger,
        Goal,
        Reward
    }

    [SerializeField] private Type _nodeType = Type.Heavy;
    public Type nodeType
    {
        get
        {
            return _nodeType;
        }
    }

    [SerializeField] private float hiddenPointMax;
    private float hiddenPoint;
    [SerializeField] private float lifeMax;
    private float life;
    [SerializeField] private float attack;
	[SerializeField] private float defense;
    [SerializeField] private string _animID = "Jump";
    public string animID
    {
        get
        {
            return _animID;
        }
    }

    private ATCharacter occupyCharacter;

    public void Init()
    {
        hiddenPoint = hiddenPointMax;
        life = lifeMax;
    }

    public void Occupy(ATCharacter character)
    {
        occupyCharacter = character;
	}

    public bool Observe(float observation)
    {
        if (hiddenPoint <= 0)
        {
            return true;
        }
        else
        {
            hiddenPoint -= observation;
            return false;
        }
    }

    public bool Attack(float damage)
    {
        if (life <= 0)
        {
            return true;
        }
        else
        {
            damage -= defense;
            life -= damage;
            return false;
        }
    }
}

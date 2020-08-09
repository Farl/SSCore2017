using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureTask : MonoBehaviour {

    private ATNode[] nodes;
    private List<ATNode> _visiableNodes = new List<ATNode>();
    public List<ATNode> visiableNodes
    {
        get
        {
            return _visiableNodes;
        }
    }
    private bool _isFinished = false;
    public bool IsFinished
    {
        get
        {
            return _isFinished;
        }
    }

    public void Init()
    {
        _isFinished = false;
        Refresh();
    }

    public void Refresh()
    {
        nodes = GetComponentsInChildren<ATNode>(false);
        if (nodes != null)
        {
            _visiableNodes.Clear();
            foreach (ATNode node in nodes)
            {
                node.Init();
                if (node.parentNode == null || node.parentNode.isActiveAndEnabled == false)
                {
                    // Root node
                    _visiableNodes.Add(node);
                }
            }
        }
        if (_visiableNodes.Count <= 0)
        {
            _isFinished = true;
        }
    }
}

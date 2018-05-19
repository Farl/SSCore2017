using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif

public class TestReorderableArray : MonoBehaviour
{
    [System.Serializable]
    public class TestClass2
    {
        public bool b2;
        public int i2;
        public float f2;
        public string s2;
    }

    [System.Serializable]
    public class TestClass
    {
        public bool b;
        public int i;
        public float f;
        public string s;
        public TestClass2 c;
    }

    [System.Serializable]
    public class TestClass3
    {
        public TestClass2[] test;
    }

    [Reorderable]
    public TestClass[] tArray;
    [Reorderable]
    public List<TestClass> tList;
    [Auto]
    public TestClass3 autoArray;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(TestReorderableArray))]
public class TestReorderableArrayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public TestClass[] tArray;
    [Reorderable]
    public List<TestClass> tList;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SS.Core;

public class ReorderableListTest : MonoBehaviour {

	[ReorderableAttribute]
	public List<string> testString = new List<string>();

    [Button("Test", "Test")]
    public bool boolean;
	public List<Vector3> testVec3;

	[System.Serializable]
	public class TestClass
	{
		public bool boolean;
		public string str;
        [ReorderableAttribute]
        public List<string> strList;
	}

	[ReorderableAttribute]
	public List<TestClass> testClass;

	//[AutoAttribute]
	//public List<TestClass> testClass2;

        void Test()
    {
        Debug.Log("Test");
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
